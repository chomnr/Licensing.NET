using License_Server.Services.LicenseService;
using License_Server.Services.User;
using Licensing_System.Services.Licensing;
using static Licensing_System.Services.Licensing.LicenseAuthorityUtil;
//client -> mediator(does authenticatin) -> license server. 
// client -> buys product -> /buy/software1 -> stripe transaction completed med
//
//
namespace Licensing_Server.Services.Licensing
{
    public class LicenseDelegation
    {
        public delegate Task<LicenseStruct> LicenseCreate(License license);
        public delegate Task<LicenseStruct> LicenseValidate(UserSession session, string productId);
    }

    public class LicenseHandler
    {
        /// <summary>
        /// Instantiating <c>LicenseProcessor</c> so the processing functions can be accessed.
        /// </summary>
        private readonly ILicenseProcessor Processor;

        /// <summary>
        /// LicenseHandler...
        /// </summary>
        /// <param name="processor"></param>
        public LicenseHandler(ILicenseProcessor processor) {
            this.Processor = processor;
        }

        /// <summary>
        /// Whenever <c>LicenseProvider.CreateLicense</c> is called CreateLicenseEvent will be called.
        /// </summary>
        #pragma warning disable
        public event LicenseDelegation.LicenseCreate? OnLicenseCreate;

        /// <summary>
        /// Whenever <c>LicenseProvider.ValidateLicense</c> is called LicenseValidateEvent will be called.
        /// </summary>
        #pragma warning disable
        public event LicenseDelegation.LicenseValidate? OnLicenseValidate;

        /// <summary>
        ///  
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns> 
        public async Task<LicenseStruct> CreateLicenseEvent(License license)
        {
            CreateLicenseAuthorityBuilder authorityBuilder = new CreateLicenseAuthorityBuilder(license);
            var result = await authorityBuilder
                .CheckFormat()
                .Approve();

            if (result.Value.Status == AUTHORITY_STATUS.APPROVED) {
                Processor.Add(license);
            }

            return result.Value;
        }

        public async Task<LicenseStruct> ValidateLicenseEvent(UserSession session, string productId)
        {
            ValidateLicenseAuthorityBuilder authorityBuilder = new(Processor, session, productId);
            var result = await authorityBuilder
                .CheckStatus()
                .CheckExpiration()
                .Auto();
            return result.Value;
            //.SetProcessor(processor)
            // do authority checks.
            //return License.LICENSE_STATUS.ACTIVATED;
            //authority, check if license has not expired, 
            // check if a transaciton exists

            //get session id -> get id of user -> get product id. Find if the user has
            // a product of Id and check if it's expired, check if novice was paid etc;
            // need a custom method that can be
        }
    }
}