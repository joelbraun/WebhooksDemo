using System.IO;
using System.Threading.Tasks;

namespace WebhooksClient.Services
{
    public interface ISignedHashValidatorService
    {
        /// <summary>
        /// Returns whether the specified hash matches the provided payload
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        Task<bool> ValidateAsync(string hash, string payload);

        /// <summary>
        /// Returns whether the specified hash matches the provided payload
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<bool> ValidateAsync(string hash, Stream stream);
    }
}
