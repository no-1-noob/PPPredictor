
using Newtonsoft.Json;
using PPPredictor.Core.Interface;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.LeaderBoard.BeatLeaderDataTypes;

namespace PPPredictor.Core.API
{
    class BeatleaderAPI : IBeatLeaderAPI
    {
        private static readonly string baseUrl = "https://api.beatleader.com";
        private readonly HttpClient client;

        public BeatleaderAPI()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "PPPredictor");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<BeatLeaderEventList> GetEvents()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/events?count=10000&sortBy=date&order=desc");
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderEventList>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in beatleaderapi GetEvents: {ex.Message}");
            }
            return new BeatLeaderEventList();
        }

        public async Task<BeatLeaderSong> GetSongByHash(string hash)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/map/hash/{hash}");
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderSong>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in beatleaderapi GetSongByHash: {ex.Message}");
            }
            return new BeatLeaderSong();
        }

        public async Task<BeatLeaderPlayer> GetPlayer(long userId, long leaderboardContextId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/player/{userId}?stats=true&leaderboardContext={leaderboardContextId}");
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayer>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in beatleaderapi GetPlayer: {ex.Message}");
            }
            return new BeatLeaderPlayer();
        }

        public async Task<BeatLeaderPlayerScoreList> GetPlayerScores(string userId, string sortBy, string order, int page, int count, long leaderboardContextId, long? eventId = null)
        {
            try
            {
                string requestUrl = $"/player/{userId}/scores?sortBy={sortBy}&order={order}&page={page}&count={count}&leaderboardContext={leaderboardContextId}";
                if (eventId.GetValueOrDefault() > 0)
                {
                    requestUrl += $"&eventId={eventId}";
                }
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayerScoreList>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in beatleaderapi GetPlayerScores: {ex.Message}");
            }
            return new BeatLeaderPlayerScoreList();
        }

        public async Task<BeatLeaderPlayerList> GetPlayersInLeaderboard(string sortBy, int page, int? count, string order, long leaderboardContextId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"players?sortBy={sortBy}&page={page}&count={count}&order={order}&mapsType=ranked&friends=false&leaderboardContext={leaderboardContextId}");
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayerList>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in beatleaderapi GetPlayersInLeaderboard: {ex.Message}");
            }
            return new BeatLeaderPlayerList();
        }

        public async Task<BeatLeaderPlayerList> GetPlayersInEventLeaderboard(long eventId, string sortBy, int page, int? count, string order)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"event/{eventId}/players?sortBy={sortBy}&page={page}&count={count}&order={order}");
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayerList>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in beatleaderapi GetPlayersInLeaderboard: {ex.Message}");
            }
            return new BeatLeaderPlayerList();
        }

        public async Task<BeatLeaderPlayList> GetPlayList(long playListId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"playlist/{playListId}");
                DebugPrintBeatLeaderNetwork(response.RequestMessage.RequestUri.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayList>(result);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"Error in beatleaderapi GetPlayList: {ex.Message}");
            }
            return new BeatLeaderPlayList();
        }

        public void DebugPrintBeatLeaderNetwork(string message)
        {
            Logging.DebugNetworkPrint($"BeatLeaderNetwork: {message}", DataType.Enums.Leaderboard.BeatLeader);
        }
    }
}
