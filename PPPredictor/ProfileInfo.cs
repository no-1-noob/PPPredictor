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
        private List<Score> lsScores;
        public ProfileInfo()
        {
            DictBasePP = new Dictionary<string, double>();
        }

        public Player SessionPlayer { get => sessionPlayer; set => sessionPlayer = value; }
        public Player CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public Dictionary<string, double> DictBasePP { get => dictBasePP; set => dictBasePP = value; }
        public List<Score> LSScores { get => lsScores; set => lsScores = value; }

        public void addDictBasePP(string hash, int difficulty, double basePp)
        {
            string seachString = createSeachString(hash, difficulty);
            DictBasePP.Add(seachString, basePp);
        }

        public double findBasePP(string hash, int difficulty)
        {
            double basePP = 0;
            string seachString = createSeachString(hash, difficulty);
            if(DictBasePP.TryGetValue(seachString, out basePP))
            {
                return basePP;
            }
            return -1;
        }

        private string createSeachString(string hash, int difficulty)
        {
            return $"{hash}_{difficulty}";
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