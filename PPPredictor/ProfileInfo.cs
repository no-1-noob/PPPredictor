using PPPredictor.Utilities;
using scoresaberapi;
using System.Collections.Generic;

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
        public ProfileInfo()
        {
            DictBasePP = new Dictionary<string, double>();
            LastPercentageSelected = 90;
            LSScores = new List<ShortScore>();
            Position = new SVector3(2.25f, 1.25f, 2.2f);
            EulerAngles = new SVector3(60, 45, 0);
        }

        public Player SessionPlayer { get => sessionPlayer; set => sessionPlayer = value; }
        public Player CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public Dictionary<string, double> DictBasePP { get => dictBasePP; set => dictBasePP = value; }
        public List<ShortScore> LSScores { get => lsScores; set => lsScores = value; }
        public float LastPercentageSelected { get => lastPercentageSelected; set => lastPercentageSelected = value; }
        public SVector3 Position { get => position; set => position = value; }
        public SVector3 EulerAngles { get => eulerAngles; set => eulerAngles = value; }

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

        /*public bool insertScores(List<Score> lsScores)
        {
            bool needMoreData = !this.lsScores.Any();
            if (this.lsScores.Any())
            {
                needMoreData = this.lsScores.Select(x => x.TimeSet).Max() < lsScores.Select(x => x.TimeSet).Min();
            }
            foreach (Score score in lsScores)
            {
                oldScore = this.lsScores.Find(x => x.);
            }
            this.lsScores.AddRange(lsScores);
        }*/
    }
}