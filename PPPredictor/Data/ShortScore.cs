using System;

namespace PPPredictor.Data
{
    public class ShortScore : IComparable<ShortScore>
    {
        private DateTimeOffset _timeSet;
        private readonly string _searchstring;
        private double _pp;

        public DateTimeOffset TimeSet { get => _timeSet; set => _timeSet = value; }
        public string Searchstring { get => _searchstring; }
        public double Pp { get => _pp; set => _pp = value; }

        public ShortScore(string searchstring, DateTimeOffset timeSet, double pp)
        {
            this._searchstring = searchstring.ToUpper();
            this._timeSet = timeSet;
            this._pp = pp;
        }

        public int CompareTo(ShortScore other)
        {
            return other.TimeSet.CompareTo(this.TimeSet);
        }
    }
}
