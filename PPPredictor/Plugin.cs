using IPA;
using IPA.Config;
using IPA.Config.Stores;
using PPPredictor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace PPPredictor
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        public static ProfileInfo ProfileInfo;

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
