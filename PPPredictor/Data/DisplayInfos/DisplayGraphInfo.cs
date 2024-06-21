using System.Collections.Generic;

namespace PPPredictor.Data.DisplayInfos
{
    internal class DisplayGraphInfo
    {
        private List<GraphPoint> _lsPoints = new List<GraphPoint>();
        private DisplayGraphSettings displayGraphSettings;

        public List<GraphPoint> LsPoints { get => _lsPoints; set => _lsPoints = value; }
        public DisplayGraphSettings DisplayGraphSettings { get => displayGraphSettings; set => displayGraphSettings = value; }

        public DisplayGraphInfo(DisplayGraphSettings displayGraphSettings)
        {
            this.displayGraphSettings = displayGraphSettings;
        }
    }

    internal class GraphPoint
    {
        double x, y;


        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public GraphPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
