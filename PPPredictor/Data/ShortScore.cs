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
        private int _modifierValuesId;

        public string Searchstring { get => _searchstring; }
        public double Pp { get => _pp; set => _pp = value; }
        public PPPStarRating StarRating { get => _starRating; set => _starRating = value; }
        public DateTime FetchTime { get => _fetchTime; set => _fetchTime = value; }
        public int ModifierValuesId { get => _modifierValuesId; set => _modifierValuesId = value; }

        public ShortScore(string searchstring, double pp)
        {
            this._searchstring = searchstring.ToUpper();
            this._pp = pp;
        }

        public ShortScore(string searchstring, double stars, DateTime fetchTime)
        {
            this._searchstring = searchstring.ToUpper();
            this._stars = stars;
            this._fetchTime = fetchTime;
        }

        public ShortScore(string searchstring, PPPStarRating starRating, DateTime fetchTime, int modifierValuesId)
        {
            this._searchstring = searchstring.ToUpper();
            this._fetchTime = fetchTime;
            this._starRating = starRating;
            this._modifierValuesId = modifierValuesId;
        }
        [JsonConstructor]
        public ShortScore(string searchstring, double pp, PPPStarRating starRating, DateTime fetchTime, int modifierValuesId)
        {
            this._searchstring = searchstring.ToUpper();
            this._pp = pp;
            this._fetchTime = fetchTime;
            this._starRating = starRating;
            this._modifierValuesId = modifierValuesId;
        }
    }
}
