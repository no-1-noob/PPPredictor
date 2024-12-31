namespace PPPredictor.Core.DataType
{
    public class Enums
    {
        public enum BeatMapDifficulty
        {
            Easy = 1,
            Normal = 3,
            Hard = 5,
            Expert = 7,
            ExpertPlus = 9
        }

        internal enum BeatLeaderDifficultyStatus
        {
            unranked = 0,
            nominated = 1,
            qualified = 2,
            ranked = 3,
            unrankable = 4,
            outdated = 5,
            inevent = 6
        }

        public enum LeaderboardContext
        {
            None,
            BeatLeaderDefault,
            BeatLeaderNoModifiers,
            BeatLeaderNoPauses,
            BeatLeaderGolf,
            BeatLeaderSCPM
        }

        public enum MapPoolType
        {
            None,
            Default,
            Custom
        }

        public enum CurveType
        {
            ScoreSaber,
            BeatLeader,
            Linear,
            Basic,
            Dummy,
            AccSaber
        }

        public enum PPGainCalculationType
        {
            Weighted,
            Raw
        }

        public enum MapPoolSorting
        {
            Alphabetical,
            Popularity
        }

        public enum Leaderboard
        {
            ScoreSaber,
            BeatLeader,
            AccSaber,
            NoLeaderboard,
            HitBloq
        }
    }
}
