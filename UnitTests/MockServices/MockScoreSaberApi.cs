using PPPredictor.Data.LeaderBoardDataTypes;
using PPPredictor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.MockServices
{
    internal class MockScoreSaberApi : IScoresaberAPI
    {
        public Task<ScoreSaberDataTypes.ScoreSaberPlayer> GetPlayer(long playerId)
        {
            throw new NotImplementedException();
        }

        public Task<ScoreSaberDataTypes.ScoreSaberPlayerList> GetPlayers(double? page)
        {
            throw new NotImplementedException();
        }

        public Task<ScoreSaberDataTypes.ScoreSaberPlayerScoreList> GetPlayerScores(string playerId, int? limit, int? page)
        {
            throw new NotImplementedException();
        }
    }
}
