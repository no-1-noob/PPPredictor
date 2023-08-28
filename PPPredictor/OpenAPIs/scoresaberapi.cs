using Newtonsoft.Json;
using PPPredictor.Interfaces;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static PPPredictor.Data.LeaderBoardDataTypes.ScoreSaberDataTypes;

namespace PPPredictor.OpenAPIs
{
    internal class ScoresaberAPI : IScoresaberAPI
    {
        private static readonly string baseUrl = "https://scoresaber.com/api/";
        private readonly HttpClient client;

        public ScoresaberAPI()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "PPPredictor");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseUrl);
        }

        [Conditional("SCORESABERNETWORK")]
        public void DebugPrintBeatLeaderNetwork(string message)
        {
            Plugin.DebugNetworkPrint($"ScoreSaberNetwork: {message}");
        }

        public async Task<ScoreSaberPlayerList> GetPlayers(double? page)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"players?&page={page}&withMetadata=true");
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ScoreSaberPlayerList>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in scoresaberapi GetPlayers: {ex.Message}");
            }
            return new ScoreSaberPlayerList();
        }

        public async Task<ScoreSaberPlayer> GetPlayer(long playerId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"player/{playerId}/basic");
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ScoreSaberPlayer>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in scoresaberapi GetPlayer: {ex.Message}");
            }
            return new ScoreSaberPlayer();
        }

        public async Task<ScoreSaberPlayerScoreList> GetPlayerScores(string playerId, int? limit, int? page)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"player/{playerId}/scores?limit={limit}&sort=recent&page={page}&withMetadata=true");
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ScoreSaberPlayerScoreList>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in scoresaberapi GetPlayerScores: {ex.Message}");
            }
            return new ScoreSaberPlayerScoreList();
        }
    }
}
