using PPPredictor.Data.DisplayInfos;
using PPPredictor.Interfaces;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Data.Curve
{
    abstract class BaseCurve : IPPPCurve
    {
        public abstract bool IsDummy { get; }

        public abstract double CalculateMaxPP(PPPBeatMapInfo _currentBeatMapInfo, LeaderboardContext leaderboardContext = LeaderboardContext.None);
        public abstract double CalculatePPatPercentage(PPPBeatMapInfo _currentBeatMapInfo, double percentage, bool failed, bool paused, LeaderboardContext leaderboardContext = LeaderboardContext.None);
        public abstract CurveInfo ToCurveInfo();

        public DisplayGraphData DisplayGraphData(PPPBeatMapInfo _currentBeatMapInfo, DisplayGraphSettings displayGraphSettings, LeaderboardContext leaderboardContext = LeaderboardContext.None)
        {
            Plugin.DebugPrint("Calculate DisplayGraphData");
            DisplayGraphData displayGraphData = new DisplayGraphData(displayGraphSettings);
            double xPos = displayGraphSettings.MinX;
            while(xPos <= displayGraphSettings.MaxX)
            {
                double yPos = CalculatePPatPercentage(_currentBeatMapInfo, xPos, false, false, leaderboardContext);
                xPos += displayGraphSettings.StepSize;
                displayGraphData.LsPoints.Add(new GraphPoint(xPos, yPos));
            }
            return displayGraphData;
        }
    }
}
