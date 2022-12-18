using PPPredictor.OpenAPIs;
using scoresaberapi;
using System.Collections.Generic;
using static PPPredictor.OpenAPIs.beatleaderapi;

namespace PPPredictor.Data
{
    public class PPPScoreCollection
    {
        private readonly List<PPPScore> lsPPPScore = new List<PPPScore>();
        private readonly double page;
        private readonly double itemsPerPage;
        private readonly double total;

        public List<PPPScore> LsPPPScore { get => lsPPPScore; }
        public double Page { get => page; }
        public double ItemsPerPage { get => itemsPerPage; }
        public double Total { get => total; }

        public PPPScoreCollection()
        {
            lsPPPScore = new List<PPPScore>();
            page = -1;
            itemsPerPage = -1;
            total = -1;
        }

        public PPPScoreCollection(PlayerScoreCollection scoreSaberPlayerScoreCollection)
        {
            this.page = scoreSaberPlayerScoreCollection.Metadata.Page;
            this.itemsPerPage = scoreSaberPlayerScoreCollection.Metadata.ItemsPerPage;
            this.total = scoreSaberPlayerScoreCollection.Metadata.Total;
            foreach (var playerScore in scoreSaberPlayerScoreCollection.PlayerScores)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        public PPPScoreCollection(BeatLeaderPlayerScoreList beatLeaderPlayerScoreList)
        {
            this.page = beatLeaderPlayerScoreList.metadata.page;
            this.itemsPerPage = beatLeaderPlayerScoreList.metadata.itemsPerPage;
            this.total = beatLeaderPlayerScoreList.metadata.total;
            foreach (var playerScore in beatLeaderPlayerScoreList.data)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        public PPPScoreCollection(List<HitBloqScores> lsHitBloqScores, int page)
        {
            this.page = page;
            this.itemsPerPage = 10;
            this.total = (lsHitBloqScores.Count > 0) ? page * itemsPerPage + 1 : 0;
            foreach (var playerScore in lsHitBloqScores)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        
    }
}
