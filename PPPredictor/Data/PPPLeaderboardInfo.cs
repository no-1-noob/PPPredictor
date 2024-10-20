﻿using PPPredictor.Data.Curve;
using PPPredictor.OpenAPIs;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;

namespace PPPredictor.Data
{
    class PPPLeaderboardInfo
    {
        private List<PPPMapPool> _lsMapPools;
        private string _leaderboardName;
        private string _leaderboardIcon;
        private PPPMapPool _currentMapPool;
        private string _lastSelectedMapPoolId;
        private string _customLeaderboardUserId;
        private string _ppSuffix;
        private int _leaderboardFirstPageIndex;
        private bool _isCountryRankEnabled;
        private int _largePageSize;

        public string LeaderboardName { get => _leaderboardName; set => _leaderboardName = value; }
        public string LeaderboardIcon { get => _leaderboardIcon; set => _leaderboardIcon = value; }
        public List<PPPMapPool> LsMapPools { get => _lsMapPools; set => _lsMapPools = value; }
        public string LastSelectedMapPoolId { get => _lastSelectedMapPoolId; set => _lastSelectedMapPoolId = value; }
        internal PPPMapPool CurrentMapPool { get => _currentMapPool; set
            {
                _currentMapPool = value;
                LastSelectedMapPoolId = value.Id;
            }
        }
        internal PPPMapPool DefaultMapPool { get => _lsMapPools.Find(x => x.MapPoolType == MapPoolType.Default); }
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
            this._lsMapPools = new List<PPPMapPool>();
            this._customLeaderboardUserId = string.Empty;
            this._ppSuffix = "pp";
            this.LeaderboardFirstPageIndex = 1;
            this.IsCountryRankEnabled = true;
            this.LargePageSize = 10;

            switch (leaderboard)
            {
                case Leaderboard.ScoreSaber:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.ScoreSaber.png";
                    _lsMapPools.Add(new PPPMapPool(MapPoolType.Default, $"", PPCalculatorScoreSaber<ScoresaberAPI>.accumulationConstant, 0, CurveParser.ParseToCurve(new CurveInfo(CurveType.ScoreSaber))));
                    break;
                case Leaderboard.BeatLeader:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.BeatLeader.png";
                    //Negative ids for distinguishing from possible future event map pools
                    _lsMapPools.Add(new PPPMapPool("-1", MapPoolType.Default, $"General", PPCalculatorBeatLeader<BeatleaderAPI>.accumulationConstant, 0, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderDefault));
                    _lsMapPools.Add(new PPPMapPool("-2", MapPoolType.Default, $"No modifiers", PPCalculatorBeatLeader<BeatleaderAPI>.accumulationConstant, 0, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderNoModifiers));
                    _lsMapPools.Add(new PPPMapPool("-3", MapPoolType.Default, $"No pauses", PPCalculatorBeatLeader<BeatleaderAPI>.accumulationConstant, 0, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderNoPauses));
                    _lsMapPools.Add(new PPPMapPool("-4", MapPoolType.Default, $"Golf", PPCalculatorBeatLeader<BeatleaderAPI>.accumulationConstant, 0, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderGolf));
                    _lsMapPools.Add(new PPPMapPool("-5", MapPoolType.Default, $"SCPM", PPCalculatorBeatLeader<BeatleaderAPI>.accumulationConstant, 0, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderSCPM));
                    this.LargePageSize = 100;
                    break;
                case Leaderboard.NoLeaderboard:
                    _leaderboardIcon = "";
                    _lsMapPools.Add(new PPPMapPool(MapPoolType.Default, $"", 0, 0, new CustomPPPCurve(new List<(double, double)>(), CurveType.Linear, 0)));
                    break;
                case Leaderboard.HitBloq:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.HitBloq.png";
                    _ppSuffix = "cr";
                    IsCountryRankEnabled = false;
                    LeaderboardFirstPageIndex = 0;
                    _lsMapPools.Add(new PPPMapPool(MapPoolType.Default, $"☞ Select a map pool ☜", 0, 0, new CustomPPPCurve(new List<(double, double)>(), CurveType.Linear, 0)));
                    break;
                case Leaderboard.AccSaber:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.AccSaber.png";
                    _ppSuffix = "ap";
                    IsCountryRankEnabled = false;
                    LeaderboardFirstPageIndex = 0;
                    _lsMapPools.Add(new PPPMapPool(MapPoolType.Default, $"☞ Select a map pool ☜", 0, 0, CurveParser.ParseToCurve(new CurveInfo(CurveType.AccSaber)), 0));
                    break;
            }

            //THIS IS DEBUG STUFF FOR NOW
            SetCurrentMapPool();
        }

        internal void SetCurrentMapPool()
        {
            if (_lsMapPools != null && _lsMapPools.Count > 0)
            {
                int index = Math.Max(_lsMapPools.FindIndex(x => x.Id == LastSelectedMapPoolId), 0); //Set the last used map pool
                this._currentMapPool = _lsMapPools[index];
            }
        }
    }
}
