using PPPredictor.Core.Interface;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using static PPPredictor.Core.DataType.LeaderBoard.AccSaberDataTypes;

namespace PPPredictor.Core.API
{
    internal class AccSaberApi : IAccSaberAPI
    {
        private static readonly string baseUrl = "http://api.accsaber.com";
        private readonly HttpClient client;

        public AccSaberApi()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "PPPredictor");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseUrl);
        }

        public void DebugPrintAccSaberNetwork(string message)
        {
            Logging.DebugNetworkPrint($"AccSaberNetwork: {message}", DataType.Enums.Leaderboard.AccSaber);
        }

        public async Task<List<AccSaberScores>> GetAllScores(string userId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/players/{userId}/scores?pageSize=9999");
                DebugPrintAccSaberNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<AccSaberScores>>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetAllScores: {ex.Message}");
            }
            return new List<AccSaberScores>();
        }

        public async Task<List<AccSaberScores>> GetAllScoresByPool(string userId, string poolId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/players/{userId}/{poolId}/scores");
                DebugPrintAccSaberNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<AccSaberScores>>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetAllScores: {ex.Message}");
            }
            return new List<AccSaberScores>();
        }

        public async Task<AccSaberPlayer> GetAccSaberUserByPool(long userId, string poolIdent)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"players/{userId}/{poolIdent}");
                DebugPrintAccSaberNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<AccSaberPlayer>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetHitBloqUserIdByUserId: {ex.Message}");
            }
            return new AccSaberPlayer();
        }

        public async Task<List<AccSaberMapPool>> GetAccSaberMapPools()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"categories");
                DebugPrintAccSaberNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<AccSaberMapPool>>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetAccSaberMapPools: {ex.Message}");
            }
            return new List<AccSaberMapPool>();
        }

        public async Task<List<AccSaberRankedMap>> GetAllRankedMaps()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"ranked-maps");
                DebugPrintAccSaberNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<AccSaberRankedMap>>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetAllRankedMaps: {ex.Message}");
            }
            return new List<AccSaberRankedMap>();
        }

        public async Task<List<AccSaberRankedMap>> GetRankedMaps(string mapPool)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"ranked-maps/category/{mapPool}");
                DebugPrintAccSaberNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<AccSaberRankedMap>>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetRankedMaps: {ex.Message}");
            }
            return new List<AccSaberRankedMap>();
        }

        public async Task<List<AccSaberPlayer>> GetPlayerListForMapPool(double page, string mapPoolId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"categories/{mapPoolId}/standings");
                DebugPrintAccSaberNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<AccSaberPlayer>>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in GetPlayerListForMapPool: {ex.Message}");
            }
            return new List<AccSaberPlayer>();
        }
    }
}
