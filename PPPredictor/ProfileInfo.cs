using PPPredictor.Utilities;
using scoresaberapi;
using System;
using System.Collections.Generic;

namespace PPPredictor
{
    public class ProfileInfo
    {
        private Player sessionPlayer;
        private Player currentPlayer;
        private List<ShortScore> lsScores;
        private float lastPercentageSelected;
        private SVector3 position;
        private SVector3 eulerAngles;
        private bool windowHandleEnabled;
        private bool displaySessionValues;
        private int resetSessionHours;
        private DateTime lastSessionReset;

        public ProfileInfo()
        {
            lsScores = new List<ShortScore>();
            LastPercentageSelected = 90;
            LSScores = new List<ShortScore>();
            Position = new SVector3(2.5f, 0.05f, 2.0f);
            EulerAngles = new SVector3(88, 60, 0);
            WindowHandleEnabled = false;
            DisplaySessionValues = false;
            ResetSessionHours = 12;
            LastSessionReset = new DateTime();
    }

        public Player SessionPlayer { get => sessionPlayer; set => sessionPlayer = value; }
        public Player CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public List<ShortScore> LSScores { get => lsScores; set => lsScores = value; }
        public float LastPercentageSelected { get => lastPercentageSelected; set => lastPercentageSelected = value; }
        public SVector3 Position { get => position; set => position = value; }
        public SVector3 EulerAngles { get => eulerAngles; set => eulerAngles = value; }
        public bool WindowHandleEnabled { get => windowHandleEnabled; set => windowHandleEnabled = value; }
        public bool DisplaySessionValues { get => displaySessionValues; set => displaySessionValues = value; }
        public int ResetSessionHours { get => resetSessionHours; set => resetSessionHours = value; }
        public DateTime LastSessionReset { get => lastSessionReset; set => lastSessionReset = value; }
    }
}