using Newtonsoft.Json;
using System;

namespace PPPredictor.Data
{
    public class ShortScore : IComparable<ShortScore>
    {
        private DateTimeOffset _timeSet;
        private readonly string _searchstring;
        private double _pp;
        private double _stars;
        private DateTime _fetchTime;

        public DateTimeOffset TimeSet { get => _timeSet; set => _timeSet = value; }
        public string Searchstring { get => _searchstring; }
        public double Pp { get => _pp; set => _pp = value; }
        public double Stars { get => _stars; set => _stars = value; }
        public DateTime FetchTime { get => _fetchTime; set => _fetchTime = value; }

        public ShortScore(string searchstring, DateTimeOffset timeSet, double pp)
        {
            this._searchstring = searchstring.ToUpper();
            this._timeSet = timeSet;
            this._pp = pp;
        }

        public ShortScore(string searchstring, double stars, DateTime fetchTime)
        {
            this._searchstring = searchstring.ToUpper();
            this._fetchTime = fetchTime;
            this._stars = stars;
        }
        [JsonConstructor]
        public ShortScore(DateTimeOffset timeSet, string searchstring, double pp, double stars, DateTime fetchTime)
        {
            this._searchstring = searchstring.ToUpper();
            this._timeSet = timeSet;
            this._pp = pp;
            this._fetchTime = fetchTime;
            this._stars = stars;
        }

        public int CompareTo(ShortScore other)
        {
            return other.TimeSet.CompareTo(this.TimeSet);
        }
    }
}
