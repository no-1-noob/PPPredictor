namespace PPPredictor.Core.DataType
{
    internal class Enums
    {
        internal enum BeatMapDifficulty
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

        internal enum LeaderboardContext
        {
            None,
            BeatLeaderDefault,
            BeatLeaderNoModifiers,
            BeatLeaderNoPauses,
            BeatLeaderGolf,
            BeatLeaderSCPM
        }

        internal enum MapPoolType
        {
            None,
            Default,
            Custom
        }

        internal enum CurveType
        {
            ScoreSaber,
            BeatLeader,
            Linear,
            Basic,
            Dummy,
            AccSaber
        }

        internal enum PPGainCalculationType
        {
            Weighted,
            Raw
        }

        internal enum MapPoolSorting
        {
            Alphabetical,
            Popularity
        }

        internal enum Leaderboard
        {
            ScoreSaber,
            BeatLeader,
            AccSaber,
            NoLeaderboard,
            HitBloq
        }
    }
}
