﻿using PPPredictor.Data;
using PPPredictor.Data.Curve;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Utilities;
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
        double CalculatePPatPercentage(PPPBeatMapInfo _currentBeatMapInfo, double percentage, bool failed, bool paused, LeaderboardContext leaderboardContext = LeaderboardContext.None);
        double CalculateMaxPP(PPPBeatMapInfo _currentBeatMapInfo, LeaderboardContext leaderboardContext = LeaderboardContext.None);
        CurveInfo ToCurveInfo();
        DisplayGraphData DisplayGraphData(PPPBeatMapInfo _currentBeatMapInfo, DisplayGraphSettings displayGraphSettings, LeaderboardContext leaderboardContext = LeaderboardContext.None);
    }
}
