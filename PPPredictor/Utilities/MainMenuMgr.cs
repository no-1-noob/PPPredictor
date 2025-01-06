using System;
using Zenject;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberPlaylistsLib.Types;
using PPPredictor.Interfaces;
using SongCore.Utilities;

namespace PPPredictor.Utilities
{
    class MainMenuMgr : IInitializable, IDisposable
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        [Inject] private readonly LevelSelectionNavigationController levelSelectionNavigationController;
        [Inject] private readonly GameplaySetupViewController gameplaySetupViewController;
        [Inject] private readonly IPPPredictorMgr ppPredictorMgr;
        [Inject] private readonly AnnotatedBeatmapLevelCollectionsViewController annotatedBeatmapLevelCollectionsViewController;
        [Inject] private readonly LobbyGameStateController lobbyGameStateController;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null

        private readonly MenuButton menuButton;

        public MainMenuMgr()
        {
            menuButton = new MenuButton("PPPredictor", "Predict PP gains", ProfileInfoMgr.ShowSettingsFlow, true);
        }

        public void Dispose()
        {
            MenuButtons.Instance.UnregisterButton(menuButton);
            levelSelectionNavigationController.didChangeDifficultyBeatmapEvent -= OnDifficultyChanged;
            levelSelectionNavigationController.didChangeLevelDetailContentEvent -= OnDetailContentChanged;
            levelSelectionNavigationController.didActivateEvent -= OnLevelSelectionActivated;
            levelSelectionNavigationController.didDeactivateEvent -= OnLevelSelectionDeactivated;
            gameplaySetupViewController._gameplayModifiersPanelController.didChangeGameplayModifiersEvent -= DidChangeGameplayModifiersEvent;
            annotatedBeatmapLevelCollectionsViewController.didSelectAnnotatedBeatmapLevelCollectionEvent -= AnnotatedBeatmapLevelCollectionsViewController_didSelectAnnotatedBeatmapLevelCollectionEvent;
        }

        public void Initialize()
        {
            MenuButtons.Instance.RegisterButton(menuButton);
            levelSelectionNavigationController.didChangeDifficultyBeatmapEvent += OnDifficultyChanged;
            levelSelectionNavigationController.didChangeLevelDetailContentEvent += OnDetailContentChanged;
            levelSelectionNavigationController.didActivateEvent += OnLevelSelectionActivated;
            levelSelectionNavigationController.didDeactivateEvent += OnLevelSelectionDeactivated;
            gameplaySetupViewController._gameplayModifiersPanelController.didChangeGameplayModifiersEvent += DidChangeGameplayModifiersEvent;
            annotatedBeatmapLevelCollectionsViewController.didSelectAnnotatedBeatmapLevelCollectionEvent += AnnotatedBeatmapLevelCollectionsViewController_didSelectAnnotatedBeatmapLevelCollectionEvent;
        }

        private void DidChangeGameplayModifiersEvent()
        {
            if (IsNormalMainMenu()) this.ppPredictorMgr.ChangeGameplayModifiers(this.gameplaySetupViewController);
        }

        private void OnDifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl)
        {
            if (IsNormalMainMenu()) DiffultyChangedDecideCustomMap(lvlSelectionNavigationCtrl);
        }

        private void OnDetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType)
        {
            if (contentType == StandardLevelDetailViewController.ContentType.OwnedAndReady && IsNormalMainMenu())
            {
                DiffultyChangedDecideCustomMap(lvlSelectionNavigationCtrl);
            }
        }

        private void DiffultyChangedDecideCustomMap(LevelSelectionNavigationController lvlSelectionNavigationCtrl)
        {
            if (!string.IsNullOrEmpty(Hashing.GetCustomLevelHash(lvlSelectionNavigationCtrl.beatmapLevel)) && IsNormalMainMenu()) //Checking if it is a custom map
            {
                this.ppPredictorMgr.DifficultyChanged(lvlSelectionNavigationCtrl.beatmapLevel, lvlSelectionNavigationCtrl.beatmapKey);
            }
            else
            {
                this.ppPredictorMgr.DifficultyChanged(null, lvlSelectionNavigationCtrl.beatmapKey);
            }

        }

        private void OnLevelSelectionActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            this.ppPredictorMgr.ActivateView(IsNormalMainMenu());
        }
        private void OnLevelSelectionDeactivated(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            this.ppPredictorMgr.ActivateView(false);
        }

        private void AnnotatedBeatmapLevelCollectionsViewController_didSelectAnnotatedBeatmapLevelCollectionEvent(BeatmapLevelPack lvlPack)
        {
            this.ppPredictorMgr.FindPoolWithSyncURL((lvlPack as PlaylistLevelPack)?.playlist);
        }

        /// <summary>
        /// Check if we are NOT in a lobby
        /// </summary>
        /// <returns></returns>
        private bool IsNormalMainMenu()
        {
            return lobbyGameStateController.state == MultiplayerLobbyState.None;
        }
    }
}
