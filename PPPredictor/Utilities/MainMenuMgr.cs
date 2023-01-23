using System;
using Zenject;

namespace PPPredictor.Utilities
{
    class MainMenuMgr : IInitializable, IDisposable
    {
        [Inject] private readonly LevelSelectionNavigationController levelSelectionNavigationController;
        [Inject] private readonly GameplaySetupViewController gameplaySetupViewController;
        [Inject] private readonly PPPredictorMgr ppPredictorMgr;

        public MainMenuMgr()
        {
        }

        public void Dispose()
        {
            levelSelectionNavigationController.didChangeDifficultyBeatmapEvent -= OnDifficultyChanged;
            levelSelectionNavigationController.didChangeLevelDetailContentEvent -= OnDetailContentChanged;
            levelSelectionNavigationController.didActivateEvent -= OnLevelSelectionActivated;
            levelSelectionNavigationController.didDeactivateEvent -= OnLevelSelectionDeactivated;
            gameplaySetupViewController.didChangeGameplayModifiersEvent -= DidChangeGameplayModifiersEvent;
        }

        public void Initialize()
        {
            levelSelectionNavigationController.didChangeDifficultyBeatmapEvent += OnDifficultyChanged;
            levelSelectionNavigationController.didChangeLevelDetailContentEvent += OnDetailContentChanged;
            levelSelectionNavigationController.didActivateEvent += OnLevelSelectionActivated;
            levelSelectionNavigationController.didDeactivateEvent += OnLevelSelectionDeactivated;
            gameplaySetupViewController.didChangeGameplayModifiersEvent += DidChangeGameplayModifiersEvent;
        }

        private void DidChangeGameplayModifiersEvent()
        {
            this.ppPredictorMgr.ChangeGameplayModifiers(this.gameplaySetupViewController);
        }

        private void OnDifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            this.ppPredictorMgr.DifficultyChanged(lvlSelectionNavigationCtrl, beatmap);
        }

        private void OnDetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType)
        {
            this.ppPredictorMgr.DetailContentChanged(lvlSelectionNavigationCtrl, contentType);
        }

        private void OnLevelSelectionActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            this.ppPredictorMgr.ActivateView(true);
        }
        private void OnLevelSelectionDeactivated(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            this.ppPredictorMgr.ActivateView(false);
        }
    }
}
