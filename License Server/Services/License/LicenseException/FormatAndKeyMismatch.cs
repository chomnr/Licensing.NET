namespace License_Server.Services.License.LicenseException
{
    public class FormatAndKeyMismatch : FormatException
    {
        public FormatAndKeyMismatch()
       : base("The key does not match the key format.") { }

        public FormatAndKeyMismatch(string message)
       : base(message) { }

        public FormatAndKeyMismatch(string message, FormatException inner)
            : base(message, inner) { }
    }
}
