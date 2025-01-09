using HarmonyLib;
using IPA;
using PPPredictor.Data;
using PPPredictor.Installers;
using PPPredictor.UI.ViewController;
using PPPredictor.Utilities;
using SiraUtil.Zenject;
using System.Reflection;
using System;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;
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

        private const string kHarmonyID = "com.github.no-1-noob.PPPredictor";
        private static readonly Harmony harmony = new Harmony(kHarmonyID);

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
            ApplyHarmonyPatches();
        }

        private static void ApplyHarmonyPatches()
        {
            try
            {
                Log?.Debug("Applying Harmony patches.");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Log?.Error("Error applying Harmony patches: " + ex.Message);
                Log?.Debug(ex);
            }
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            ProfileInfoMgr.SaveProfile(ProfileInfo, PPPredictorMgr.CalculatorInstance.GetSaveData());
        }

        [OnDisable]
        public void OnDisable()
        {
            RemoveHarmonyPatches();
        }

        private static void RemoveHarmonyPatches()
        {
            try
            {
                harmony.UnpatchSelf();
            }
            catch (Exception ex)
            {
                Log?.Error("Error removing Harmony patches: " + ex.Message);
                Log?.Debug(ex);
            }
        }

        public static void ErrorPrint(string text)
        {
            Plugin.Log?.Error(text);
        }

        public static void DebugPrint(string text)
        {
            Plugin.Log?.Error(text);
        }

        public static void DebugNetworkPrint(string text, Leaderboard leaderBoard)
        {

            switch (leaderBoard)
            {
#if SCORESABERNETWORK
                case Leaderboard.ScoreSaber:
                    break;
#endif
#if BEATLEADERNETWORK
                case Leaderboard.BeatLeader:
                    break;
#endif
#if ACCSABERNETWORK
                case Leaderboard.AccSaber:
                    break;
#endif
#if HITBLOQNETWORK
                case Leaderboard.HitBloq:
                    break;
#endif
                default:
                    return;
            }
            Plugin.Log?.Error(text);
        }

        internal static async Task<UserInfo> GetUserInfoBS()
        {
            return await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
        }
    }
}
