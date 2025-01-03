using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace PPPredictor.Core.DataType.Score
{
    public class ShortScore
    {
        private readonly string _searchstring;
        private readonly string _category = "";
        private double _pp;
        private PPPStarRating _starRating;
        private DateTime _fetchTime;

        public string Searchstring { get => _searchstring; }
        public double Pp { get => _pp; set => _pp = value; }
        public PPPStarRating StarRating { get => _starRating; set => _starRating = value; }
        public DateTime FetchTime { get => _fetchTime; set => _fetchTime = value; }
        [DefaultValue("")]
        public string Category => _category;

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

        public ShortScore(string searchstring, PPPStarRating starRating, DateTime fetchTime, string category)
        {
            _searchstring = searchstring.ToUpper();
            _starRating = starRating;
            _fetchTime = fetchTime;
            _category = category;
        }

        [JsonConstructor]
        public ShortScore(string searchstring, double pp, PPPStarRating starRating, DateTime fetchTime, string category)
        {
            _searchstring = searchstring.ToUpper();
            _pp = pp;
            _fetchTime = fetchTime;
            _starRating = starRating ?? new PPPStarRating();
            _category = category;
        }

        public bool ShouldSerializeStarRating()
        {
            return _starRating.IsRanked();
        }
    }
}
