using PPPredictor.Utilities;
using System;
using System.Collections.Generic;

namespace PPPredictor.Data
{
    public class ProfileInfo
    {
        List<PPPLeaderboardInfo> _lsLeaderboardInfo;
        private float _lastPercentageSelected;
        private SVector3 _position;
        private SVector3 _eulerAngles;
        private bool _windowHandleEnabled;
        private bool _displaySessionValues;
        private int _resetSessionHours;
        private DateTime _lastSessionReset;
        private string _lastLeaderBoardSelected;
        private bool _counterShowGain;
        private bool _counterHighlightTargetPercentage;
        private bool _counterUseIcons;
        private CounterScoringType _counterScoringType;
        private bool _counterHideWhenUnranked;
        private string acknowledgedVersion;
        private DateTime _dtLastVersionCheck;
        private bool _isVersionCheckEnabled;

        private bool _isScoreSaberEnabled;
        private bool _isBeatLeaderEnabled;

        public ProfileInfo()
        {
            LsLeaderboardInfo = new List<PPPLeaderboardInfo>();
            LastPercentageSelected = 90;
            Position = new SVector3(2.5f, 0.05f, 2.0f);
            EulerAngles = new SVector3(88, 60, 0);
            WindowHandleEnabled = false;
            DisplaySessionValues = false;
            ResetSessionHours = 12;
            LastSessionReset = new DateTime();
            LastLeaderBoardSelected = Leaderboard.ScoreSaber.ToString();
            CounterShowGain = false;
            CounterScoringType = CounterScoringType.Global;
            CounterHighlightTargetPercentage = true;
            CounterHideWhenUnranked = true;
            AcknowledgedVersion = string.Empty;
            CounterUseIcons = true;
            DtLastVersionCheck = new DateTime(2000, 1, 1);
            IsVersionCheckEnabled = true;
            IsScoreSaberEnabled = true;
            IsBeatLeaderEnabled = true;
        }

        public float LastPercentageSelected { get => _lastPercentageSelected; set => _lastPercentageSelected = value; }
        public SVector3 Position { get => _position; set => _position = value; }
        public SVector3 EulerAngles { get => _eulerAngles; set => _eulerAngles = value; }
        public bool WindowHandleEnabled { get => _windowHandleEnabled; set => _windowHandleEnabled = value; }
        public bool DisplaySessionValues { get => _displaySessionValues; set => _displaySessionValues = value; }
        public int ResetSessionHours { get => _resetSessionHours; set => _resetSessionHours = value; }
        public DateTime LastSessionReset { get => _lastSessionReset; set => _lastSessionReset = value; }
        public List<PPPLeaderboardInfo> LsLeaderboardInfo { get => _lsLeaderboardInfo; set => _lsLeaderboardInfo = value; }
        public string LastLeaderBoardSelected { get => _lastLeaderBoardSelected; set => _lastLeaderBoardSelected = value; }
        public bool CounterShowGain { get => _counterShowGain; set => _counterShowGain = value; }
        public CounterScoringType CounterScoringType { get => _counterScoringType; set => _counterScoringType = value; }
        public bool CounterHighlightTargetPercentage { get => _counterHighlightTargetPercentage; set => _counterHighlightTargetPercentage = value; }
        public bool CounterHideWhenUnranked { get => _counterHideWhenUnranked; set => _counterHideWhenUnranked = value; }
        public string AcknowledgedVersion { get => acknowledgedVersion; set => acknowledgedVersion = value; }
        public bool CounterUseIcons { get => _counterUseIcons; set => _counterUseIcons = value; }
        public DateTime DtLastVersionCheck { get => _dtLastVersionCheck; set => _dtLastVersionCheck = value; }
        public bool IsVersionCheckEnabled { get => _isVersionCheckEnabled; set => _isVersionCheckEnabled = value; }
        public bool IsScoreSaberEnabled { get => _isScoreSaberEnabled; set => _isScoreSaberEnabled = value; }
        public bool IsBeatLeaderEnabled { get => _isBeatLeaderEnabled; set => _isBeatLeaderEnabled = value; }
    }
}