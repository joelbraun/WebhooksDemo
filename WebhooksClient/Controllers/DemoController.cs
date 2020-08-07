using System.IO;
using System.Text;
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

        [HttpPost("recieve")]
        public async Task<IActionResult> Index()
        {
            var signature = Request.Headers["X-Webhook-Signature"];

            using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
            var bodyPayload =  await reader.ReadToEndAsync();

            var result = await _signedHashValidatorService.Validate(signature, bodyPayload);

            if (!result)
            {
                return new UnauthorizedResult();
            }

            return Ok();
        }
    }
}
