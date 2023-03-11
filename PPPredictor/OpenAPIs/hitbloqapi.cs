
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PPPredictor.OpenAPIs
{
    public class hitbloqapi
    {
        private static readonly string baseUrl = "https://hitbloq.com";
        private HttpClient client;

        public hitbloqapi()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<HitBloqUserId> GetHitBloqUserIdByUserId(string id)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/tools/ss_to_hitbloq/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqUserId>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in GetHitBloqUserIdByUserId: {ex.Message}");
            }
            return new HitBloqUserId();
        }

        public async Task<List<HitBloqMapPool>> GetHitBloqMapPools()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/map_pools_detailed");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<HitBloqMapPool>>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in GetHitBloqUserIdByUserId: {ex.Message}");
            }
            return new List<HitBloqMapPool>();
        }

        public async Task<HitBloqMapPoolDetails> GetHitBloqMapPoolDetails(string poolIdent, int page)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/ranked_list/{poolIdent}/{page}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqMapPoolDetails>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in GetHitBloqMapPoolDetails: {ex.Message}");
            }
            return new HitBloqMapPoolDetails();
        }

        public async Task<HitBloqUser> GetHitBloqUserByPool(long userId, string poolIdent)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/player_rank/{poolIdent}/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqUser>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in GetHitBloqUserIdByUserId: {ex.Message}");
            }
            return new HitBloqUser();
        }

        public async Task<List<HitBloqScores>> GetRecentScores(string userId, string poolId, int page)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/user/{userId}/scores?page={page}&pool={poolId}&sort=newest");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<HitBloqScores>>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in GetRecentScores: {ex.Message}");
            }
            return new List<HitBloqScores>();
        }

        public async Task<HitBloqLadder> GetPlayerListForMapPool(double page, string mapPoolId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/ladder/{mapPoolId}/players/{page}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqLadder>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in GetPlayerListForMapPool: {ex.Message}");
            }
            return new HitBloqLadder();
        }

        public async Task<HitBloqLeaderboardInfo> GetLeaderBoardInfo(string searchString)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"api/leaderboard/{searchString}/info");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<HitBloqLeaderboardInfo>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in GetLeaderBoardInfo: {ex.Message}");
            }
            return new HitBloqLeaderboardInfo();
        }
    }

    public class HitBloqUserId
    {
        public long id { get; set; }
    }

    public class HitBloqMapPool
    {
        public string id { get; set; }
        public string title { get; set; }
        public string short_description { get; set; }
        public string image { get; set; }
    }

    public class HitBloqMapPoolDetails
    {
        public float accumulation_constant { get; set; }
        public CrCurve cr_curve { get; set; }
        public List<string> leaderboard_id_list { get; set; }
        public string long_description { get; set; }
        public int priority { get; set; }
    }

    public class CrCurve
    {
        public List<float[]> points { get; set; }
        public string type { get; set; }
        [DefaultValue(null)]
        public double? baseline { get; set; }
        [DefaultValue(null)]
        public double? exponential { get; set; }
        [DefaultValue(null)]
        public double? cutoff { get; set; }
    }

    public class HitBloqUser
    {
        public float cr { get; set; }
        public int rank { get; set; }
        public string username { get; set; }
    }

    public class HitBloqScores
    {
        public float cr_received { get; set; }
        public string song_id { get; set; }
        public long time { get; set; }
    }

    public class HitBloqLadder
    {
        public List<HitBloqUser> ladder { get; set; }
    }

    public class HitBloqLeaderboardInfo
    {
        public Dictionary<string, double> star_rating { get; set; }
    }
}
