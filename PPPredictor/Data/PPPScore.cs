using PPPredictor.OpenAPIs;
using PPPredictor.Utilities;
using scoresaberapi;
using System;
using System.Linq;
using static PPPredictor.OpenAPIs.BeatleaderAPI;

namespace PPPredictor.Data
{
    public class PPPScore
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

        public PPPScore(PlayerScore playerScore)
        {
            timeSet = playerScore.Score.TimeSet;
            pp = playerScore.Score.Pp;
            songHash = playerScore.Leaderboard.SongHash;
            difficulty = playerScore.Leaderboard.Difficulty.Difficulty1;
            gameMode = playerScore.Leaderboard.Difficulty.GameMode;
        }

        public PPPScore(BeatLeaderPlayerScore playerScore)
        {
            if(long.TryParse(playerScore.timeset, out long timeSetLong)){
                timeSet = new DateTime(1970, 1, 1).AddSeconds(timeSetLong);
            }
            pp = (int)playerScore.leaderboard.difficulty.status == (int)BeatLeaderDifficultyStatus.ranked ? playerScore.pp : 0;
            songHash = playerScore.leaderboard.song.hash;
            difficulty = playerScore.leaderboard.difficulty.value;
            gameMode = "Solo" + playerScore.leaderboard.difficulty.modeName;
        }

        public PPPScore(HitBloqScores playerScore)
        {
            timeSet = new DateTime(1970, 1, 1).AddSeconds(playerScore.time);
            pp = playerScore.cr_received;
            var (hash, diff, mode) = PPCalculatorHitBloq.ParseHashDiffAndMode(playerScore.song_id);
            songHash = hash;
            difficulty = ParsingUtil.ParseDifficultyNameToInt(diff);
            gameMode = mode;
        }
    }
}
