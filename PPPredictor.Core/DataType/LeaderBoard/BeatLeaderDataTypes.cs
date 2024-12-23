using System;
using System.Collections.Generic;

namespace PPPredictor.Core.DataType.LeaderBoard
{
    internal class BeatLeaderDataTypes
    {
#pragma warning disable IDE1006 // Naming Styles; api dictates them...
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
            public DateTime dtEndDate
            {
                get
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
            public double? stars { get; set; }
            public double? predictedAcc { get; set; }
            public double? passRating { get; set; }
            public double? accRating { get; set; }
            public double? techRating { get; set; }
            public int status { get; set; }
            public string modeName { get; set; }
            public Dictionary<string, double> modifierValues { get; set; }
            public Dictionary<string, double> modifiersRating { get; set; }
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
            public float pp { get; set; }
            public BeatLeaderLeaderboard leaderboard { get; set; } = new BeatLeaderLeaderboard();
        }

        public class BeatLeaderPlayerScoreListMetaData
        {
            public int itemsPerPage { get; set; }
            public int page { get; set; }
            public int total { get; set; }
        }

        public class BeatLeaderLeaderboard
        {
            public string hash { get; set; }
            public BeatLeaderDifficulty difficulty { get; set; } = new BeatLeaderDifficulty();
            public BeatLeaderSong song { get; set; } = new BeatLeaderSong();
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
            public Enums.BeatMapDifficulty name { get; set; }
            public string characteristic { get; set; }
        }
    }
#pragma warning restore IDE1006 // Naming Styles; api dictates them...
}
