using License_Server.Services.Licensing;
using License_Server.Services.Licensing.Rules;
using static License_Server.Services.Licensing.License;

namespace License_Server.Services.License.Rules
{
    public class RequireUnclaimedRule : IAuthorityRule
    {
        public string Name { get; set; } = "RequireUnclaimedRule";

        public LicenseResult Execute(Licensing.License license, LicenseError error)
        {
            LICENSE_STATUS licenseStatus = license.Status;
            AUTHORITY_STATE state = AUTHORITY_STATE.APPROVED;

            if (licenseStatus != LICENSE_STATUS.UNCLAIMED)
            {
                state = AUTHORITY_STATE.REJECTED;
            }
            return new LicenseResult(license, state, error);
        }
    }
}
