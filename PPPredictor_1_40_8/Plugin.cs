namespace PPPredictor
{
    class Plugin :  PPPredictor.Shared.PluginBase
    {
        internal static string Beta = string.Empty;
        internal static string BeatSaberVersion = "1_40";
        
        internal override void CreateConverter()
        {
            Converter = new Converter.Converter();
        }
    }
}
