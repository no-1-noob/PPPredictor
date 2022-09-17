using beatleaderapi;
using scoresaberapi;
using System.Collections.Generic;

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

        public PPPScoreCollection(ScoreResponseWithMyScoreResponseWithMetadata scoreSaberPlayerScoreCollection)
        {
            this.page = scoreSaberPlayerScoreCollection.Metadata.Page;
            this.itemsPerPage = scoreSaberPlayerScoreCollection.Metadata.ItemsPerPage;
            this.total = scoreSaberPlayerScoreCollection.Metadata.Total;
            foreach (var playerScore in scoreSaberPlayerScoreCollection.Data)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }
    }
}
