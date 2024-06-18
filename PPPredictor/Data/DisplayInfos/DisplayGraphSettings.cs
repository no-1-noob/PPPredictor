using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Data.DisplayInfos
{
    internal class DisplayGraphSettings
    {
        private double _minX = 0;
        private double _maxX = 100;
        private double _minY = 0;
        private double _maxY = 299;
        private double stepSize = .1f;


        public double MinX { get => _minX; set => _minX = value; }
        public double MaxX { get => _maxX; set => _maxX = value; }
        public double MinY { get => _minY; set => _minY = value; }
        public double MaxY { get => _maxY; set => _maxY = value; }
        public double StepSize { get => stepSize; set => stepSize = value; }
        public DisplayGraphSettings(double minX, double maxX, double minY, double maxY, double stepSize)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
            this.stepSize = stepSize;
        }
    }
}
