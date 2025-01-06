using System;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core
{
    internal class Logging
    {
        public static event EventHandler<LoggingMessage> OnMessage;

        internal static void ErrorPrint(string message)
        {
            OnMessage?.Invoke(null, new LoggingMessage(LoggingMessage.LoggingType.Error, message));
        }

        internal static void DebugNetworkPrint(string message, Leaderboard leaderboard)
        {
            OnMessage?.Invoke(null, new LoggingMessage(LoggingMessage.LoggingType.DebugNetworkPrint, leaderboard, message));
        }
    }

    public class LoggingMessage
    {
        public LoggingType loggingType;
        public string message;
        public Leaderboard leaderboard;

        public LoggingMessage(LoggingType loggingType, string message)
        {
            this.loggingType = loggingType;
            this.message = message;
            this.leaderboard = Leaderboard.NoLeaderboard;
        }

        public LoggingMessage(LoggingType loggingType, Leaderboard leaderboard, string message)
        {
            this.loggingType = loggingType;
            this.message = message;
            this.leaderboard = leaderboard;
        }

        public enum LoggingType
        {
            Error,
            DebugNetworkPrint
        }
    }
}
