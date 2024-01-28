using PPPredictor.Interfaces;

namespace PPPredictor.Data
{
    internal class PPPWebSocketData
    {
        public string leaderboardName;
        public string userId;
        public int context;
        public string hash;
    }

    #region scoresaber
    class PPPWsScoreSaberCommand : IPPPRawWebsocketData
    {
        public PPPWsScoreSaberData commandData;

        public PPPWebSocketData ConvertToPPPWebSocketData(string leaderboardName)
        {
            var data = new PPPWebSocketData();
            data.leaderboardName = leaderboardName;
            data.userId = commandData.score.leaderboardPlayerInfo.id;
            data.hash = $"{commandData.leaderboard.songHash}_{commandData.leaderboard.difficulty.gameMode}_{commandData.leaderboard.difficulty.difficulty}".ToUpper();
            return data;
        }
    }

    class PPPWsScoreSaberData
    {
        public PPPWsScoreSaberScore score;
        public PPPWsScoreSaberLeaderBoard leaderboard;
    }

    class PPPWsScoreSaberLeaderBoard
    {
        public string songHash;
        public PPPWsScoreSaberDifficulty difficulty;
    }

    class PPPWsScoreSaberDifficulty
    {
        public int difficulty;
        public string gameMode;
        public string difficultyRaw;
    }

    class PPPWsScoreSaberScore
    {
        public double pp;
        public WebSocketScoreCommandPlayerInfo leaderboardPlayerInfo;
    }

    class WebSocketScoreCommandPlayerInfo
    {
        public string id;
    }
    #endregion

    #region beatleader
    class PPPWsBeatLeaderData : IPPPRawWebsocketData
    {
        public int validContexts { get; set; }
        public string playerId { get; set; }
        public double pp { get; set; }
        public PPPWsBeatLeaderPlayer player { get; set; }

        public PPPWsBeatLeaderLeaderboard leaderboard { get; set; }

        public PPPWebSocketData ConvertToPPPWebSocketData(string leaderboardName)
        {
            var data = new PPPWebSocketData();
            data.leaderboardName = leaderboardName;
            data.context = validContexts;
            data.userId = playerId;
            data.hash = $"{leaderboard.song.hash}_SOLO{leaderboard.difficulty.modeName}_{leaderboard.difficulty.value}".ToUpper();
            return data;
        }
    }

    class PPPWsBeatLeaderPlayer
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    class PPPWsBeatLeaderLeaderboard
    {
        public PPPWsBeatLeaderSong song { get; set; }


        public PPPWsBeatLeaderDifficulty difficulty;
    }

    class PPPWsBeatLeaderSong
    {
        public string hash { get; set; }
        public string name { get; set; }
    }

    class PPPWsBeatLeaderDifficulty
    {
        public int value { get; set; }
        public int mode { get; set; }
        public string difficultyName { get; set; }

        public string modeName { get; set; }
    }
    #endregion
}
