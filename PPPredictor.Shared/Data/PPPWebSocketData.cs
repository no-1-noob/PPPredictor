using PPPredictor.Core;
using PPPredictor.Core.DataType;
using PPPredictor.Shared.Interfaces;
using System;
using System.Collections.Generic;

namespace PPPredictor.Shared.Data
{
    #region scoresaber
    class PPPWsScoreSaberCommand : IPPPRawWebsocketData
    {
        public PPPWsScoreSaberData commandData { get; set; }

        public PPPScoreSetData ConvertToPPPWebSocketData(string leaderboardName)
        {
            var data = new PPPScoreSetData();
            data.leaderboardName = leaderboardName;
            data.userId = commandData.score.leaderboardPlayerInfo.id;
            data.hash = $"{commandData.leaderboard.songHash}_{commandData.leaderboard.difficulty.gameMode}_{commandData.leaderboard.difficulty.difficulty}".ToUpper();
            return data;
        }
    }

    class PPPWsScoreSaberData
    {
        public PPPWsScoreSaberScore score { get; set; }
        public PPPWsScoreSaberLeaderBoard leaderboard { get; set; }
    }

    class PPPWsScoreSaberLeaderBoard
    {
        public string songHash { get; set; }
        public PPPWsScoreSaberDifficulty difficulty { get; set; }
    }

    class PPPWsScoreSaberDifficulty
    {
        public int difficulty { get; set; }
        public string gameMode { get; set; }
    }

    class PPPWsScoreSaberScore
    {
        public double pp { get; set; }
        public WebSocketScoreCommandPlayerInfo leaderboardPlayerInfo { get; set; }
    }

    class WebSocketScoreCommandPlayerInfo
    {
        public string id { get; set; }
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

        public PPPScoreSetData ConvertToPPPWebSocketData(string leaderboardName)
        {
            var data = new PPPScoreSetData();
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


        public PPPWsBeatLeaderDifficulty difficulty { get; set; }
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

    #region AccSaberReloaded
    class PPPWsAccSaberReloadedData : IPPPRawWebsocketData
    {
        public string userId { get; set; }
        public string songHash { get; set; }
        public string difficulty { get; set; }

        public PPPScoreSetData ConvertToPPPWebSocketData(string leaderboardName)
        {
            var data = new PPPScoreSetData();
            data.leaderboardName = leaderboardName;
            data.userId = userId;
            data.hash = $"{songHash}_SoloStandard_{ParsingUtil.ParseDifficultyNameToInt(difficulty)}".ToUpper();
            return data;
        }
    }
    #endregion
}
