using License_Server.Services.Licensing;
using License_Server.Services.User;
using Licensing_Server.Services.Licensing;
using Licensing_System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static License_Server.Services.Licensing.LicenseAuthorityUtil;
using Stripe;
using System.Security.Permissions;

namespace License_Server.Controllers
{
    [Route("api/admin/license")]
    [ApiController]
    public class LicenseAdminController : Controller
    {
        private readonly LicenseDbContext _context;
        private readonly ILicenseProcessor _processor;
        private readonly ILicenseProvider _provider;   

        const string secret = "whsec_13a51b76e3b1cc99eba439d820ca97315ea971f9cc6bc9d3fea3e12ebdcd01e0";

        public LicenseAdminController(LicenseDbContext context, ILicenseProcessor processor, ILicenseProvider provider, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _processor = processor;
            _provider = new LicenseProvider(processor);
        }

        [HttpPost("activate")]
        public async Task<IActionResult> PostActivateLicense(string sessionId, string? target, string key)
        {
            // Handle cookie/ permissions here or mask it around a different endpoint.
            throw new NotImplementedException();
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> PostDeactivateLicense(string sessionId, string? target, string key)
        {
            // Handle cookie/ permissions here or mask it around a different endpoint.
            throw new NotImplementedException();
        }

        [HttpPost("suspend")]
        public async Task<IActionResult> PostSuspendLicense(string sessionId, string? target, string key)
        {
            // Handle cookie/ permissions here or mask it around a different endpoint.
            throw new NotImplementedException();
        }
    }
}
