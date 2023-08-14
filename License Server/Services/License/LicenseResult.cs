namespace License_Server.Services.Licensing
{
    public class LicenseResult
    {
        public License? License { get; set; }
        public AUTHORITY_STATE AuthorityState { get; set; }

        public LicenseResult() { }
        public LicenseResult(License? license, AUTHORITY_STATE state) { 
            this.License = license;
            this.AuthorityState = state;
        }
    }
}
