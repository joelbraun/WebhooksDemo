using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebhooksServer.Services
{
    public class WebhookMessageService : IWebhookMessageService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public WebhookMessageService(ICertificateRepository certificateRepository,
            IHttpClientFactory httpClientFactory)
        {
            _certificateRepository = certificateRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendAsync(object data)
        {
            var serialized = JsonSerializer.Serialize(data);

            var hash = GenerateHash(serialized);

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Webhook-Signature", hash);
            await client.PostAsync("https://localhost:40344/recieve", new StringContent(serialized));
        }

        /// <inheritdoc cref="ISignedHashService.Generate(string)" />
        private string GenerateHash(string data)
        {
            using var hasher = SHA256.Create();
            var hashValue = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));

            var signingCertificate = _certificateRepository.GetPrimarySigningCertificate();

            using var rsa = signingCertificate.GetRSAPrivateKey();
            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm("SHA256");

            return Convert.ToBase64String(rsaFormatter.CreateSignature(hashValue));
        }
    }
}
