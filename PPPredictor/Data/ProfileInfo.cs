﻿using Newtonsoft.Json;
using PPPredictor.Core;
using PPPredictor.Core.DataType.LeaderBoard;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Data
{
    class ProfileInfo
    {
        Dictionary<string, LeaderboardData> _dctleaderBoardData;
        private float _lastPercentageSelected;
        private SVector3 _position;
        private SVector3 _eulerAngles;
        private bool _displaySessionValues;
        private int _resetSessionHours;
        private DateTime _lastSessionReset;
        private string _lastLeaderBoardSelected;
        private Dictionary<string, string> _mapPoolSelection;
        private bool _counterHighlightTargetPercentage;
        private bool _counterUseIcons;
        private bool _counterUseCustomMapPoolIcons;
        private CounterScoringType _counterScoringType;
        //private PPGainCalculationType _ppGainCalculationType;
        private CounterDisplayType _counterDisplayType;
        private int _rawPPLossHighlightThreshold;
        private bool _counterHideWhenUnranked;
        private string acknowledgedVersion;
        private DateTime _dtLastVersionCheck;
        private bool _isVersionCheckEnabled;
        private int _profileInfoVersion;
        private int _selectedTab;
        private MapPoolSorting _hitbloqMapPoolSorting;
        private bool _isPredictorSwitchBySyncUrlEnabled;
        private bool _isCounterGainSilentModeEnabled;
        private float _lastMinPercentageSelected;
        private float _lastMaxPercentageSelected;

        private bool _isScoreSaberEnabled;
        private bool _isBeatLeaderEnabled;
        private bool _isHitBloqEnabled;
        private bool _isAccSaberEnabled;
        private bool _isAccSaberEnabledManual;

        private string _streamOverlayPort;
        private PPGainCalculationType _ppGainCalculationType;
        internal const int RefetchMapInfoAfterDays = -7;

        public ProfileInfo()
        {
            LastPercentageSelected = 90;
            Position = MenuPositionHelper.UnderScoreboardPosition;
            EulerAngles = MenuPositionHelper.UnderScoreboardEulerAngles;
            DisplaySessionValues = false;
            ResetSessionHours = 12;
            LastSessionReset = new DateTime();
            LastLeaderBoardSelected = Leaderboard.ScoreSaber.ToString();
            MapPoolSelection = new Dictionary<string, string>();
            CounterDisplayType = CounterDisplayType.PP;
            CounterScoringType = CounterScoringType.Local;
            CounterHighlightTargetPercentage = true;
            CounterHideWhenUnranked = true;
            AcknowledgedVersion = string.Empty;
            CounterUseIcons = true;
            CounterUseCustomMapPoolIcons = true;
            DtLastVersionCheck = new DateTime(2000, 1, 1);
            IsVersionCheckEnabled = true;
            IsScoreSaberEnabled = true;
            IsBeatLeaderEnabled = true;
            IsHitBloqEnabled = true;
            IsAccSaberEnabled = true;
            IsAccSaberEnabledManual = false;
            //PpGainCalculationType = PPGainCalculationType.Weighted;
            RawPPLossHighlightThreshold = -10;
            ProfileInfoVersion = 0;
            SelectedTab = 0;
            HitbloqMapPoolSorting = MapPoolSorting.Popularity;
            IsPredictorSwitchBySyncUrlEnabled = true;
            IsCounterGainSilentModeEnabled = false;
            LastMinPercentageSelected = ((int)(LastPercentageSelected / 10)) * 10;
            LastMaxPercentageSelected = Math.Min((((int)(LastPercentageSelected / 10)) * 10) + 10, 100);
            StreamOverlayPort = "6558";
            _dctleaderBoardData = new Dictionary<string, LeaderboardData>();
        }

        internal void ResetCachedData()
        {
            _dctleaderBoardData = new Dictionary<string, LeaderboardData>();
        }

        public float LastPercentageSelected { get => _lastPercentageSelected; set => _lastPercentageSelected = value; }
        public SVector3 Position { get => _position; set => _position = value; }
        public SVector3 EulerAngles { get => _eulerAngles; set => _eulerAngles = value; }
        public bool DisplaySessionValues { get => _displaySessionValues; set => _displaySessionValues = value; }
        public int ResetSessionHours { get => _resetSessionHours; set => _resetSessionHours = value; }
        public DateTime LastSessionReset { get => _lastSessionReset; set => _lastSessionReset = value; }
        public string LastLeaderBoardSelected { get => _lastLeaderBoardSelected; set => _lastLeaderBoardSelected = value; }
        public CounterScoringType CounterScoringType { get => _counterScoringType; set => _counterScoringType = value; }
        public bool CounterHighlightTargetPercentage { get => _counterHighlightTargetPercentage; set => _counterHighlightTargetPercentage = value; }
        public bool CounterHideWhenUnranked { get => _counterHideWhenUnranked; set => _counterHideWhenUnranked = value; }
        public string AcknowledgedVersion { get => acknowledgedVersion; set => acknowledgedVersion = value; }
        public bool CounterUseIcons { get => _counterUseIcons; set => _counterUseIcons = value; }
        public bool CounterUseCustomMapPoolIcons { get => _counterUseCustomMapPoolIcons; set => _counterUseCustomMapPoolIcons = value; }
        public DateTime DtLastVersionCheck { get => _dtLastVersionCheck; set => _dtLastVersionCheck = value; }
        public bool IsVersionCheckEnabled { get => _isVersionCheckEnabled; set => _isVersionCheckEnabled = value; }
        [JsonIgnore]
        public bool IsScoreSaberEnabled { get => _isScoreSaberEnabled; set => _isScoreSaberEnabled = value; }
        [JsonIgnore]
        public bool IsBeatLeaderEnabled { get => _isBeatLeaderEnabled; set => _isBeatLeaderEnabled = value; }
        [JsonIgnore]
        public bool IsHitBloqEnabled { get => _isHitBloqEnabled; set => _isHitBloqEnabled = value; }
        [JsonIgnore]
        public bool IsAccSaberEnabled { get => _isAccSaberEnabled; set => _isAccSaberEnabled = value; }
        public PPGainCalculationType PpGainCalculationType { get => _ppGainCalculationType; set => _ppGainCalculationType = value; }
        public int RawPPLossHighlightThreshold { get => _rawPPLossHighlightThreshold; set => _rawPPLossHighlightThreshold = value; }
        public CounterDisplayType CounterDisplayType { get => _counterDisplayType; set => _counterDisplayType = value; }
        public int ProfileInfoVersion { get => _profileInfoVersion; set => _profileInfoVersion = value; }
        public int SelectedTab { get => _selectedTab; set => _selectedTab = value; }
        public MapPoolSorting HitbloqMapPoolSorting { get => _hitbloqMapPoolSorting; set => _hitbloqMapPoolSorting = value; }
        public bool IsPredictorSwitchBySyncUrlEnabled { get => _isPredictorSwitchBySyncUrlEnabled; set => _isPredictorSwitchBySyncUrlEnabled = value; }
        public bool IsCounterGainSilentModeEnabled { get => _isCounterGainSilentModeEnabled; set => _isCounterGainSilentModeEnabled = value; }
        public float LastMinPercentageSelected { get => _lastMinPercentageSelected; set => _lastMinPercentageSelected = value; }
        public float LastMaxPercentageSelected { get => _lastMaxPercentageSelected; set => _lastMaxPercentageSelected = value; }
        public string StreamOverlayPort { get => _streamOverlayPort; set => _streamOverlayPort = value; }
        public bool IsAccSaberEnabledManual { get => _isAccSaberEnabledManual; set => _isAccSaberEnabledManual = value; }
        public Dictionary<string, LeaderboardData> DctleaderBoardData { get => _dctleaderBoardData; set => _dctleaderBoardData = value; }
        public Dictionary<string, string> MapPoolSelection { get => _mapPoolSelection; set => _mapPoolSelection = value; }

        internal void ClearOldMapInfos()
        {
            foreach (LeaderboardData leaderboard in _dctleaderBoardData.Values.ToList())
            {
                foreach (PPPMapPool mappool in leaderboard.DctMapPool.Values)
                {
                    mappool.LsLeaderboadInfo = mappool.LsLeaderboadInfo.Where(x => x.FetchTime > DateTime.Now.AddDays(ProfileInfo.RefetchMapInfoAfterDays)).ToList();
                }
            }
        }

        public async Task<Settings> ParseToSetting()
        {
            return new Settings(
                    IsScoreSaberEnabled,
                    IsBeatLeaderEnabled,
                    IsHitBloqEnabled,
                    IsAccSaberEnabled,
                    (await Plugin.GetUserInfoBS()).platformUserId,
                    PpGainCalculationType,
                    HitbloqMapPoolSorting,
                    //Plugin.ProfileInfo.platformUserId,
                    "",
                    RefetchMapInfoAfterDays,
                    LastSessionReset,
                    ResetSessionHours
                );
        }
    }
}