using License_Server;
using License_Server.Services.Licensing;
using License_Server.Services.User;
using Licensing_Server.Services.Licensing;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using LicenseProvider = License_Server.Services.Licensing.LicenseProvider;

namespace Licensing_System.Controllers
{
    /* 
        EXAMPLE STRIPE INTEGRATION
    */

    [Route("api/license")]
    [ApiController]
    public class LicenseController : Controller
    {
        private readonly LicenseDbContext _context;
        private readonly ILicenseProcessor _processor;
        private readonly ILicenseProvider _provider;

        const string secret = "whsec_13a51b76e3b1cc99eba439d820ca97315ea971f9cc6bc9d3fea3e12ebdcd01e0";


        public LicenseController(LicenseDbContext context, ILicenseProcessor processor, ILicenseProvider provider)
        {
            _context = context;
            _processor = processor;
            _provider = new LicenseProvider(_processor);
        }

        /*
        [HttpPost("/api/admin/license/activate")]
        public async Task<IActionResult> PostActivateLicense(string key)
        {
            // Handle cookie/ permissions here or mask it around a different endpoint.
            LicenseStruct result = await _provider.ActivateLicense(key);
            await _context.SaveChangesAsync();
            return Ok(result.AuthorityStatus.ToString());
        }*/

        [HttpPost("generate")]
        public async Task<IActionResult> PostGenerateLicense()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                if (String.IsNullOrEmpty(json)) { return BadRequest(); }

                var headers = Request.Headers["Stripe-Signature"];
                var @event = EventUtility.ConstructEvent(json, headers, secret);

                if (@event != null)
                {
                    // Payment Intent Succeeded Event
                    if (@event.Type == Events.PaymentIntentSucceeded)
                    {
                        await _provider.CreateLicense(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok();
            }
            catch (StripeException)
            {
                // Exception does not catch StripeException for some reason.
                return Challenge();
            }
            catch (Exception ex2)
            {
                return BadRequest(ex2);
            }
        }

        [HttpGet("verify")]
        public async Task<IActionResult> GetVerifyLicense(string sessionId, string productId)
        {
            //todo: for YOU; get session token(id), then get id of user.
            UserSession session = new UserSession(sessionId);
            LicenseResult result = await _provider.ValidateLicense(session, productId);
            if (result.License != null)
            {
                if (result.AuthorityState != AUTHORITY_STATE.APPROVED)
                {
                    await _context.SaveChangesAsync();
                }
            } else
            {
                return Ok(Json(result.AuthorityState.ToString()).Value);
            }
            return Ok(Json(result).Value);
        }
    }
}


/*
 

       [HttpPost("activate")]
        public async Task<IActionResult> PostActivateLicense(string key)
        {
            // Handle cookie/ permissions here or mask it around a different endpoint.
            var result = await _provider.ActivateLicense(key);
            await _context.SaveChangesAsync();
            return Ok(result.Status.ToString());
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
 */