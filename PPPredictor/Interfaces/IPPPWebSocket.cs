using PPPredictor.Core.DataType;
using System;

namespace PPPredictor.Interfaces
{
    internal interface IPPPWebSocket
    {
        event EventHandler<PPPScoreSetData> OnScoreSet;
        void StopWebSocket();
    }
}
