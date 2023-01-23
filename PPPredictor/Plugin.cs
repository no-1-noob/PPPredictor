using IPA;
using PPPredictor.Data;
using PPPredictor.Installers;
using PPPredictor.UI.ViewController;
using PPPredictor.Utilities;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace PPPredictor
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        internal static ProfileInfo ProfileInfo;

        internal static PPPredictorViewController pppViewController;

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
            ProfileInfo = ProfileInfoMgr.LoadProfileInfo();
            zenjector.UseSiraSync();
            zenjector.Install<PPPPredictorDisplayInstaller>(Location.Menu);
            zenjector.Install<MainMenuInstaller>(Location.Menu);
            zenjector.Install<CoreInstaller>(Location.App);
        }

        [OnStart]
        public void OnApplicationStart()
        {
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            ProfileInfoMgr.SaveProfile(ProfileInfo);
        }
    }
}
