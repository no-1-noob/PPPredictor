using PPPredictor.Utilities;
using System.Collections.Generic;

namespace PPPredictor.Data
{
    public class PPPLeaderboardInfo
    {
        private PPPPlayer _sessionPlayer;
        private PPPPlayer _currentPlayer;
        private List<ShortScore> _lsScores;
        private string _leaderboardName;


        public PPPPlayer SessionPlayer { get => _sessionPlayer; set => _sessionPlayer = value; }
        public PPPPlayer CurrentPlayer { get => _currentPlayer; set => _currentPlayer = value; }
        public List<ShortScore> LSScores { get => _lsScores; set => _lsScores = value; }
        public string LeaderboardName { get => _leaderboardName; set => _leaderboardName = value; }

        public PPPLeaderboardInfo(Leaderboard leaderboard)
        {
            this._leaderboardName = leaderboard.ToString();
            this._lsScores = new List<ShortScore>();
            this._currentPlayer = new PPPPlayer();
            this._sessionPlayer = new PPPPlayer();
        }
    }
}
