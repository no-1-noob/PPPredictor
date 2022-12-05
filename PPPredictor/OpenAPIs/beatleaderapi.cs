
using Newtonsoft.Json;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PPPredictor.OpenAPIs
{
    public class beatleaderapi
    {
        private static readonly string baseUrl = "https://api.beatleader.xyz";
        private HttpClient client;

        public beatleaderapi()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<Dictionary<string, float>> GetModifiers()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"modifiers");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Dictionary<string, float>>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in beatleaderapi GetModifiers: {ex.Message}");
            }
            return new Dictionary<string, float>();
        }

        public async Task<BeatLeaderEventList> GetEvents()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/events?count=10000&sortBy=date&order=desc");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderEventList>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in beatleaderapi GetEvents: {ex.Message}");
            }
            return new BeatLeaderEventList();
        }

        public async Task<BeatLeaderSong> GetSongByHash(string hash)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/map/hash/{hash}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderSong>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in beatleaderapi GetSongByHash: {ex.Message}");
            }
            return new BeatLeaderSong();
        }

        public async Task<BeatLeaderPlayer> GetPlayer(long userId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/player/{userId}?stats=true");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayer>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in beatleaderapi GetPlayer: {ex.Message}");
            }
            return new BeatLeaderPlayer();
        }

        public async Task<BeatLeaderPlayerScoreList> GetPlayerScores(string userId, string sortBy, string order, int page, int count, long? eventId = null)
        {
            try
            {
                string requestUrl = $"/player/{userId}/scores?sortBy={sortBy}&order={order}&page={page}&count={count}'";
                if (eventId.GetValueOrDefault() > 0)
                {
                    requestUrl += $"&eventId={eventId}";
                }
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayerScoreList>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in beatleaderapi GetPlayerScores: {ex.Message}");
            }
            return new BeatLeaderPlayerScoreList();
        }

        public async Task<BeatLeaderPlayerList> GetPlayersInLeaderboard(string sortBy, int page, int? count, string order)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"players?sortBy={sortBy}&page={page}&count={count}&order={order}&mapsType=ranked&friends=false");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayerList>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in beatleaderapi GetPlayersInLeaderboard: {ex.Message}");
            }
            return new BeatLeaderPlayerList();
        }

        public async Task<BeatLeaderPlayerList> GetPlayersInEventLeaderboard(long eventId, string sortBy, int page, int? count, string order)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"event/{eventId}/players?sortBy={sortBy}&page={page}&count={count}&order={order}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayerList>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in beatleaderapi GetPlayersInLeaderboard: {ex.Message}");
            }
            return new BeatLeaderPlayerList();
        }

        public async Task<BeatLeaderPlayList> GetPlayList(long playListId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"playlist/{playListId}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BeatLeaderPlayList>(result);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error in beatleaderapi GetPlayList: {ex.Message}");
            }
            return new BeatLeaderPlayList();
        }

        public class BeatLeaderEventList
        {
            public List<BeatLeaderEvent> data { get; set; }
            public BeatLeaderEventList()
            {
                data = new List<BeatLeaderEvent>();
            }
        }

        public class BeatLeaderEvent
        {
            public int id { get; set; }
            public string name { get; set; }
            public long endDate { get; set; }
            public DateTime dtEndDate { get
                {
                    if (long.TryParse(endDate.ToString(), out long timeSetLong))
                    {
                        return new DateTime(1970, 1, 1).AddSeconds(timeSetLong);
                    }
                    return new DateTime(1970, 1, 1);
                } 
            }
            public long playListId { get; set; }
        }

        public class BeatLeaderSong
        {
            public List<BeatLeaderDifficulty> difficulties { get; set; }
            public string hash { get; set; }
            public BeatLeaderSong()
            {
                difficulties = new List<BeatLeaderDifficulty>();
            }
        }

        public class BeatLeaderDifficulty
        {
            public int value { get; set; }
            public float? stars { get; set; }
            public int status { get; set; }
        }

        public class BeatLeaderPlayerList
        {
            public List<BeatLeaderPlayer> data;
            public BeatLeaderPlayerList()
            {
                data = new List<BeatLeaderPlayer>();
            }
        }

        public class BeatLeaderPlayer
        {
            public string name { get; set; }
            public string country { get; set; }
            public float rank { get; set; }
            public float countryRank { get; set; }
            public float pp { get; set; }
            public List<BeatLeaderPlayerEvents> eventsParticipating { get; set; }
        }

        public class BeatLeaderPlayerEvents
        {
            public long id { get; set; }
            public long eventId { get; set; }
            public string name { get; set; }
            public string country { get; set; }
            public float rank { get; set; }
            public float countryRank { get; set; }
            public float pp { get; set; }
        }

        public class BeatLeaderPlayerScoreList
        {
            public List<BeatLeaderPlayerScore> data { get; set; }
            public BeatLeaderPlayerScoreListMetaData metadata { get; set; }
            public BeatLeaderPlayerScoreList()
            {
                metadata = new BeatLeaderPlayerScoreListMetaData();
                data = new List<BeatLeaderPlayerScore>();
            }
        }

        public class BeatLeaderPlayerScore
        {
            public string timeset { get; set; }
            public float pp {get; set;}
            public BeatLeaderLeaderboard leaderboard { get; set; }
        }

        public class BeatLeaderPlayerScoreListMetaData
        {
            public int itemsPerPage { get; set; }
            public int page { get; set; }
            public int total { get; set; }
        }

        public class BeatLeaderLeaderboard {
            public string hash { get; set; }
            public BeatLeaderDifficulty difficulty { get; set; }
            public BeatLeaderSong song { get; set; }
        }

        public class BeatLeaderPlayList
        {
            public List<BeatLeaderPlayListSong> songs { get; set; }
            public BeatLeaderPlayList()
            {
                songs = new List<BeatLeaderPlayListSong>();
            }
            
        }

        public class BeatLeaderPlayListSong
        {
            public string hash { get; set; }
            public List<BeatLeaderPlayListDifficulties> difficulties { get; set; }
            public BeatLeaderPlayListSong()
            {
                difficulties = new List<BeatLeaderPlayListDifficulties>();
            }
        }


        public class BeatLeaderPlayListDifficulties
        {
            public PPPBeatMapDifficulty name { get; set; }
            public string characteristic { get; set; }
        }
    }   
}
