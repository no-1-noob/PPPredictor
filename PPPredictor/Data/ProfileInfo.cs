using scoresaberapi;
using System;
using System.Collections.Generic;

namespace PPPredictor.Data
{
    public class ProfileInfo
    {
        private Player _sessionPlayer;
        private Player _currentPlayer;
        private List<ShortScore> _lsScores;
        private float _lastPercentageSelected;
        private SVector3 _position;
        private SVector3 _eulerAngles;
        private bool _windowHandleEnabled;
        private bool _displaySessionValues;
        private int _resetSessionHours;
        private DateTime _lastSessionReset;

        public ProfileInfo()
        {
            LSScores = new List<ShortScore>();
            LastPercentageSelected = 90;
            LSScores = new List<ShortScore>();
            Position = new SVector3(2.5f, 0.05f, 2.0f);
            EulerAngles = new SVector3(88, 60, 0);
            WindowHandleEnabled = false;
            DisplaySessionValues = false;
            ResetSessionHours = 12;
            LastSessionReset = new DateTime();
        }

        public Player SessionPlayer { get => _sessionPlayer; set => _sessionPlayer = value; }
        public Player CurrentPlayer { get => _currentPlayer; set => _currentPlayer = value; }
        public List<ShortScore> LSScores { get => _lsScores; set => _lsScores = value; }
        public float LastPercentageSelected { get => _lastPercentageSelected; set => _lastPercentageSelected = value; }
        public SVector3 Position { get => _position; set => _position = value; }
        public SVector3 EulerAngles { get => _eulerAngles; set => _eulerAngles = value; }
        public bool WindowHandleEnabled { get => _windowHandleEnabled; set => _windowHandleEnabled = value; }
        public bool DisplaySessionValues { get => _displaySessionValues; set => _displaySessionValues = value; }
        public int ResetSessionHours { get => _resetSessionHours; set => _resetSessionHours = value; }
        public DateTime LastSessionReset { get => _lastSessionReset; set => _lastSessionReset = value; }
    }
}