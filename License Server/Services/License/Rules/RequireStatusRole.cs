using License_Server.Services.License;
using License_Server.Services.License.LicenseException;
using License_Server.Services.Licensing;
using License_Server.Services.Licensing.Rules;
using static License_Server.Services.Licensing.License;

namespace License_Server.Services.Licensing.Rules
{
    /// <summary>
    /// The LICENSE_STATUS must be at least one of the given LICENSE_STATUES
    /// </summary>
    public class RequireStatusRole : IAuthorityRule
    {
        public string Name { get; set; } = "RequireStatusRole";

        private List<LICENSE_STATUS> statues = new List<LICENSE_STATUS>();

        public RequireStatusRole(LICENSE_STATUS[] atLeastOne) { 
            for (int i = 0; i < atLeastOne.Length; i++)
            {
                statues.Add(atLeastOne[i]);
            }
        }

        public LicenseResult Execute(License license, LicenseError error)
        {
            LICENSE_STATUS licenseStatus = license.Status;
            AUTHORITY_STATE state = AUTHORITY_STATE.APPROVED;

            if (statues.Count == 0)
            {
                throw new AuthorityRuleMissingField();
            }

            if (!statues.Contains(licenseStatus))
            {
                state = AUTHORITY_STATE.REJECTED;
            }
            return new LicenseResult(license, state, error);
        }
    }
}
