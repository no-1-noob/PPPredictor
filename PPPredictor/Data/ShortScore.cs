using Newtonsoft.Json;
using System;

namespace PPPredictor.Data
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
            this._searchstring = searchstring.ToUpper();
            this._pp = pp;
            this._starRating = new PPPStarRating();
        }

        public ShortScore(string searchstring, PPPStarRating starRating, DateTime fetchTime)
        {
            this._searchstring = searchstring.ToUpper();
            this._starRating = starRating;
            this._fetchTime = fetchTime;
        }

        [JsonConstructor]
        public ShortScore(string searchstring, double pp, PPPStarRating starRating, DateTime fetchTime)
        {
            this._searchstring = searchstring.ToUpper();
            this._pp = pp;
            this._fetchTime = fetchTime;
            this._starRating = starRating ?? new PPPStarRating();
        }

        public bool ShouldSerializeStarRating()
        {
            return _starRating.IsRanked();
        }
    }
}
