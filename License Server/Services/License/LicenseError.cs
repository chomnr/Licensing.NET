using License_Server.Services.Licensing;

namespace License_Server.Services.License
{
    public class LicenseError
    {
        public string State { get; set; } = AUTHORITY_STATE.REJECTED.ToString();
        public string Message { get; set; }
        public List<string> FailedRules { get; set; } = new List<string>();

        public LicenseError(string message) { 
            this.Message = message;
        }
    }
}
