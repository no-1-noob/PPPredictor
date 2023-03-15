using PPPredictor.Data.Curve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Interfaces
{
    interface IPPPCurve
    {
        bool IsDummy { get; }
        double CalculatePPatPercentage(double star, double percentage, bool failed);
        double CalculateMaxPP(double star);
        CurveInfo ToCurveInfo();
    }
}
