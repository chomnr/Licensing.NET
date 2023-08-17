using License_Server.Services.License;
using License_Server.Services.Licensing;

namespace License_Server.Services.Licensing.Rules
{
    public interface IAuthorityRule
    {
        string Name { get; set; }

        LicenseResult Execute(License license, LicenseError error);
    }
}
