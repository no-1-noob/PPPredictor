using PPPredictor.Utilities;
using scoresaberapi;
using System.Collections.Generic;
using System.ComponentModel;

namespace PPPredictor
{
    public class ProfileInfo
    {
        private Dictionary<string, double> dictBasePP;
        private Player sessionPlayer;
        private Player currentPlayer;
        private List<ShortScore> lsScores;
        private float lastPercentageSelected;
        private SVector3 position;
        private SVector3 eulerAngles;
        private bool windowHandleEnabled;
        public ProfileInfo()
        {
            dictBasePP = new Dictionary<string, double>();
            lsScores = new List<ShortScore>();
            DictBasePP = new Dictionary<string, double>();
            LastPercentageSelected = 90;
            LSScores = new List<ShortScore>();
            Position = new SVector3(2.5f, 0.05f, 2.0f);
            EulerAngles = new SVector3(88, 60, 0);
            WindowHandleEnabled = false;
        }

        public Player SessionPlayer { get => sessionPlayer; set => sessionPlayer = value; }
        public Player CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public Dictionary<string, double> DictBasePP { get => dictBasePP; set => dictBasePP = value; }
        public List<ShortScore> LSScores { get => lsScores; set => lsScores = value; }
        public float LastPercentageSelected { get => lastPercentageSelected; set => lastPercentageSelected = value; }
        public SVector3 Position { get => position; set => position = value; }
        public SVector3 EulerAngles { get => eulerAngles; set => eulerAngles = value; }
        public bool WindowHandleEnabled { get => windowHandleEnabled; set => windowHandleEnabled = value; }

        public void addDictBasePP(string hash, int difficulty, double basePp)
        {
            string seachString = PPCalculator.createSeachString(hash, difficulty);
            DictBasePP.Add(seachString, basePp);
        }

        public double findBasePP(string hash, int difficulty)
        {
            double basePP = 0;
            string seachString = PPCalculator.createSeachString(hash, difficulty);
            if(DictBasePP.TryGetValue(seachString, out basePP))
            {
                return basePP;
            }
            return -1;
        }
    }
}