using IPA;
using SiraUtil.Zenject;
using PPPredictor.UI.ViewController;
using IPALogger = IPA.Logging.Logger;

namespace PPPredictor
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        public static ProfileInfo ProfileInfo;

        public static PPPredictorViewController pppViewController;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            ProfileInfo = ProfileInfoMgr.loadProfileInfo();
            zenjector.UseSiraSync();
            zenjector.Install<PPPPredictorDisplayInstaller>(Location.Menu);
            BeatSaberMarkupLanguage.Settings.BSMLSettings.instance.AddSettingsMenu("PPPredictor", "PPPredictor.UI.Views.PPPredictorSettingsView.bsml", new PPPredictorSettingsViewController());
            Log.Info("PPPredictor initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh += (o) =>
            {
                //new GameObject("PPPredictorController").AddComponent<PPPredictorController>();
            };
            

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");
            ProfileInfoMgr.SaveProfile(ProfileInfo);
        }
    }
}
