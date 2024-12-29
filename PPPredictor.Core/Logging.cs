using System;

namespace PPPredictor.Core
{
    public class Logging
    {
        public static event EventHandler<LoggingMessage> OnMessage;

        internal static void ErrorPrint(string message)
        {
            OnMessage?.Invoke(null, new LoggingMessage(LoggingMessage.LoggingType.Error, message));
        }

        internal static void DebugNetworkPrint(string message)
        {
            OnMessage?.Invoke(null, new LoggingMessage(LoggingMessage.LoggingType.DebugNetworkPrint, message));
        }
    }

    public class LoggingMessage
    {
        public LoggingType loggingType;
        public string message;

        public LoggingMessage(LoggingType loggingType, string message)
        {
            this.loggingType = loggingType;
            this.message = message;
        }

        public enum LoggingType
        {
            Error,
            DebugNetworkPrint
        }
    }
}
