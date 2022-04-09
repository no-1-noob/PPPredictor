using PPPredictor.Utilities;
using scoresaberapi;
using System.Collections.Generic;
using System.Linq;

namespace PPPredictor
{
    public class ProfileInfo
    {
        private Dictionary<string, double> dictBasePP;
        private Player sessionPlayer;
        private Player currentPlayer;
        private List<ShortScore> lsScores;
        private float lastPercentageSelected;
        public ProfileInfo()
        {
            DictBasePP = new Dictionary<string, double>();
            LastPercentageSelected = 90;
            LSScores = new List<ShortScore>();
        }

        public Player SessionPlayer { get => sessionPlayer; set => sessionPlayer = value; }
        public Player CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public Dictionary<string, double> DictBasePP { get => dictBasePP; set => dictBasePP = value; }
        public List<ShortScore> LSScores { get => lsScores; set => lsScores = value; }
        public float LastPercentageSelected { get => lastPercentageSelected; set => lastPercentageSelected = value; }

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