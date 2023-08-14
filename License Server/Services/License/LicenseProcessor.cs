using License_Server;
using License_Server.Services.Licensing;
using License_Server.Services.User;
using Licensing_System;
using Microsoft.EntityFrameworkCore;

namespace Licensing_Server.Services.Licensing
{
    public interface ILicenseProcessor
    {
        public void AddLicense(License license);
        public License? FindLicense(LicenseLookUp lookUp);
        public void UpdateLicense(License license);
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

        public License? FindLicense(LicenseLookUp lookUp)
        {
            IQueryable<License> query = _context.Licenses;

            if (lookUp.Owner != null)
            {
                query = query.Where(c1 => c1.Owner == lookUp.Owner);
            }

            if (lookUp.ProductId != null)
            {
                query = query.Where(c1 => c1.ProductId == lookUp.ProductId);
            }

            if (lookUp.Key != null)
            {
                query = query.Where(c1 => c1.Key == lookUp.Key);
            }
            return query.FirstOrDefault();
        }

        public License? FindLicense(string key)
        {
            return _context.Licenses.Where(c1 => c1.Key == key).FirstOrDefault();
        }

        public void UpdateLicense(License license)
        {
            // Update entire license.
            _context.Licenses.Where(c1 => c1.Key == license.Key )
                .ForEachAsync(i => i = license);
        }
    }
}
