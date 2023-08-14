using License_Server.Services.User;
using Licensing_Server.Services.Licensing;
using Microsoft.AspNetCore.Mvc;
using static License_Server.Services.Licensing.License;
using static License_Server.Services.Licensing.LicenseAuthorityUtil;

namespace License_Server.Services.Licensing.Authorities
{
    public class ValidateLicenseAuthorityBuilder : LicenseAuthority<ValidateLicenseAuthorityBuilder>
    {
        private ILicenseProcessor Processor { get; set; }

        private UserSession Session { get; set; }
        private string ProductId { get; set; }

        public ValidateLicenseAuthorityBuilder(ILicenseProcessor processor, UserSession session, string productId)
        {
            this.Session = session;
            this.ProductId = productId;
            this.Processor = processor;
            try
            {
                License? lookForLicense = Processor.FindLicense(Session, ProductId);
                if (lookForLicense != null)
                {
                    _license.License = lookForLicense;
                }
            }
            catch (Exception)
            {
                _license.Status = AUTHORITY_STATUS.DENIED;
            }
        }

        public ValidateLicenseAuthorityBuilder CheckExpiration()
        {
            if (_license.License != null)
            {
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                long expiration = _license.License.PurchaseDate + _license.License.Duration;

                if (currentTime > expiration)
                {
                    _license.License.Status = License.LICENSE_STATUS.DEACTIVATED;
                    _license.Status = AUTHORITY_STATUS.DENIED;
                }
            }
            return this;
        }

        /*
        public ValidateLicenseAuthorityBuilder CheckStatus()
        {
            if (_license.License != null)
            {
                if (_license.License.Status != LICENSE_STATUS.ACTIVATED)
                {
                    _license.Status = AUTHORITY_STATUS.DENIED;
                    return this;
                }
            }
            return this;
        }
        */

        public async override Task<ActionResult<LicenseStruct>> Auto()
        {
            // If the AuthorityStatus is still pending at this point
            // that means all the checks worked.
            if (_license.Status == AUTHORITY_STATUS.PENDING)
            {
                _license.Status = AUTHORITY_STATUS.APPROVED;
            }

            return await Task.FromResult(_license);
        }
    }
}
