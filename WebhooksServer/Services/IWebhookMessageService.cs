using System.Threading.Tasks;

namespace WebhooksServer.Services
{
    /// <summary>
    /// Manages sending webhooks messages
    /// </summary>
    public interface IWebhookMessageService
    {
        /// <summary>
        /// Send an object to the client application in a serialized format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task SendAsync<T>(T data) where T : class;
    }
}
