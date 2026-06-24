using System.Threading.Tasks;
using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.Core.Interfaces
{
    /// <summary>
    /// Tracks API usage per subscriber and enforces subscription quota limits.
    /// </summary>
    public interface IUsageTrackingService
    {
        /// <summary>
        /// Records a request and checks whether it falls within the API key's quota.
        /// </summary>
        /// <param name="apiKey">The RapidAPI key identifying the subscriber.</param>
        /// <param name="tier">The subscriber's current plan (Basic, Pro, Ultra, Mega).</param>
        /// <param name="endpoint">The endpoint path being accessed.</param>
        /// <returns>True if within quota; false if quota exceeded.</returns>
        Task<bool> TrackUsageAsync(string apiKey, string tier, string endpoint);

        /// <summary>
        /// Returns current usage statistics for a given API key.
        /// </summary>
        /// <param name="apiKey">The RapidAPI key to look up.</param>
        /// <returns>Usage statistics, or null if the key has made no requests yet.</returns>
        Task<UsageStatistics> GetUsageStatisticsAsync(string apiKey);
    }
}
