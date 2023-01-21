using beatleaderapi;
using PPPredictor.Utilities;
using scoresaberapi;
using System;

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
            this.timeSet = playerScore.Score.TimeSet;
            this.pp = playerScore.Score.Pp;
            this.songHash = playerScore.Leaderboard.SongHash;
            this.difficulty = playerScore.Leaderboard.Difficulty.Difficulty1;
            this.gameMode = playerScore.Leaderboard.Difficulty.GameMode;
        }

        public PPPScore(ScoreResponseWithMyScore playerScore)
        {
            if(long.TryParse(playerScore.Timeset, out long timeSetLong)){
                this.timeSet = new DateTime(1970, 1, 1).AddSeconds(timeSetLong);
            }
            this.pp = (int)playerScore.Leaderboard.Difficulty.Status == (int)BeatLeaderDifficultyStatus.ranked ? playerScore.Pp : 0;
            this.songHash = playerScore.Leaderboard.Song.Hash;
            this.difficulty = playerScore.Leaderboard.Difficulty.Value;
            this.gameMode = "Solo"+playerScore.Leaderboard.Difficulty.ModeName;
        }
    }
}
