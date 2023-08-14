using License_Server.Services.License;
using License_Server.Services.Licensing;
using License_Server.Services.Licensing.Rules;
using Microsoft.IdentityModel.Tokens;

namespace License_Server.Services.Licensing.Rules
{
    /// <summary>
    /// Require that the license has a owner.
    /// </summary>
    public class RequireOwnershipRule : IAuthorityRule
    {
        public LicenseResult Execute(License license, LicenseError error)
        {
            AUTHORITY_STATE state = AUTHORITY_STATE.APPROVED;
            string? owner = license.Owner;

            if (string.IsNullOrEmpty(owner)) {
                state = AUTHORITY_STATE.REJECTED;
            }
            return new LicenseResult(license, state, error);
        }
    }
}
