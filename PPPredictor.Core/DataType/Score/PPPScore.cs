using PPPredictor.Core.API;
using PPPredictor.Core.Calculator;
using System;
using static PPPredictor.Core.DataType.Enums;
using static PPPredictor.Core.DataType.LeaderBoard.AccSaberDataTypes;
using static PPPredictor.Core.DataType.LeaderBoard.BeatLeaderDataTypes;
using static PPPredictor.Core.DataType.LeaderBoard.HitBloqDataTypes;
using static PPPredictor.Core.DataType.LeaderBoard.ScoreSaberDataTypes;

namespace PPPredictor.Core.DataType.Score
{
    class PPPScore
    {
        readonly DateTimeOffset timeSet;
        readonly double pp;
        readonly string songHash;
        readonly double difficulty;
        readonly string gameMode;

        public DateTimeOffset TimeSet { get => timeSet; }
        public double Pp { get => pp; }
        public string SongHash { get => songHash; }
        public double Difficulty1 { get => difficulty; }
        public string GameMode { get => gameMode; }

        public PPPScore(ScoreSaberPlayerScore playerScore)
        {
            timeSet = playerScore.score.timeSet;
            pp = playerScore.score.pp;
            songHash = playerScore.leaderboard.songHash;
            difficulty = playerScore.leaderboard.difficulty.difficulty;
            gameMode = playerScore.leaderboard.difficulty.gameMode;
        }

        public PPPScore(BeatLeaderPlayerScore playerScore)
        {
            if (long.TryParse(playerScore.timeset, out long timeSetLong))
            {
                timeSet = new DateTime(1970, 1, 1).AddSeconds(timeSetLong);
            }
            pp = playerScore.leaderboard.difficulty.status == (int)BeatLeaderDifficultyStatus.ranked ? playerScore.pp : 0;
            songHash = playerScore.leaderboard.song.hash;
            difficulty = playerScore.leaderboard.difficulty.value;
            gameMode = "Solo" + playerScore.leaderboard.difficulty.modeName;
        }

        public PPPScore(HitBloqScores playerScore)
        {
            timeSet = new DateTime(1970, 1, 1).AddSeconds(playerScore.time);
            pp = playerScore.cr_received;
            var (hash, diff, mode) = PPCalculatorHitBloq<HitbloqAPI>.ParseHashDiffAndMode(playerScore.song_id);
            songHash = hash;
            difficulty = ParsingUtil.ParseDifficultyNameToInt(diff);
            gameMode = mode;
        }

        public PPPScore(AccSaberScores playerScore)
        {
            timeSet = playerScore.timeSet;
            pp = playerScore.ap;
            songHash = playerScore.songHash?.ToUpper();
            difficulty = ParsingUtil.ParseDifficultyNameToInt(playerScore.difficulty);
            gameMode = "SoloStandard";
        }
    }
}
