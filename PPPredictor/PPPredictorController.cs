using PPPredictor.Utilities;
using BS_Utils.Utilities;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PPPredictor
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class PPPredictorController : MonoBehaviour
    {
        public static PPPredictorController Instance { get; private set; }
        private PPPDisplay _pppDisplay;

        // These methods are automatically called by Unity, you should remove any you aren't using.
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
            //double pp = PPCalculator.calculatePPForBeatmapAsync(379720, 94.8554477546, Plugin.Log).GetAwaiter().GetResult();
            //Plugin.Log?.Debug($"PP: {pp}");
        }
        /// <summary>
        /// Only ever called once on the first frame the script is Enabled. Start is called after any other script's Awake() and before Update().
        /// </summary>
        private void Start()
        {
            var soloButton = Resources.FindObjectsOfTypeAll<Button>().First(x => x.name == "SoloButton");
            soloButton.onClick.AddListener(InitializeSolo);
            _pppDisplay = PPPDisplay.Create();
        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// Called every frame after every other enabled script's Update().
        /// </summary>
        private void LateUpdate()
        {

        }

        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable()
        {

        }

        /// <summary>
        /// Called when the script becomes disabled or when it is being destroyed.
        /// </summary>
        private void OnDisable()
        {

        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion

        void InitializeSolo()
        {
            var _flowCoordinator = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().LastOrDefault();
            var levelSelectionNavController = _flowCoordinator?.GetPrivateField<LevelSelectionNavigationController>("levelSelectionNavigationController");
            levelSelectionNavController.didChangeDifficultyBeatmapEvent += OnDifficultyChanged;
        }
        private async void OnDifficultyChanged(LevelSelectionNavigationController _, IDifficultyBeatmap beatmap)
        {
            Plugin.Log?.Info($"DifficultyChanged: {beatmap}");
            Plugin.Log?.Info($"{beatmap.level.levelID}, {beatmap.difficultyRank}");
            if (beatmap.level.levelID.StartsWith("custom_level_"))
            {
                string hash = beatmap.level.levelID.Replace("custom_level_", "");
                Plugin.Log?.Info($"hash: {hash}");
                double pp = await PPCalculator.calculateBasePPForBeatmapAsync(beatmap);
                Plugin.Log?.Info($"PP: {pp}");
                _pppDisplay.showMessage($"PP: { pp}");

            }
        }
    }
}
