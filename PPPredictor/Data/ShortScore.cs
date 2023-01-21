using Newtonsoft.Json;
using System;

namespace PPPredictor.Data
{
    public class ShortScore
    {
        private readonly string _searchstring;
        private double _pp;
        private double _stars;
        private DateTime _fetchTime;
        private int _modifierValuesId;

        public string Searchstring { get => _searchstring; }
        public double Pp { get => _pp; set => _pp = value; }
        public double Stars { get => _stars; set => _stars = value; }
        public DateTime FetchTime { get => _fetchTime; set => _fetchTime = value; }
        public int ModifierValuesId { get => _modifierValuesId; set => _modifierValuesId = value; }

        public ShortScore(string searchstring, double pp)
        {
            this._searchstring = searchstring.ToUpper();
            this._pp = pp;
        }

        public ShortScore(string searchstring, double stars, DateTime fetchTime, int modifierValuesId)
        {
            this._searchstring = searchstring.ToUpper();
            this._fetchTime = fetchTime;
            this._stars = stars;
            this._modifierValuesId = modifierValuesId;
        }
        [JsonConstructor]
        public ShortScore(string searchstring, double pp, double stars, DateTime fetchTime, int modifierValuesId)
        {
            this._searchstring = searchstring.ToUpper();
            this._pp = pp;
            this._fetchTime = fetchTime;
            this._stars = stars;
            this._modifierValuesId = modifierValuesId;
        }
    }
}
