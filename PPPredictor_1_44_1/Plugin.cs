using IPA;
using IPA.Logging;
using SiraUtil.Zenject;
namespace PPPredictor
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin :  PPPredictor.Shared.PluginBase
    {
        internal static string Beta = string.Empty;
        internal static string BeatSaberVersion = "1_44_1";
        
        internal override void CreateConverter()
        {
            Converter = new Converter.Converter();
        }
        
        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public Plugin(Logger logger, Zenjector zenjector) : base(logger, zenjector)
        {
        }

        [OnStart]
        public new void OnApplicationStart()
        {
            base.OnApplicationStart();
        }
        
        [OnExit]
        public new void OnApplicationQuit()
        {
            base.OnApplicationQuit();
        }

        [OnDisable]
        public new void OnDisable()
        {
            base.OnDisable();
        }
    }
}
