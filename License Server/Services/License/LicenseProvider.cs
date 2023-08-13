using License_Server.Services.LicenseService;
using License_Server.Services.User;
using Licensing_Server.Services.Licensing;
using Licensing_System.Services.Licensing;
using static System.Net.Mime.MediaTypeNames;

namespace License_Server.Services.Licensing
{
    public interface ILicenseProvider
    {
        // Create License
        public Task<LicenseStruct> CreateLicense(LicenseGen lgen);
        public Task<LicenseStruct> CreateLicense(string owner, string productId);

        // Validate License
        public Task<LicenseStruct> ValidateLicense(UserSession session, string productId);
    }

    public class LicenseProvider : ILicenseProvider
    {
        private readonly LicenseHandler handler;

        public LicenseProvider(ILicenseProcessor processor)
        {
            this.handler = new LicenseHandler(processor);
            //this.processor = processor;
        }

        /// <summary>
        /// Creates license with LicenseGen directly. Licenses are automatically set to activated. If LICENSE_STATUS is
        /// not specified.
        /// </summary>
        /// <param name="lgen"></param>
        public async Task<LicenseStruct> CreateLicense(LicenseGen lgen)
        {
            handler.OnLicenseCreate += new LicenseDelegation.LicenseCreate(handler.CreateLicenseEvent);
            return await handler.CreateLicenseEvent(lgen.Build());
        }

        /// <summary>
        /// Indirectly use License. Licenses are automatically set to activated. If LICENSE_STATUS is not specified.
        /// </summary>
        /// <param name="buyerId"></param>
        /// <param name="productId"></param>
        public async Task<LicenseStruct> CreateLicense(string buyerId, string productId) 
        {
            handler.OnLicenseCreate += new LicenseDelegation.LicenseCreate(handler.CreateLicenseEvent);
            return await handler.CreateLicenseEvent(new LicenseGen()
                .SetOwner(buyerId)
                .SetProduct(productId)
                .Build());
        }

        /// <summary>
        /// Ensures that the key is still valid and active.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<LicenseStruct> ValidateLicense(UserSession session, string productId)
        {
            // Use LicensesController to interpret the data. and then store the results
            // inside new UserSession(id, name(nullable))
            handler.OnLicenseValidate += new LicenseDelegation.LicenseValidate(handler.ValidateLicenseEvent);
            return await handler.ValidateLicenseEvent(session, productId);
        }
    }
}
