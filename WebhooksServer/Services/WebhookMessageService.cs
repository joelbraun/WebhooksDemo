using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebhooksServer.Services
{
    public class WebhookMessageService : IWebhookMessageService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string HeaderType = "X-Webhook-Signature";
        private const string Recipient = "https://localhost:40344/receive";

        public WebhookMessageService(ICertificateRepository certificateRepository,
            IHttpClientFactory httpClientFactory)
        {
            _certificateRepository = certificateRepository;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc cref="IWebhookMessageService.SendAsync(object)" />
        public async Task SendAsync<T>(T data) where T: class
        {
            using var serialized = new MemoryStream();
            await JsonSerializer.SerializeAsync<T>(serialized, data);
            var hash = GenerateHash(serialized);

            // Avoid using PostAsync to prevent concurrency issues with header dictionary
            var message = new HttpRequestMessage(HttpMethod.Post, Recipient);
            message.Headers.Add(HeaderType, hash);
            message.Content = new StreamContent(serialized);

            var client = _httpClientFactory.CreateClient();
            await client.SendAsync(message);
        }

        private string GenerateHash(Stream data)
        {
            using var hasher = SHA256.Create();
            var hashValue = hasher.ComputeHash(data);

            var signingCertificate = _certificateRepository.GetPrimarySigningCertificate();
            using var rsa = signingCertificate.GetRSAPrivateKey();
            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm("SHA256");

            return Convert.ToBase64String(rsaFormatter.CreateSignature(hashValue));
        }
    }
}
