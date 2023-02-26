using PPPredictor.Data.Curve;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;

namespace PPPredictor.Data
{
    public class PPPLeaderboardInfo
    {
        private List<PPPMapPool> _lsMapPools;
        private string _leaderboardName;
        private string _leaderboardIcon;
        private PPPMapPool _currentMapPool;
        private string _lastSelectedMapPoolId;
        private string _customLeaderboardUserId;
        private string _ppSuffix;
        private List<PPPModifierValues> _lsModifierValues;

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
        public List<PPPModifierValues> LsModifierValues { get => _lsModifierValues; set => _lsModifierValues = value; }

        public PPPLeaderboardInfo(Leaderboard leaderboard)
        {
            this._leaderboardName = leaderboard.ToString();
            this._lsMapPools = new List<PPPMapPool>();
            this._customLeaderboardUserId = string.Empty;
            this._ppSuffix = "pp";
            this._lsModifierValues = new List<PPPModifierValues>();

            switch (leaderboard)
            {
                case Leaderboard.ScoreSaber:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.ScoreSaber.png";
                    _lsMapPools.Add(new PPPMapPool(MapPoolType.Default, $"Default", PPCalculatorScoreSaber.accumulationConstant, 0, CurveParser.ParseToCurve(new CurveInfo(CurveType.ScoreSaber))));
                    break;
                case Leaderboard.BeatLeader:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.BeatLeader.png";
                    _lsMapPools.Add(new PPPMapPool(MapPoolType.Default, $"Default", PPCalculatorBeatLeader.accumulationConstant, 0, new BeatLeaderPPPCurve()));
                    break;
                case Leaderboard.NoLeaderboard:
                    _leaderboardIcon = "";
                    _lsMapPools.Add(new PPPMapPool(MapPoolType.Default, $"Default", 0, 0, new CustomPPPCurve(new double[0, 2] { }, CurveType.Linear, 0)));
                    break;
                case Leaderboard.HitBloq:
                    _leaderboardIcon = "PPPredictor.Resources.LeaderBoardLogos.HitBloq.png";
                    _ppSuffix = "cr";
                    _lsMapPools.Add(new PPPMapPool(MapPoolType.Default, $"Default", 0, 0, new CustomPPPCurve(new double[0, 2] { }, CurveType.Linear, 0)));
                    break;
                default:
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
