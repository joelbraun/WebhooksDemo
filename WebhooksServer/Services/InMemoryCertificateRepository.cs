using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace WebhooksServer.Services
{
    public class InMemoryCertificateRepository : ICertificateRepository
    {
        private readonly List<X509Certificate2> _signingCertificates;

        public InMemoryCertificateRepository()
        {
            const string CertificatePath = "certificate.pfx";
            const string CertificatePassword = "dCxnf3k0sFKf";

            _signingCertificates = new List<X509Certificate2>
            {
                new X509Certificate2(CertificatePath, CertificatePassword)
            };
        }

        /// <inheritdoc cref="ICertificateRepository.GetPrimarySigningCertificate"/>
        public X509Certificate2 GetPrimarySigningCertificate()
        {
            return _signingCertificates.First();
        }

        /// <inheritdoc cref="ICertificateRepository.GetPublicKeys"/>
        public IEnumerable<Models.JsonWebKey> GetPublicKeys()
        {
            var securityKeys = _signingCertificates.Select(x => new X509SecurityKey(x));

            // Turn the certificate into a serializiable model. This has been borrowed from
            // https://github.com/IdentityServer/IdentityServer4/blob/main/src/IdentityServer4/src/ResponseHandling/Default/DiscoveryResponseGenerator.cs
            // which uses a similar approach to serialize its keys.
            return securityKeys.Select(x509Key =>
            {
                var cert64 = Convert.ToBase64String(x509Key.Certificate.RawData);
                var thumbprint = Base64UrlEncoder.Encode(x509Key.Certificate.GetCertHash());

                using var rsa = x509Key.PublicKey as RSA;
                var parameters = rsa.ExportParameters(false);
                var exponent = Base64UrlEncoder.Encode(parameters.Exponent);
                var modulus = Base64UrlEncoder.Encode(parameters.Modulus);

                return new Models.JsonWebKey
                {
                    kty = "RSA",
                    use = "sig",
                    kid = x509Key.KeyId,
                    x5t = thumbprint,
                    e = exponent,
                    n = modulus,
                    x5c = new[] { cert64 },
                    alg = rsa.SignatureAlgorithm
                };
            });
        }
    }
}
