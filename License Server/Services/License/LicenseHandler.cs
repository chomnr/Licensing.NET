﻿using License_Server.Services.License.Rules;
using License_Server.Services.Licensing;
using License_Server.Services.Licensing.Rules;
using License_Server.Services.User;
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
            return new LicenseResult(license, AUTHORITY_STATE.APPROVED, null);
        }

        /// <summary>
        /// ValidateLicenseEvent
        /// </summary>
        /// <param name="session"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<LicenseResult> ValidateLicenseEvent(UserSession session, string productId)
        {
            //todo remove modifers from rules have them manually handled
            LicenseLookUp lookUp = new LicenseLookUp(productId, session.Id, null);
            IAuthorityRule[] rules = { new RequireExpirationRule(true), new RequireActivationRule() };
            LicenseResult result = await Authority
                .SetErrorMessage("Verification has failed either the license has expired or is invalid.")
                .AddRules(rules)
                .RunOn(lookUp);
            return result;
        }

        /// <summary>
        /// LicenseActivateEvent. (Admin Command)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<LicenseResult> LicenseActivateEvent(string key)
        {
            LicenseLookUp lookUp = new LicenseLookUp(null, null, key);
            LICENSE_STATUS[] statusToLookFor = new LICENSE_STATUS[] { LICENSE_STATUS.SUSPENDED, LICENSE_STATUS.UNCLAIMED };
            IAuthorityRule[] rules = { new RequireStatusRole(statusToLookFor), new RequireOwnershipRule(), new RequireDurationRule() };
            LicenseResult result = await Authority
                .SetErrorMessage("Activation has failed either because the license does not exist, is deactivated, is currently activated, has no owner, or the duration is set to 0.")
                .AddRules(rules)
                .RunOn(lookUp);

            if ( result.AuthorityState == AUTHORITY_STATE.APPROVED)
            {
                var status = result.License.Status;
                if (status == LICENSE_STATUS.SUSPENDED)
                {
                    result.License.PurchaseDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
                result.License.Status = LICENSE_STATUS.ACTIVATED;
            }
            return result;
        }
    }
}

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