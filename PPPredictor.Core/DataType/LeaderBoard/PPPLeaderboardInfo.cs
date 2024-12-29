using PPPredictor.Core.API;
using PPPredictor.Core.DataType.Curve;
using System;
using System.Collections.Generic;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.DataType.LeaderBoard
{
    public class PPPLeaderboardInfo
    {
        private string _leaderboardName;
        private string _leaderboardIcon;
        private string _lastSelectedMapPoolId;
        private string _customLeaderboardUserId;
        private string _ppSuffix;
        private int _leaderboardFirstPageIndex;
        private bool _isCountryRankEnabled;
        private int _largePageSize;

        public string LeaderboardName { get => _leaderboardName; set => _leaderboardName = value; }
        public string LeaderboardIcon { get => _leaderboardIcon; set => _leaderboardIcon = value; }
        public string LastSelectedMapPoolId { get => _lastSelectedMapPoolId; set => _lastSelectedMapPoolId = value; }
        //internal PPPMapPool DefaultMapPool { get => _dctMapPool.Find(x => x.MapPoolType == MapPoolType.Default); }
        public string CustomLeaderboardUserId { get => _customLeaderboardUserId; set => _customLeaderboardUserId = value; }
        public string PpSuffix { get => _ppSuffix; set => _ppSuffix = value; }
        public int LeaderboardFirstPageIndex { get => _leaderboardFirstPageIndex; set => _leaderboardFirstPageIndex = value; }
        public bool IsCountryRankEnabled { get => _isCountryRankEnabled; set => _isCountryRankEnabled = value; }
        public int LargePageSize { get => _largePageSize; set => _largePageSize = value; }

        public PPPLeaderboardInfo()
        {
        }

        public PPPLeaderboardInfo(Leaderboard leaderboard)
        {
            this._leaderboardName = leaderboard.ToString();
            this._customLeaderboardUserId = string.Empty;
            this._ppSuffix = "pp";
            this.LeaderboardFirstPageIndex = 1;
            this.IsCountryRankEnabled = true;
            this.LargePageSize = 10;
            PPPMapPool mapPool = null;

            switch (leaderboard)
            {
                case Leaderboard.ScoreSaber:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.ScoreSaber.png";
#warning readd
                    //_lsMapPools.Add(new PPPMapPool(MapPoolType.Default, $"", PPCalculatorScoreSaber<ScoresaberAPI>.accumulationConstant, 0, CurveParser.ParseToCurve(new CurveInfo(CurveType.ScoreSaber))));
                    break;
                case Leaderboard.BeatLeader:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.BeatLeader.png";
                    this.LargePageSize = 100;
                    break;
                case Leaderboard.NoLeaderboard:
                    _leaderboardIcon = "";
                    break;
                case Leaderboard.HitBloq:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.HitBloq.png";
                    _ppSuffix = "cr";
                    IsCountryRankEnabled = false;
                    LeaderboardFirstPageIndex = 0;
                    break;
                case Leaderboard.AccSaber:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.AccSaber.png";
                    _ppSuffix = "ap";
                    IsCountryRankEnabled = false;
                    LeaderboardFirstPageIndex = 0;
                    break;
            }

            //THIS IS DEBUG STUFF FOR NOW
            SetCurrentMapPool();
        }

        internal void SetCurrentMapPool()
        {
#warning needed? I dont think so
            //if (_lsMapPools != null && _lsMapPools.Count > 0)
            //{
            //    int index = Math.Max(_lsMapPools.FindIndex(x => x.Id == LastSelectedMapPoolId), 0); //Set the last used map pool
            //    this._currentMapPool = _lsMapPools[index];
            //}
        }
    }
}
