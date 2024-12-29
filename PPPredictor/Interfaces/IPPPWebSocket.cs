using PPPredictor.Core.DataType;
using PPPredictor.Data;
using System;

namespace PPPredictor.Interfaces
{
    internal interface IPPPWebSocket
    {
        event EventHandler<PPPScoreSetData> OnScoreSet;
        void StopWebSocket();
    }
}
