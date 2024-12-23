using Newtonsoft.Json;
using System;

namespace PPPredictor.Core.DataType.Score
{
    class ShortScore
    {
        private readonly string _searchstring;
        private double _pp;
        private PPPStarRating _starRating;
        private DateTime _fetchTime;

        public string Searchstring { get => _searchstring; }
        public double Pp { get => _pp; set => _pp = value; }
        public PPPStarRating StarRating { get => _starRating; set => _starRating = value; }
        public DateTime FetchTime { get => _fetchTime; set => _fetchTime = value; }

        public ShortScore(string searchstring, double pp)
        {
            _searchstring = searchstring.ToUpper();
            _pp = pp;
            _starRating = new PPPStarRating();
        }

        public ShortScore(string searchstring, PPPStarRating starRating, DateTime fetchTime)
        {
            _searchstring = searchstring.ToUpper();
            _starRating = starRating;
            _fetchTime = fetchTime;
        }

        [JsonConstructor]
        public ShortScore(string searchstring, double pp, PPPStarRating starRating, DateTime fetchTime)
        {
            _searchstring = searchstring.ToUpper();
            _pp = pp;
            _fetchTime = fetchTime;
            _starRating = starRating ?? new PPPStarRating();
        }

        public bool ShouldSerializeStarRating()
        {
            return _starRating.IsRanked();
        }
    }
}
