using License_Server.Services.License.Rules;
using License_Server.Services.Licensing;
using License_Server.Services.Licensing.Rules;
using License_Server.Services.User;
using System.Diagnostics;
using static License_Server.Services.Licensing.License;
//client -> mediator(does authenticatin) -> license server. 
// client -> buys product -> /buy/software1 -> stripe transaction completed med
//
//
namespace Licensing_Server.Services.Licensing
{
    public class LicenseDelegation
    {
        public delegate Task<LicenseResult> LicenseCreate(License license);
        public delegate Task<LicenseResult> LicenseValidate(UserSession session, string productId);
        public delegate Task<LicenseResult> LicenseActivate(string key);
    }

    public class LicenseHandler
    {
        /// <summary>
        /// Instantiating <c>LicenseProcessor</c> so the processing functions can be accessed.
        /// </summary>
        private readonly ILicenseProcessor Processor;

        /// <summary>
        /// ....
        /// </summary>
        private readonly ILicenseAuthority Authority;

        /// <summary>
        /// LicenseHandler...
        /// </summary>
        /// <param name="processor"></param>
        public LicenseHandler(ILicenseProcessor processor) {
            this.Processor = processor;
            this.Authority = new LicenseAuthority(this.Processor);
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
        /// Whenever <c>LicenseProvider.ActivateLicense</c> is called LicenseValidateEvent will be called.
        /// </summary>
        #pragma warning disable
        public event LicenseDelegation.LicenseActivate? OnLicenseActivate;

        /// <summary>
        ///  CreateLicenseEvent
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns> 
        public async Task<LicenseResult> CreateLicenseEvent(License license)
        {
            // Disabled Authority for CreateLicense; until I found what rules I can add
            // and to avoid confliction with Stripe.
            Processor.AddLicense(license);
            return new LicenseResult(license, AUTHORITY_STATE.APPROVED);
        }

        /// <summary>
        /// ValidateLicenseEvent
        /// </summary>
        /// <param name="session"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<LicenseResult> ValidateLicenseEvent(UserSession session, string productId)
        {
            LicenseLookUp lookUp = new LicenseLookUp(productId, session.Id, null);
            IAuthorityRule[] rules = { new NoExpirationRule(true), new RequireActivationRule() };
            LicenseResult result = await Authority
                .AddRules(rules)
                .RunOn(lookUp);
            return result;
        }

        /// <summary>
        /// LicenseActivateEvent.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<LicenseResult> LicenseActivateEvent(string key)
        {
            /*
            ActivateLicenseAuthorityBuilder authorityBuilder = new(Processor, key);
            var result = await authorityBuilder
                .CheckStatus()
                .CheckForOwner()
                .CheckPurchaseDate()
                .Auto();

            if (result.Value.AuthorityStatus == AUTHORITY_STATUS.APPROVED)
            {
                Processor.UpdateLicense(result.Value.License);
            }
            */
            //return result.Value;
            throw new NotImplementedException();
        }
    }
}


//LicenseAuthority licenseAuthority = new LicenseAuthority()
// licenseAuthority.AddRules(license, [NO_EXPIRY, CHECK_FOR_ACTIVATION]);
// LicenseAuthorityResultvbcxz
/*
ValidateLicenseAuthorityBuilder authorityBuilder = new(Processor, session, productId);
var result = await authorityBuilder
    .CheckStatus()
    .CheckExpiration()
    .Auto();

if (result.Value.AuthorityStatus == AUTHORITY_STATUS.DENIED)
{
    // If AuthorityStatus is anything but approved that means something is wrong with the license.
    // expired, the current status etc;
    Processor.UpdateLicense(result.Value.License);
}
 */

//.SetProcessor(processor)
// do authority checks.
//return License.LICENSE_STATUS.ACTIVATED;
//authority, check if license has not expired, 
// check if a transaciton exists

//get session id -> get id of user -> get product id. Find if the user has
// a product of Id and check if it's expired, check if novice was paid etc;
// need a custom method that can be

/*
CreateLicenseAuthorityBuilder authorityBuilder = new CreateLicenseAuthorityBuilder(license);
var result = await authorityBuilder
    .CheckFormat()
    .Approve();

if (result.Value.Status == AUTHORITY_STATUS.APPROVED) {
    Processor.AddLicense(license);
}
*/
// Disabled Authority for CreateLicense; until I found what rules I can add
// and to avoid confliction with Stripe.