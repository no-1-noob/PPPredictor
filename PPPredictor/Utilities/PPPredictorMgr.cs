﻿using BeatSaberMarkupLanguage.FloatingScreen;
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
        private readonly List<IPPPredictor> _lsPPPredictor;
        private int index = 0;
        public IPPPredictor CurrentPPPredictor;
        private PropertyChangedEventHandler propertyChanged;
        private bool isLeftArrowActive = false;
        private bool isRightArrowActive = false;
        private bool isLeaderboardNavigationActive = false;

        public bool IsLeftArrowActive { get => isLeftArrowActive; }
        public bool IsRightArrowActive { get => isRightArrowActive; }
        public bool IsLeaderboardNavigationActive { get => isLeaderboardNavigationActive; }

        public PPPredictorMgr()
        {
            _lsPPPredictor = new List<IPPPredictor>();
            if (Plugin.ProfileInfo.IsScoreSaberEnabled) _lsPPPredictor.Add(new PPPredictor<PPCalculatorScoreSaber>(Leaderboard.ScoreSaber));
            if (Plugin.ProfileInfo.IsBeatLeaderEnabled) _lsPPPredictor.Add(new PPPredictor<PPCalculatorBeatLeader>(Leaderboard.BeatLeader));
            if (_lsPPPredictor.Count == 0)
            {
                _lsPPPredictor.Add(new PPPredictor<PPCalculatorNoLeaderboard>(Leaderboard.NoLeaderboard));
            }
            index = _lsPPPredictor.FindIndex(x => x.LeaderBoardName == Plugin.ProfileInfo.LastLeaderBoardSelected);
            if(index >= 0)
            {
                CurrentPPPredictor = _lsPPPredictor[index];
            }
            else
            {
                CurrentPPPredictor = _lsPPPredictor[0];
            }
            CurrentPPPredictor.SetActive(true);
            SetNavigationArrowInteractivity();
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
            if (_lsPPPredictor.Count() == 1) isLeaderboardNavigationActive = false;
            else isLeaderboardNavigationActive = true;
            isLeftArrowActive = index > 0;
            isRightArrowActive = index < _lsPPPredictor.Count() - 1;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLeftArrowActive)));
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRightArrowActive)));
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLeaderboardNavigationActive)));
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

        internal double GetPercentage()
        {
            foreach (var item in _lsPPPredictor)
            {
                return item.Percentage;
            }
            return 0;
        }

        internal void ResetDisplay(bool resetAll)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.ResetDisplay(resetAll);
            }
        }

        internal double GetPPAtPercentageForCalculator(Leaderboard leaderBoardName, double percentage)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.CalculatePPatPercentage(percentage);
            }
            return 0;
        }

        internal bool IsRanked(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.IsRanked();
            }
            return false;
        }

        internal double GetPPGainForCalculator(Leaderboard leaderBoardName, double pp)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.CalculatePPGain(pp);
            }
            return 0;
        }
    }
}
