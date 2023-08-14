using License_Server.Services.Licensing;

namespace License_Server.Services.License
{
    public class LicenseError
    {
        public string state { get; set; } = AUTHORITY_STATE.REJECTED.ToString();
        public string message { get; set; }

        public LicenseError(string message) { 
            this.message = message;
        }
    }
}
