namespace License_Server.Services.License.LicenseException
{
    public class BadLicenseKeyFormat : FormatException
    {
        public BadLicenseKeyFormat()
       : base("The format of the license key is invalid.") { }

        public BadLicenseKeyFormat(string message)
       : base(message) { }

        public BadLicenseKeyFormat(string message, FormatException inner)
            : base(message, inner) { }
    }
}
