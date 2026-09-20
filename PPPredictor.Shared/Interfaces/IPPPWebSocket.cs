using PPPredictor.Core.DataType;
using PPPredictor.Data;
using System;

namespace PPPredictor.Shared.Interfaces
{
    internal interface IPPPWebSocket
    {
        event EventHandler<PPPScoreSetData> OnScoreSet;
        void StopWebSocket();
    }
}
