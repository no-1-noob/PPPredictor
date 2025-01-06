using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.DataType.LeaderBoard
{
    public class PPPLeaderboardInfo
    {
        private string _leaderboardName;
        private string _leaderboardIcon;
        private string _ppSuffix;
        private int _leaderboardFirstPageIndex;
        private bool _isCountryRankEnabled;
        private int _largePageSize;
        private int _playerPerPages = 0;
        private bool _hasGetAllScoresFunctionality = false;
        private bool _hasGetRecentScoresFunctionality = true;
        private bool _hasPPToRankFunctionality = false;
        private int _taskDelayValue = 250;
        private bool _hasOldDotRanking = true;
        private int _pageFetchLimit = 5;

        public string LeaderboardName { get => _leaderboardName; set => _leaderboardName = value; }
        public string LeaderboardIcon { get => _leaderboardIcon; set => _leaderboardIcon = value; }
        public string PpSuffix { get => _ppSuffix; set => _ppSuffix = value; }
        public int LeaderboardFirstPageIndex { get => _leaderboardFirstPageIndex; set => _leaderboardFirstPageIndex = value; }
        public bool IsCountryRankEnabled { get => _isCountryRankEnabled; set => _isCountryRankEnabled = value; }
        public int LargePageSize { get => _largePageSize; set => _largePageSize = value; }
        public int PlayerPerPages { get => _playerPerPages; set => _playerPerPages = value; }
        public bool HasGetAllScoresFunctionality { get => _hasGetAllScoresFunctionality; set => _hasGetAllScoresFunctionality = value; }
        public bool HasGetRecentScoresFunctionality { get => _hasGetRecentScoresFunctionality; set => _hasGetRecentScoresFunctionality = value; }
        public bool HasPPToRankFunctionality { get => _hasPPToRankFunctionality; set => _hasPPToRankFunctionality = value; }
        public int TaskDelayValue { get => _taskDelayValue; set => _taskDelayValue = value; }
        public bool HasOldDotRanking { get => _hasOldDotRanking; set => _hasOldDotRanking = value; }
        public int PageFetchLimit { get => _pageFetchLimit; set => _pageFetchLimit = value; }

        public PPPLeaderboardInfo()
        {
        }

        public PPPLeaderboardInfo(Leaderboard leaderboard)
        {
            this._leaderboardName = leaderboard.ToString();
            this._ppSuffix = "pp";
            this.LeaderboardFirstPageIndex = 1;
            this.IsCountryRankEnabled = true;
            this.LargePageSize = 10;

            switch (leaderboard)
            {
                case Leaderboard.ScoreSaber:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.ScoreSaber.png";
                    PlayerPerPages = 50;
                    HasOldDotRanking = false;
                    break;
                case Leaderboard.BeatLeader:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.BeatLeader.png";
                    this.LargePageSize = 100;
                    PlayerPerPages = 50;
                    TaskDelayValue = 1100;
                    break;
                case Leaderboard.NoLeaderboard:
                    _leaderboardIcon = "";
                    break;
                case Leaderboard.HitBloq:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.HitBloq.png";
                    _ppSuffix = "cr";
                    IsCountryRankEnabled = false;
                    LeaderboardFirstPageIndex = 0;
                    PlayerPerPages = 10;
                    HasGetAllScoresFunctionality = true;
                    HasPPToRankFunctionality = true;
                    break;
                case Leaderboard.AccSaber:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.AccSaber.png";
                    _ppSuffix = "ap";
                    IsCountryRankEnabled = false;
                    LeaderboardFirstPageIndex = 0;
                    HasGetAllScoresFunctionality = true;
                    HasGetRecentScoresFunctionality = false;
                    break;
            }
        }
    }
}
