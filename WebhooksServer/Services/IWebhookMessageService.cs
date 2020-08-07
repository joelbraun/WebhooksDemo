using System;
using System.Threading.Tasks;

namespace WebhooksServer.Services
{
    public interface IWebhookMessageService
    {
        public Task SendAsync(object data);
    }
}
