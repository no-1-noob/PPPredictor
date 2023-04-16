using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PPPredictor.OpenAPIs
{
    internal class ScoresaberAPI
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

#pragma warning disable IDE1006 // Naming Styles; api dictates them...
        public class ScoreSaberPlayerList
        {
            public List<ScoreSaberPlayer> players { get; set; }

            public ScoreSaberPlayerList()
            {
                players = new List<ScoreSaberPlayer>();
            }
        }

        public class ScoreSaberPlayer
        {
            public string country { get; set; }
            public double pp { get; set; }
            public double rank { get; set; }
            public double countryRank { get; set; }
        }

        public class ScoreSaberPlayerScoreList
        {
            public List<ScoreSaberPlayerScore> playerScores { get; set; }
            public ScoreSaberScoreListMetadata metadata { get; set; }
        }

        public class ScoreSaberScoreListMetadata
        {
            public double total { get; set; }
            public double page { get; set; }
            public double itemsPerPage { get; set; }
        }

        public class ScoreSaberPlayerScore
        {
            public ScoreSaberScore score { get; set; }
            public ScoreSaberLeaderboardInfo leaderboard { get; set; }
        }

        public class ScoreSaberLeaderboardInfo
        {
            public string songHash { get; set; }
            public ScoreSaberDifficulty difficulty { get; set; }
        }

        public class ScoreSaberDifficulty
        {
            public double difficulty { get; set; }
            public string gameMode { get; set; }
        }

        public class ScoreSaberScore
        {
            public double pp { get; set; }
            public System.DateTimeOffset timeSet { get; set; }
        }
#pragma warning restore IDE1006 // Naming Styles; api dictates them...
    }
}
