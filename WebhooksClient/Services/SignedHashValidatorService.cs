using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace WebhooksClient.Services
{
    public class SignedHashValidatorService : ISignedHashValidatorService, IDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private List<RSA> _keys;

        private static string KeySetUri = "https://localhost:3023/keys";

        public SignedHashValidatorService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> ValidateAsync(string hash, Stream inputStream)
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

            var bodyData = new Memory<byte>();
            try 
            {
                await inputStream.ReadAsync(bodyData);
            }
            catch (Exception) 
            {
                return false;
            }

            return _keys.Any(x =>
            {
                using var hasher = SHA256.Create();
                var hashValue = hasher.ComputeHash(bodyData.ToArray());

                var deformatter = new RSAPKCS1SignatureDeformatter(x);
                deformatter.SetHashAlgorithm("SHA256");
                return deformatter.VerifySignature(hashValue, signature);
            });
        }

        public async Task<bool> ValidateAsync(string hash, string payload)
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

            return _keys.Any(x =>
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
            var data = await JsonSerializer.DeserializeAsync<List<Models.JsonWebKey>>(contentStream);

            _keys = data.Select(x => RSA.Create(new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(x.n),
                Exponent = Base64UrlEncoder.DecodeBytes(x.e)
            })).ToList();
        }

        public void Dispose()
        {
            _keys.ForEach(x => x.Dispose());
        }
    }
}
