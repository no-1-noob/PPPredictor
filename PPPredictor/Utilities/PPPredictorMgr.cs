using BeatSaberMarkupLanguage.FloatingScreen;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace PPPredictor.Utilities
{
    class PPPredictorMgr : IInitializable, IDisposable
    {

        private List<IPPPredictor> _lsPPPredictor;
        private int index = 0;
        internal IPPPredictor CurrentPPPredictor;
        private PropertyChangedEventHandler propertyChanged;
        private bool isLeftArrowActive = false;
        private bool isRightArrowActive = false;
        private bool isMapPoolDropDownActive = true;
        private bool isLeaderboardNavigationActive = false;

        public event EventHandler<bool> ViewActivated;
        public event EventHandler<bool> OnDataLoading;
        public event EventHandler<DisplaySessionInfo> OnDisplaySessionInfo;
        public event EventHandler<DisplayPPInfo> OnDisplayPPInfo;
        public event EventHandler OnMapPoolRefreshed;

        internal bool IsLeftArrowActive { get => isLeftArrowActive; }
        internal bool IsRightArrowActive { get => isRightArrowActive; }
        internal bool IsMapPoolDropDownActive { get => isMapPoolDropDownActive; }
        internal bool IsLeaderboardNavigationActive { get => isLeaderboardNavigationActive; }

        internal PPPredictorMgr()
        {
            ResetPredictors();
        }

        internal void ResetPredictors()
        {
            _lsPPPredictor = new List<IPPPredictor>();
            CurrentPPPredictor = null;
            if (Plugin.ProfileInfo.IsScoreSaberEnabled) _lsPPPredictor.Add(new PPPredictor<PPCalculatorScoreSaber>(Leaderboard.ScoreSaber));
            if (Plugin.ProfileInfo.IsBeatLeaderEnabled) _lsPPPredictor.Add(new PPPredictor<PPCalculatorBeatLeader>(Leaderboard.BeatLeader));
            if (Plugin.ProfileInfo.IsHitBloqEnabled) _lsPPPredictor.Add(new PPPredictor<PPCalculatorHitBloq>(Leaderboard.HitBloq));
            if (_lsPPPredictor.Count == 0)
            {
                _lsPPPredictor.Add(new PPPredictor<PPCalculatorNoLeaderboard>(Leaderboard.NoLeaderboard));
            }
            index = _lsPPPredictor.FindIndex(x => x.LeaderBoardName == Plugin.ProfileInfo.LastLeaderBoardSelected);
            if (index >= 0)
            {
                CurrentPPPredictor = _lsPPPredictor[index];
            }
            else
            {
                CurrentPPPredictor = _lsPPPredictor[0];
            }
            foreach (IPPPredictor pPPredictor in _lsPPPredictor)
            {
                pPPredictor.OnDataLoading += PPPredictor_OnDataLoading;
                pPPredictor.OnDisplayPPInfo += PPPredictor_OnDisplayPPInfo;
                pPPredictor.OnDisplaySessionInfo += PPPredictor_OnDisplaySessionInfo;
                pPPredictor.OnMapPoolRefreshed += PPPredictor_OnMapPoolRefreshed;
            }
            CurrentPPPredictor.SetActive(true);
            SetNavigationArrowInteractivity();
        }

        private void PPPredictor_OnMapPoolRefreshed(object sender, EventArgs e)
        {
            OnMapPoolRefreshed?.Invoke(this, null);
        }

        #region event handler
        private void PPPredictor_OnDisplaySessionInfo(object sender, DisplaySessionInfo displaySessionInfo)
        {
            OnDisplaySessionInfo?.Invoke(this, displaySessionInfo);
        }

        private void PPPredictor_OnDisplayPPInfo(object sender, DisplayPPInfo displayPPInfo)
        {
            OnDisplayPPInfo?.Invoke(this, displayPPInfo);
        }

        private void PPPredictor_OnDataLoading(object sender, bool isDataLoading)
        {
            OnDataLoading?.Invoke(this, isDataLoading);
        }
        #endregion

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
            isMapPoolDropDownActive = CurrentPPPredictor.MapPoolOptions.Count() > 1;
            CurrentPPPredictor.CalculatePP();
            OnMapPoolRefreshed?.Invoke(this, null);
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

        internal void UpdateCurrentAndCheckResetSession(bool v)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.UpdateCurrentAndCheckResetSession(v);
            }
        }

        internal void RefreshCurrentData(int v, bool refreshStars = false)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.RefreshCurrentData(v, refreshStars);
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

        internal double GetPPAtPercentageForCalculator(Leaderboard leaderBoardName, double percentage, bool levelFailed, double stars, GameplayModifiers gameplayModifiers)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.CalculatePPatPercentage(stars, percentage, gameplayModifiers, levelFailed);
            }
            return 0;
        }
        internal string GetPPSuffixForLeaderboard(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.PPSuffix;
            }
            return string.Empty;
        }

        internal double GetMaxPPForCalculator(Leaderboard leaderBoardName, double stars, GameplayModifiers gameplayModifiers)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.CalculateMaxPP(stars, gameplayModifiers);
            }
            return 0;
        }

        internal double GetModifiedStarsForCalculator(Leaderboard leaderBoardName, GameplayModifiers gameplayModifiers)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.GetModifiedStars(gameplayModifiers);
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

        internal string GetLeaderboardIcon(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.LeaderBoardIcon;
            }
            return string.Empty;
        }
        internal string GetMapPoolIcon(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.MapPoolIcon;
            }
            return string.Empty;
        }

        internal async Task<byte[]> GetLeaderboardIconData(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                if(predictor.MapPoolIconData == null)
                {
                    await predictor.GetMapPoolIconData();
                }
                return predictor.MapPoolIconData;
            }
            return null;
        }

        internal void ActivateView(bool activate)
        {
            Plugin.Log?.Error($"ActivateView '{activate}'");
            ViewActivated?.Invoke(this, activate);
        }

        internal List<object> GetMapPoolsFromLeaderboard(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.MapPoolOptions;
            }
            return new List<object>();
        }

        public void Initialize()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (IPPPredictor pPPredictor in _lsPPPredictor)
            {
                pPPredictor.OnDataLoading -= PPPredictor_OnDataLoading;
                pPPredictor.OnDisplayPPInfo -= PPPredictor_OnDisplayPPInfo;
                pPPredictor.OnDisplaySessionInfo -= PPPredictor_OnDisplaySessionInfo;
                pPPredictor.OnMapPoolRefreshed -= PPPredictor_OnMapPoolRefreshed;
            }
        }
    }
}
