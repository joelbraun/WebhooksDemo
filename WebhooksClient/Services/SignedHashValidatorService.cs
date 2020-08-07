using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace WebhooksClient.Services
{
    public class SignedHashValidatorService : ISignedHashValidatorService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private List<Models.JsonWebKey> _keys = null;

        private static string KeySetUri = "https://localhost:3023/keys";

        public SignedHashValidatorService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> Validate(string hash, string payload)
        {
            byte[] signature = null;
            try
            {
                signature = Base64UrlEncoder.DecodeBytes(hash);
            }
            catch (Exception)
            {
                return false;
            }

            if (_keys == null)
            {
                await GetKeys();
            }

            var data = _keys.Select(x => RSA.Create(new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(x.n),
                Exponent = Base64UrlEncoder.DecodeBytes(x.e)
            }));

            return data.Any(x =>
            {
                using var hasher = SHA256.Create();
                var hashValue = hasher.ComputeHash(Encoding.UTF8.GetBytes(payload));

                var deformatter = new RSAPKCS1SignatureDeformatter(x);
                deformatter.SetHashAlgorithm("SHA256");
                return deformatter.VerifySignature(hashValue, signature);
            });
        }

        private async Task GetKeys()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(KeySetUri);

            var contentStream = await response.Content.ReadAsStreamAsync();

            _keys = await JsonSerializer.DeserializeAsync<List<Models.JsonWebKey>>(contentStream);
        }
    }
}
