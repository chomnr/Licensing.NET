using License_Server.Services.License;

namespace License_Server.Services.Licensing
{
    public class LicenseResult
    {
        public AUTHORITY_STATE AuthorityState { get; set; }
        public License? License { get; set; }
        public LicenseError? Error { get; set; }

        public LicenseResult() { }

        public LicenseResult(License? license, AUTHORITY_STATE state, LicenseError? error) { 
            this.License = license;
            this.AuthorityState = state;
            this.Error = error;
        }

        public LicenseResult(LicenseError error, AUTHORITY_STATE state)
        {
            this.Error = error;
            this.AuthorityState = state;
            if (state == AUTHORITY_STATE.REJECTED)
            {
                error.state = state.ToString();
            }
        }
    }
}
