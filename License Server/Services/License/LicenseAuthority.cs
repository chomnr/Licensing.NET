using License_Server.Services.Licensing.Authorities;
using Microsoft.AspNetCore.Mvc;
using static License_Server.Services.Licensing.License;
using static License_Server.Services.Licensing.LicenseAuthorityUtil;

namespace License_Server.Services.Licensing
{
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

    public abstract class LicenseAuthority<T> where T : LicenseAuthority<T>
    {
        public LicenseStruct _license;

        public abstract Task<ActionResult<LicenseStruct>> Auto();

        public T CheckStatus()
        {
            if (_license.License != null)
            {
                if (_license.License.Status != LICENSE_STATUS.ACTIVATED)
                {
                    _license.Status = AUTHORITY_STATUS.DENIED;
                }
            }
            return (T)this;
        }

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

    /// <summary>
    /// Handles the authorities for anything related to CreateLicense.
    /// </summary>
   /* public class CreateLicenseAuthorityBuilder : LicenseAuthority
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
    }*/


