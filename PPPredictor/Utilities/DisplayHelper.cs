using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    class DisplayHelper
    {
        public static string GetDisplayColor(double value, bool invert)
        {
            if (invert) value *= -1;
            if (value > 0)
            {
                return $"green";
            }
            else if (value < 0)
            {
                return $"red";
            }
            return "white";
        }
    }
}
