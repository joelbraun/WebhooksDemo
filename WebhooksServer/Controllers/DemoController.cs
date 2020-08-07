using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebhooksServer.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebhooksServer.Controllers
{
    public class DemoController : Controller
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IWebhookMessageService _webhookMessageService;

        public DemoController(ICertificateRepository certificateRepository,
            IWebhookMessageService webhookMessageService)
        {
            _certificateRepository = certificateRepository;
            _webhookMessageService = webhookMessageService;
        }

        [HttpGet("keys")]
        public IActionResult Keys()
        {
            var data = _certificateRepository.GetPublicKeys();

            return new JsonResult(data);
        }

        [HttpGet("SendMessage")]
        public async Task<IActionResult> SendMessage()
        {
            await _webhookMessageService.SendAsync(new
            {
                Test = "test"
            });

            return Ok();
        }
    }
}
