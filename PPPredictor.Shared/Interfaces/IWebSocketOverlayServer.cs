namespace PPPredictor.Shared.Interfaces
{
    public interface IWebSocketOverlayServer
    {
        void StartSocket();
        void CloseSocket();
        void SendData(string s);
    }
}
