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
        public LicenseStruct CreateLicense(LicenseGen lgen);
        public LicenseStruct CreateLicense(string owner, string productId);

        // Validate License
        public LicenseStruct ValidateLicense(UserSession session, string productId);
    }

    public class LicenseProvider : ILicenseProvider
    {
        private readonly LicenseHandler handler;
        private readonly ILicenseProcessor processor;

        public LicenseProvider(ILicenseProcessor processor)
        {
            this.handler = new LicenseHandler(processor);
            this.processor = processor;
        }

        /// <summary>
        /// Creates license with LicenseGen directly. Licenses are automatically set to activated. If LICENSE_STATUS is
        /// not specified.
        /// </summary>
        /// <param name="lgen"></param>
        public LicenseStruct CreateLicense(LicenseGen lgen)
        {
            handler.OnLicenseCreate += new LicenseDelegation.LicenseCreate(handler.CreateLicenseEvent);
            return handler.CreateLicenseEvent(lgen.Build()).Result;
        }

        /// <summary>
        /// Indirectly use License. Licenses are automatically set to activated. If LICENSE_STATUS is not specified.
        /// </summary>
        /// <param name="buyerId"></param>
        /// <param name="productId"></param>
        public LicenseStruct CreateLicense(string buyerId, string productId) 
        {
            handler.OnLicenseCreate += new LicenseDelegation.LicenseCreate(handler.CreateLicenseEvent);
            return handler.CreateLicenseEvent(new LicenseGen()
                .SetOwner(buyerId)
                .SetProduct(productId)
                .Build()).Result;
        }

        /// <summary>
        /// Ensures that the key is still valid and active.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public LicenseStruct ValidateLicense(UserSession session, string productId)
        {
            // Use LicensesController to interpret the data. and then store the results
            // inside new UserSession(id, name(nullable))
            handler.OnLicenseValidate += new LicenseDelegation.LicenseValidate(handler.ValidateLicenseEvent);
            return handler.ValidateLicenseEvent(session, productId).Result;
        }
    }
}
