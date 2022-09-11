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
        private List<IPPPredictor> _lsPPPredictor;
        private int index = 0;
        public IPPPredictor CurrentPPPredictor;

        public PPPredictorMgr()
        {
            _lsPPPredictor = new List<IPPPredictor>();
            _lsPPPredictor.Add(new PPPredictor<PPCalculatorScoreSaber>(Leaderboard.ScoreSaber));
            _lsPPPredictor.Add(new PPPredictor<PPCalculatorBeatLeader>(Leaderboard.BeatLeader));
            if (_lsPPPredictor.Count > 0)
            {
                CurrentPPPredictor = _lsPPPredictor.Where(x => x.LeaderBoardName == Plugin.ProfileInfo.LastLeaderBoardSelected).FirstOrDefault();
                CurrentPPPredictor.SetActive(true);
            }
        }

        public void CyclePredictors()
        {
            _lsPPPredictor.ForEach(item => item.SetActive(false));
            index = (index + 1) % _lsPPPredictor.Count();
            CurrentPPPredictor = _lsPPPredictor[index];
            CurrentPPPredictor.SetActive(true);
            Plugin.ProfileInfo.LastLeaderBoardSelected = CurrentPPPredictor.LeaderBoardName;
            //TODO: Display Name
            Plugin.Log?.Error($"CyclePredictors {CurrentPPPredictor}");
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
    }
}
