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
        Task<bool> Validate(string hash, string payload);
    }
}
