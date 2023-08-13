using License_Server.Services.LicenseService;
using License_Server.Services.User;
using Licensing_Server.Services.Licensing;
using Microsoft.AspNetCore.Mvc;
using static Licensing_System.Services.Licensing.LicenseAuthorityUtil;

namespace Licensing_System.Services.Licensing
{

    /// <summary>
    /// Returns the user License and the AuthorityStatus.
    /// </summary>
    public struct LicenseStruct
    {
        public AUTHORITY_STATUS Status { get; set; } = AUTHORITY_STATUS.PENDING;
        public License? License { get; set; }

        public LicenseStruct() { }
        public LicenseStruct(License? License, AUTHORITY_STATUS Status)
        {
            this.License = License;
            this.Status = Status;
        }
    }

    public abstract class LicenseAuthority
    {
        public abstract Task<ActionResult<LicenseStruct>> Auto();

        [Obsolete("Use .Auto() instead, will not return a License because it was approved Forcefully.")]
        public async Task<ActionResult<LicenseStruct>> Approve()
        {
            return await Task.FromResult(new LicenseStruct(null, AUTHORITY_STATUS.APPROVED));
        }

        [Obsolete("Use .Auto() instead")]
        public async Task<ActionResult<LicenseStruct>> Deny()
        {
            return await Task.FromResult(new LicenseStruct(null, AUTHORITY_STATUS.DENIED));
        }
    }

    /// <summary>
    /// Handles the authorities for anything related to CreateLicense.
    /// </summary>
    public class CreateLicenseAuthorityBuilder : LicenseAuthority
    {
        private LicenseStruct _license = new LicenseStruct();

        public CreateLicenseAuthorityBuilder(License license)
        {
            this._license.License = license;
        }

        //check expiration date, 

        public CreateLicenseAuthorityBuilder CheckFormat()
        {
            return this;
        }

        public async override Task<ActionResult<LicenseStruct>> Auto()
        {
            return await Task.FromResult(_license);
        }
    }

    /// <summary>
    /// Handles the authorities for anything related to ValidateLicense.
    /// </summary>
    public class ValidateLicenseAuthorityBuilder : LicenseAuthority 
    {
        private LicenseStruct _license = new LicenseStruct();

        private ILicenseProcessor Processor { get; set; }

        private UserSession Session { get; set; }
        private string ProductId { get; set; }

        public ValidateLicenseAuthorityBuilder(ILicenseProcessor processor, UserSession session, string productId) {
            this.Session = session;
            this.ProductId = productId;
            this.Processor = processor;
            try
            {
                License? lookForLicense = Processor.FindLicense(session, productId);
                if (lookForLicense != null)
                {
                    _license.License = lookForLicense;
                }
            } catch (Exception)
            {
                _license.Status = AUTHORITY_STATUS.DENIED;
            }
        }

        public ValidateLicenseAuthorityBuilder CheckExpiration()
        {
            if ( _license.License != null )
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

        public ValidateLicenseAuthorityBuilder CheckStatus()
        {
            if (_license.License != null)
            {
                if (_license.License.Status != License.LICENSE_STATUS.ACTIVATED )
                {
                    _license.Status = AUTHORITY_STATUS.DENIED;
                    return this;
                }
            }
            return this;
        }

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

    public class LicenseAuthorityUtil
    {
        public enum AUTHORITY_STATUS
        {
            APPROVED,
            PENDING,
            DENIED
        }
    }
}

