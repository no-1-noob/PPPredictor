namespace PPPredictor.Utilities
{
    public enum BeatLeaderDifficultyStatus
    {
        unranked = 0,
        nominated = 1,
        qualified = 2,
        ranked = 3,
        unrankable = 4,
        outdated = 5,
        inevent = 6
    }

    public enum Leaderboard
    {
        ScoreSaber,
        BeatLeader
    }
}
