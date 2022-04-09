using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor
{
    public class ShortScore : IComparable<ShortScore>
    {
        private DateTimeOffset timeSet;
        private double maxScore;
        private double modifiedScore;
        private string searchstring;
        private double pp;

        public DateTimeOffset TimeSet { get => timeSet; set => timeSet = value; }
        public double MaxScore { get => maxScore; set => maxScore = value; }
        public double ModifiedScore { get => modifiedScore; set => modifiedScore = value; }
        public string Searchstring { get => searchstring; set => searchstring = value; }
        public double Pp { get => pp; set => pp = value; }

        public ShortScore(string searchstring, DateTimeOffset timeSet, double maxScore, double modifiedScore, double pp)
        {
            this.searchstring = searchstring;
            this.timeSet = timeSet;
            this.maxScore = maxScore;
            this.modifiedScore = modifiedScore;
            this.pp = pp;
        }

        public int CompareTo(ShortScore other)
        {
            return other.TimeSet.CompareTo(this.TimeSet);
        }
    }
}
