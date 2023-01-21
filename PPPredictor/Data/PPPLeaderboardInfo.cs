using PPPredictor.Utilities;
using System;
using System.Collections.Generic;

namespace PPPredictor.Data
{
    public class PPPLeaderboardInfo
    {
        private PPPPlayer _sessionPlayer;
        private PPPPlayer _currentPlayer;
        private List<ShortScore> _lsScores;
        private List<ShortScore> _lsLeaderboardScores;
        private string _leaderboardName;
        private DateTimeOffset _dtLastScoreSet;
        private List<PPPModifierValues> _lsModifierValues;

        public PPPPlayer SessionPlayer { get => _sessionPlayer; set => _sessionPlayer = value; }
        public PPPPlayer CurrentPlayer { get => _currentPlayer; set => _currentPlayer = value; }
        public List<ShortScore> LSScores { get => _lsScores; set => _lsScores = value; }
        public string LeaderboardName { get => _leaderboardName; set => _leaderboardName = value; }
        public List<ShortScore> LsLeaderboardScores { get => _lsLeaderboardScores; set => _lsLeaderboardScores = value; }
        public DateTimeOffset DtLastScoreSet { get => _dtLastScoreSet; set => _dtLastScoreSet = value; }
        public List<PPPModifierValues> LsModifierValues { get => _lsModifierValues; set => _lsModifierValues = value; }

        public PPPLeaderboardInfo(Leaderboard leaderboard)
        {
            this._leaderboardName = leaderboard.ToString();
            this._lsScores = new List<ShortScore>();
            this._lsLeaderboardScores = new List<ShortScore>();
            this._currentPlayer = new PPPPlayer();
            this._sessionPlayer = new PPPPlayer();
            this._dtLastScoreSet = new DateTime(2000, 1, 1);
            this._lsModifierValues = new List<PPPModifierValues>();
        }
    }
}
