
using Newtonsoft.Json;
using PPPredictor.Core.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.LeaderBoard.HitBloqDataTypes;

namespace PPPredictor.Core.API
{
    class HitbloqAPI : IHitBloqAPI
    {
        private static readonly string baseUrl = "https://hitbloq.com";
        private readonly HttpClient client;

        public HitbloqAPI()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "PPPredictor");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<HitBloqUserId> GetHitBloqUserIdByUserId(string id)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/tools/ss_to_hitbloq/{id}");
                DebugPrintHitbloqNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqUserId>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetHitBloqUserIdByUserId: {ex.Message}");
            }
            return new HitBloqUserId();
        }

        public async Task<List<HitBloqMapPool>> GetHitBloqMapPools()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/map_pools_detailed");
                DebugPrintHitbloqNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<HitBloqMapPool>>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetHitBloqUserIdByUserId: {ex.Message}");
            }
            return new List<HitBloqMapPool>();
        }

        public async Task<HitBloqMapPoolDetails> GetHitBloqMapPoolDetails(string poolIdent, int page)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/ranked_list/{poolIdent}/{page}");
                DebugPrintHitbloqNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqMapPoolDetails>(result);
                }
                }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetHitBloqMapPoolDetails: {ex.Message}");
            }
            return new HitBloqMapPoolDetails();
        }

        public async Task<HitBloqUser> GetHitBloqUserByPool(long userId, string poolIdent)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/player_rank/{poolIdent}/{userId}");
                DebugPrintHitbloqNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqUser>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetHitBloqUserIdByUserId: {ex.Message}");
            }
            return new HitBloqUser();
        }

        public async Task<List<HitBloqScores>> GetRecentScores(string userId, string poolId, int page)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/user/{userId}/scores?page={page}&pool={poolId}&sort=newest");
                DebugPrintHitbloqNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<HitBloqScores>>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetRecentScores: {ex.Message}");
            }
            return new List<HitBloqScores>();
        }

        public async Task<List<HitBloqScores>> GetAllScores(string userId, string poolId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/user/{userId}/all_scores?pool={poolId}");
                DebugPrintHitbloqNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<HitBloqScores>>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetAllScores: {ex.Message}");
            }
            return new List<HitBloqScores>();
        }

        public async Task<HitBloqLadder> GetPlayerListForMapPool(double page, string mapPoolId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/ladder/{mapPoolId}/players/{page}");
                DebugPrintHitbloqNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqLadder>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetPlayerListForMapPool: {ex.Message}");
            }
            return new HitBloqLadder();
        }

        public async Task<HitBloqRankFromCr> GetPlayerRankByCr(string mapPoolId, double cr)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/ladder/{mapPoolId}/cr_to_rank/{cr.ToString(new CultureInfo("en-US"))}");
                DebugPrintHitbloqNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqRankFromCr>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetPlayerRankByCr: {ex.Message}");
            }
            return new HitBloqRankFromCr();
        }

        public async Task<HitBloqLeaderboardInfo> GetLeaderBoardInfo(string searchString)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/leaderboard/{searchString}/info");
                DebugPrintHitbloqNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqLeaderboardInfo>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetLeaderBoardInfo: {ex.Message}");
            }
            return new HitBloqLeaderboardInfo();
        }

        public void DebugPrintHitbloqNetwork(string message)
        {
            Logging.DebugNetworkPrint($"HitbloqNetwork: {message}", DataType.Enums.Leaderboard.HitBloq);
        }
    }
}
