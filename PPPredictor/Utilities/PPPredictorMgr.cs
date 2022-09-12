using BeatSaberMarkupLanguage.FloatingScreen;
using PPPredictor.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PPPredictor.Utilities
{
    class PPPredictorMgr
    {
        private List<IPPPredictor> _lsPPPredictor;
        private int index = 0;
        public IPPPredictor CurrentPPPredictor;
        private PropertyChangedEventHandler propertyChanged;
        private bool isLeftArrowActive = false;
        private bool isRightArrowActive = false;

        public bool IsLeftArrowActive { get => isLeftArrowActive; }
        public bool IsRightArrowActive { get => isRightArrowActive; }

        public PPPredictorMgr()
        {
            _lsPPPredictor = new List<IPPPredictor>();
            _lsPPPredictor.Add(new PPPredictor<PPCalculatorScoreSaber>(Leaderboard.ScoreSaber));
            _lsPPPredictor.Add(new PPPredictor<PPCalculatorBeatLeader>(Leaderboard.BeatLeader));
            if (_lsPPPredictor.Count > 0)
            {
                index = _lsPPPredictor.FindIndex(x => x.LeaderBoardName == Plugin.ProfileInfo.LastLeaderBoardSelected);
                if(index >= 0)
                {
                    CurrentPPPredictor = _lsPPPredictor[index];
                    CurrentPPPredictor.SetActive(true);
                    SetNavigationArrowInteractivity();
                }
            }
        }

        public void CyclePredictors(int offset)
        {
            _lsPPPredictor.ForEach(item => item.SetActive(false));
            index = Math.Min(Math.Max((index + offset), 0), _lsPPPredictor.Count() - 1);
            CurrentPPPredictor = _lsPPPredictor[index];
            CurrentPPPredictor.SetActive(true);
            Plugin.ProfileInfo.LastLeaderBoardSelected = CurrentPPPredictor.LeaderBoardName;
            SetNavigationArrowInteractivity();
            //TODO: Reload also when scoresaber doesnt upload...
            //TODO: Beatleader played score reload is sometimes slow
        }

        private void SetNavigationArrowInteractivity()
        {
            isLeftArrowActive = index > 0;
            isRightArrowActive = index < _lsPPPredictor.Count() - 1;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLeftArrowActive)));
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRightArrowActive)));
        }

        public void ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.ChangeGameplayModifiers(gameplaySetupViewController);
            }
        }

        public void DetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.DetailContentChanged(lvlSelectionNavigationCtrl, contentType);
            }
        }

        public void DifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.DifficultyChanged(lvlSelectionNavigationCtrl, beatmap);
            }
        }

        internal void SetPropertyChangedEventHandler(PropertyChangedEventHandler propertyChanged)
        {
            this.propertyChanged = propertyChanged;
            foreach (var item in _lsPPPredictor)
            {
                item.SetPropertyChangedEventHandler(propertyChanged);
            }
        }

        internal void UpdateCurrentAndCheckResetSession(bool v)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.UpdateCurrentAndCheckResetSession(v);
            }
        }

        internal void RefreshCurrentData(int v)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.RefreshCurrentData(v);
            }
        }

        internal void SetPercentage(float percentage)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.Percentage = percentage;
            }
        }
    }
}
