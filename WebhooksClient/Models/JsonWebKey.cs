using System;
namespace WebhooksClient.Models
{
    /// <summary>
    /// Properties for a JSON web key.
    /// Borrowed from https://github.com/IdentityServer/IdentityServer4/blob/main/src/IdentityServer4/src/Models/JsonWebKey.cs
    /// </summary>
    public class JsonWebKey
    {
        public string kty { get; set; }
        public string use { get; set; }
        public string kid { get; set; }
        public string x5t { get; set; }
        public string e { get; set; }
        public string n { get; set; }
        public string[] x5c { get; set; }
        public string alg { get; set; }
    }
}
