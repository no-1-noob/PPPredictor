using IPA;
using PPPredictor.Data;
using PPPredictor.Installers;
using PPPredictor.UI.ViewController;
using PPPredictor.Utilities;
using SiraUtil.Zenject;
using System.Threading.Tasks;
using IPALogger = IPA.Logging.Logger;

namespace PPPredictor
{
    [Plugin(RuntimeOptions.DynamicInit)]
    class Plugin
    {
        internal static string Beta = string.Empty;
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        internal static ProfileInfo ProfileInfo;

        internal static PPPredictorViewController pppViewController;

        //Only Used for UnitTests
        internal Plugin()
        {
            Instance = this;
            ProfileInfo = new ProfileInfo();
        }

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
            zenjector.Install<GamePlayInstaller>(Location.StandardPlayer | Location.CampaignPlayer);
        }

        [OnStart]
        public void OnApplicationStart()
        {
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            ProfileInfoMgr.SaveProfile(ProfileInfo, PPPredictorMgr.instance.GetSaveData());
        }

        public static void ErrorPrint(string text)
        {
            Plugin.Log?.Error(text);
        }

        public static void DebugPrint(string text)
        {
            Plugin.Log?.Error(text);
        }

        public static void DebugNetworkPrint(string text)
        {
            Plugin.Log?.Error(text);
        }

        internal static async Task<UserInfo> GetUserInfoBS()
        {
            return await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
        }
    }
}
