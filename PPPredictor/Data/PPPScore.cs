using beatleaderapi;
using scoresaberapi;
using System;

namespace PPPredictor.Data
{
    public class PPPScore
    {
        DateTimeOffset timeSet;
        double pp;
        string songHash;
        double difficulty;

        public DateTimeOffset TimeSet { get => timeSet; }
        public double Pp { get => pp; }
        public string SongHash { get => songHash; }
        public double Difficulty1 { get => difficulty; }

        public PPPScore(PlayerScore playerScore)
        {
            this.timeSet = playerScore.Score.TimeSet;
            this.pp = playerScore.Score.Pp;
            this.songHash = playerScore.Leaderboard.SongHash;
            this.difficulty = playerScore.Leaderboard.Difficulty.Difficulty1;
        }

        public PPPScore(ScoreResponseWithMyScore playerScore)
        {
            if(long.TryParse(playerScore.Timeset, out long timeSetLong)){

                this.timeSet = new DateTimeOffset(timeSetLong, new TimeSpan());
            }
            this.pp = playerScore.Leaderboard.Difficulty.Ranked ? playerScore.Pp : 0;
            this.songHash = playerScore.Leaderboard.Song.Hash;
            this.difficulty = playerScore.Leaderboard.Difficulty.Value;
        }
    }
}
