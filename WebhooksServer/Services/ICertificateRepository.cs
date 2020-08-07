using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using WebhooksServer.Models;

namespace WebhooksServer.Services
{
    public interface ICertificateRepository
    {
        /// <summary>
        /// Returns the public keys associated with the stored signing certificate(s)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JsonWebKey> GetPublicKeys();

        /// <summary>
        /// Returns the primary signing certificate in the store
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 GetPrimarySigningCertificate();
    }
}
