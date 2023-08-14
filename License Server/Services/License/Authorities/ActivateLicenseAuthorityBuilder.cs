using License_Server.Services.Licensing;
using Licensing_Server.Services.Licensing;
using Microsoft.AspNetCore.Mvc;
using static License_Server.Services.Licensing.LicenseAuthorityUtil;

namespace License_Server.Services.Licensing.Authorities
{
    public class ActivateLicenseAuthorityBuilder : LicenseAuthority<ActivateLicenseAuthorityBuilder>
    {
        private ILicenseProcessor Processor { get; set; }

        public ActivateLicenseAuthorityBuilder(ILicenseProcessor processor, String key)
        {
            this.Processor = processor;
            try
            {
                License? lookForLicense = Processor.FindLicense(key);
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

       // public ActivateLicenseAuthorityBuilder CheckStatus()
       // {
                     
        //}

        //check for already activation
        //check if a owner exists
        //check if product exists? (maybe this one)

        public async override Task<ActionResult<LicenseStruct>> Auto()
        {
            if (_license.Status == AUTHORITY_STATUS.PENDING)
            {
                _license.Status = AUTHORITY_STATUS.APPROVED;
            }

            return await Task.FromResult(_license);
        }
    }
}
