using License_Server.Services.License;
using License_Server.Services.Licensing;
using License_Server.Services.Licensing.Rules;
using Microsoft.IdentityModel.Tokens;

namespace License_Server.Services.Licensing.Rules
{
    /// <summary>
    /// Require a duration that is greater than 0.
    /// </summary>
    public class RequireDurationRule : IAuthorityRule
    {
        public LicenseResult Execute(License license, LicenseError error)
        {
            AUTHORITY_STATE state = AUTHORITY_STATE.APPROVED;
            long duration = license.Duration;

            if (duration <= 0) {
                state = AUTHORITY_STATE.REJECTED;
            }
            return new LicenseResult(license, state, error);
        }
    }
}
