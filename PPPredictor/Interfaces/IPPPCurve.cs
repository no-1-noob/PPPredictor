using PPPredictor.Data.Curve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Interfaces
{
    public interface IPPPCurve
    {
        bool isDummy { get; }
        double CalculatePPatPercentage(double star, double percentage);
        CurveInfo ToCurveInfo();
    }
}
