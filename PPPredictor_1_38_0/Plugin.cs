using PPPredictor.Shared;
namespace PPPredictor
{
    
    class Plugin :  PluginBase
    {
        internal static string Beta = string.Empty;
        internal static string BeatSaberVersion = "1_38";
        internal override void CreateConverter()
        {
            Converter = new Converter.Converter();
        }
    }
}
