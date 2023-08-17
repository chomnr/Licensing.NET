using Azure.Core;
using License_Server.Services.Licensing;
using License_Server.Services.User;
using Licensing_Server.Services.Licensing;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace License_Server.Controllers
{
    [Route("api/admin/license")]
    [ApiController]
    public class LicenseAdminController : Controller
    {
        private readonly LicenseDbContext _context;
        private readonly ILicenseProcessor _processor;
        private readonly ILicenseProvider _provider;

        public LicenseAdminController(LicenseDbContext context, ILicenseProcessor processor, ILicenseProvider provider)
        {
            _context = context;
            _processor = processor;
            _provider = new LicenseProvider(_processor);
        }

        [HttpPost("activate")]
        public async Task<IActionResult> PostActivateLicense(string key)
        {
            // Handle cookie/ permissions here or mask it around a different endpoint.
            // LicenseStruct result = await _provider.ActivateLicense(key);
            LicenseResult result = await _provider.ActivateLicense(key);
            if (result.AuthorityState != AUTHORITY_STATE.APPROVED)
            {
                return Ok(Json(result.Error).Value);
            } else
            {
                await _context.SaveChangesAsync();
            }
            return Ok(Json(result.License).Value);
        }
    }
}
