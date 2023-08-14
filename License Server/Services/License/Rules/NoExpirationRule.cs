
using static License_Server.Services.Licensing.License;

namespace License_Server.Services.Licensing.Rules
{
    public class NoExpirationRule : IAuthorityRule
    {
        private readonly bool Modify = false;

        public NoExpirationRule() { }
        public NoExpirationRule(bool Modify)
        {
            this.Modify = Modify;
        }

        public LicenseResult Execute(License license)
        {
            AUTHORITY_STATE state = AUTHORITY_STATE.APPROVED;

            long current = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long purchaseDate = license.PurchaseDate;
            long duration = license.Duration;

            if (current > (purchaseDate + duration)) {
                state = AUTHORITY_STATE.REJECTED;
                if (Modify)
                {
                    license.PurchaseDate = 0;
                    license.Duration = 0;
                    license.Status = LICENSE_STATUS.DEACTIVATED;
                }
            }
            return new LicenseResult(license, state);
        }
    }
}
