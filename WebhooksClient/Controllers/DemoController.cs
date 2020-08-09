using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebhooksClient.Services;

namespace WebhooksClient.Controllers
{
    public class DemoController : Controller
    {
        private readonly ISignedHashValidatorService _signedHashValidatorService;

        public DemoController(ISignedHashValidatorService signedHashValidatorService)
        {
            _signedHashValidatorService = signedHashValidatorService;
        }

        [HttpPost("receive")]
        public async Task<IActionResult> Index()
        {
            // Read signature in from request header
            var signature = Request.Headers["X-Webhook-Signature"];

            // Compare against body request stream
            var result = await _signedHashValidatorService.ValidateAsync(signature, Request.Body);

            if (!result)
            {
                return new UnauthorizedResult();
            }

            return Ok();
        }
    }
}
