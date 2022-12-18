using PPPredictor.OpenAPIs;
using PPPredictor.Utilities;
using scoresaberapi;
using System;
using System.Linq;
using static PPPredictor.OpenAPIs.beatleaderapi;

namespace PPPredictor.Data
{
    public class PPPScore
    {
        readonly DateTimeOffset timeSet;
        readonly double pp;
        readonly string songHash;
        readonly double difficulty;

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

        public PPPScore(BeatLeaderPlayerScore playerScore)
        {
            if(long.TryParse(playerScore.timeset, out long timeSetLong)){
                this.timeSet = new DateTime(1970, 1, 1).AddSeconds(timeSetLong);
            }
            this.pp = (int)playerScore.leaderboard.difficulty.status == (int)BeatLeaderDifficultyStatus.ranked ? playerScore.pp : 0;
            this.songHash = playerScore.leaderboard.song.hash;
            this.difficulty = playerScore.leaderboard.difficulty.value;
        }

        public PPPScore(HitBloqScores playerScore)
        {
            this.timeSet = new DateTime(1970, 1, 1).AddSeconds(playerScore.time);
            this.pp = playerScore.cr_received;
            this.songHash = playerScore.song_id;
            this.difficulty = -1;
        }

        public string GetSearchString()
        {
            if(difficulty >= 0)
            {
                return $"{SongHash}_{Difficulty1}";
            }
            return SongHash;
        }
    }
}
