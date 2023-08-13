using License_Server.Services.LicenseService;
using License_Server.Services.User;
using Licensing_System;

namespace Licensing_Server.Services.Licensing
{
    public interface ILicenseProcessor
    {
        public void AddLicense(License license);
        public License? FindLicense(UserSession session, string productId);
    }

    public class LicenseProcessor: ILicenseProcessor
    {
        private readonly LicenseDbContext _context;

        public LicenseProcessor(LicenseDbContext context) {
            _context = context;
        }

        public void AddLicense(License license)
        {
            _context.Licenses.Add(license);
        }

        public License? FindLicense(UserSession session, string productId)
        {
            return _context.Licenses
                .Where(c1 => c1.Owner == session.Id && c1.ProductId == productId)
                .FirstOrDefault();
        }
    }
}
