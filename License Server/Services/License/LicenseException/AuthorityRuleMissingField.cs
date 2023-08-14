namespace License_Server.Services.License.LicenseException
{
    public class AuthorityRuleMissingField : MissingFieldException
    {
        public AuthorityRuleMissingField()
       : base("One of the fields are missing.") { }

        public AuthorityRuleMissingField(string message)
       : base(message) { }

        public AuthorityRuleMissingField(string message, FormatException inner)
            : base(message, inner) { }
    }
}
