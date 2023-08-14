using License_Server.Services.User;
using Licensing_Server.Services.Licensing;
using static System.Net.Mime.MediaTypeNames;

namespace License_Server.Services.Licensing
{
    public interface ILicenseProvider
    {
        // Create License
        public Task<LicenseResult> CreateLicense(LicenseGen lgen);
        public Task<LicenseResult> CreateLicense(string owner, string productId);

        // Validate License
        public Task<LicenseResult> ValidateLicense(UserSession session, string productId);

        //ActivateLicense
        public Task<LicenseResult> ActivateLicense(string key);
    }

    public class LicenseProvider : ILicenseProvider
    {
        private readonly LicenseHandler Handler;

        private LicenseAuthority Authority;
        
        public LicenseProvider(ILicenseProcessor processor)
        {
            this.Handler = new LicenseHandler(processor);
            this.Authority = new LicenseAuthority(processor);
            //this.processor = processor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<LicenseResult> ActivateLicense(string key)
        {
            Handler.OnLicenseActivate += new LicenseDelegation.LicenseActivate(Handler.LicenseActivateEvent);
            return await Handler.LicenseActivateEvent(key);
        }

        /// <summary>
        /// Creates license with LicenseGen directly. Licenses are automatically set to activated. If LICENSE_STATUS is
        /// not specified.
        /// </summary>
        /// <param name="lgen"></param>
        public async Task<LicenseResult> CreateLicense(LicenseGen lgen)
        {
            Handler.OnLicenseCreate += new LicenseDelegation.LicenseCreate(Handler.CreateLicenseEvent);
            return await Handler.CreateLicenseEvent(lgen.Build());
        }

        /// <summary>
        /// Indirectly use License. Licenses are automatically set to activated. If LICENSE_STATUS is not specified.
        /// </summary>
        /// <param name="buyerId"></param>
        /// <param name="productId"></param>
        public async Task<LicenseResult> CreateLicense(string buyerId, string productId) 
        {
            Handler.OnLicenseCreate += new LicenseDelegation.LicenseCreate(Handler.CreateLicenseEvent);
            return await Handler.CreateLicenseEvent(new LicenseGen()
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
        public async Task<LicenseResult> ValidateLicense(UserSession session, string productId)
        {
            // Use LicensesController to interpret the data. and then store the results
            // inside new UserSession(id, name(nullable))
            /*
            authority.AddRules({
                    NO_EXPIRY, 
                    NO_PURCHASEDATE, 
                    RULE_3, 
                    RULE_4, 
                    RULE_5})*/

            Handler.OnLicenseValidate += new LicenseDelegation.LicenseValidate(Handler.ValidateLicenseEvent);
            return await Handler.ValidateLicenseEvent(session, productId);
        }
    }
}
