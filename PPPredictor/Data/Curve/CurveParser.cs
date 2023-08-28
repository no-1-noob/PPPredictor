using PPPredictor.Interfaces;
using System.Collections.Generic;

namespace PPPredictor.Data.Curve
{
    class CurveParser
    {
        private static readonly double basePPMultiplierScoreSaber = 42.117208413;
        private static readonly List<(double, double)> arrPPCurveScoreSaber = new List<(double, double)>(new (double, double)[37] {
            (1.0, 5.367394282890631),
            (0.9995, 5.019543595874787),
            (0.999, 4.715470646416203),
            (0.99825, 4.325027383589547),
            (0.9975, 3.996793606763322),
            (0.99625, 3.5526145337555373),
            (0.995, 3.2022017597337955),
            (0.99375, 2.9190155639254955),
            (0.9925, 2.685667856592722),
            (0.99125, 2.4902905794106913),
            (0.99, 2.324506282149922),
            (0.9875, 2.058947159052738),
            (0.985, 1.8563887693647105),
            (0.9825, 1.697536248647543),
            (0.98, 1.5702410055532239),
            (0.9775, 1.4664726399289512),
            (0.975, 1.3807102743105126),
            (0.9725, 1.3090333065057616),
            (0.97, 1.2485807759957321),
            (0.965, 1.1552120359501035),
            (0.96, 1.0871883573850478),
            (0.955, 1.0388633331418984),
            (0.95, 1.0),
            (0.94, 0.9417362980580238),
            (0.93, 0.9039994071865736),
            (0.92, 0.8728710341448851),
            (0.91, 0.8488375988124467),
            (0.9, 0.825756123560842),
            (0.875, 0.7816934560296046),
            (0.85, 0.7462290664143185),
            (0.825, 0.7150465663454271),
            (0.8, 0.6872268862950283),
            (0.75, 0.6451808210101443),
            (0.7, 0.6125565959114954),
            (0.65, 0.5866010012767576),
            (0.6, 0.18223233667439062),
            (0.0, 0.0) });
        public static IPPPCurve ParseToCurve(CurveInfo curveInfo)
        {
            switch (curveInfo.CurveType)
            {
                case Utilities.CurveType.ScoreSaber:
                    return new CustomPPPCurve(arrPPCurveScoreSaber, Utilities.CurveType.ScoreSaber, basePPMultiplierScoreSaber);
                case Utilities.CurveType.BeatLeader:
                    return new BeatLeaderPPPCurve();
                case Utilities.CurveType.Linear:
                    return new CustomPPPCurve(curveInfo.ArrPPCurve, Utilities.CurveType.Linear, curveInfo.BasePPMultiplier.GetValueOrDefault());
                case Utilities.CurveType.Basic:
                    return CustomPPPCurve.CreateBasicPPPCurve(curveInfo.BasePPMultiplier.GetValueOrDefault(), curveInfo.Baseline, curveInfo.Exponential, curveInfo.Cutoff);
                default:
                    return CustomPPPCurve.CreateDummyPPPCurve();
            }
        }
    }
}
