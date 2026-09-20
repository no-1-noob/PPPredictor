using System;
namespace PPPredictor.Shared.Interfaces
{
    public interface IPluginLog
    {
        void Error(string message);
        void Warn(string message);
        void Warn(Exception ex);
        void Debug(string message);
        void Debug(Exception ex);
    }
}