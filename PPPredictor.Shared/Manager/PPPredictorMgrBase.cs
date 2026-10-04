using PPPredictor.Core;
using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Shared.Interfaces;
using PPPredictor.Shared.Predictor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Shared.Manager
{
    public abstract class PPPredictorMgrBase

    {
        internal readonly WebSocketMgrBase _websocketMgr;
        private const string _beatleaderSyncUrlIdentifier = ".beatleader.";
        internal List<IPPPredictor> _lsPPPredictor;
        private int index = 0;
        private IPPPredictor _currentPPPredictor;
        private bool isLeftArrowActive = false;
        private bool isRightArrowActive = false;
        private bool isMapPoolDropDownActive = true;
        private bool isLeaderboardNavigationActive = false;
        private int _loadingCounter = 0;

        public event EventHandler<bool> ViewActivated;
        internal event EventHandler<bool> OnDataLoading;
        internal  event EventHandler<DisplaySessionInfo> OnDisplaySessionInfo;
        internal  event EventHandler<DisplayPPInfo> OnDisplayPPInfo;
        internal  event EventHandler OnMapPoolRefreshed;

        public IPPPredictor CurrentPPPredictor { get => _currentPPPredictor; }
        public bool IsLeftArrowActive { get => isLeftArrowActive; }
        public bool IsRightArrowActive { get => isRightArrowActive; }
        public bool IsMapPoolDropDownActive { get => isMapPoolDropDownActive; }
        public bool IsLeaderboardNavigationActive { get => isLeaderboardNavigationActive; }

        public WebSocketMgrBase WebsocketMgr { get => _websocketMgr; }

        internal static CalculatorInstance CalculatorInstance;
        
        private float _percentage;

        public PPPredictorMgrBase(WebSocketMgrBase websocketMgr)
        {
            this._websocketMgr = websocketMgr;
            this._websocketMgr.OnScoreSet += WebsocketMgrOnOnScoreSet;
            _ = ResetPredictors(true);
        }

        public abstract List<string> GetEnabledPlugins();
        public abstract PPPBeatMapInfo ScoreSaberSongCoreLookUp(PPPBeatMapInfo beatMapInfo);
        public abstract Task SetupSongDetails();
        public abstract void Initialize();
        public abstract void Dispose();

        public async Task ResetPredictors(bool isConstructor = false)
        {
            RefreshLeaderboardVisibilityByIPAPluginManager();
            _loadingCounter = 0;
            _lsPPPredictor = new List<IPPPredictor>();
            _currentPPPredictor = new PPPredictorDummy();
            if (!isConstructor)
            {
                _websocketMgr.CloseScoreWebSockets();
                CalculatorInstance.OnMessage -= Logging_OnMessage;
            }
            await SetupSongDetails();
            CalculatorInstance = await CalculatorInstance.CreateAsync(
                PluginBase.ProfileInfo.ParseToSetting(await PluginBase.Instance.GetPlatformUserId()),
                PluginBase.ProfileInfo.DctleaderBoardData,
                (PPPBeatMapInfo beatMapInfo) => ScoreSaberSongCoreLookUp(beatMapInfo)
            );
            CalculatorInstance.OnMessage += Logging_OnMessage;
            if (PluginBase.ProfileInfo.IsScoreSaberEnabled) _lsPPPredictor.Add(new Shared.Predictor.PPPredictor(Leaderboard.ScoreSaber, CalculatorInstance));
            if (PluginBase.ProfileInfo.IsBeatLeaderEnabled) _lsPPPredictor.Add(new Shared.Predictor.PPPredictor(Leaderboard.BeatLeader, CalculatorInstance));
            if (PluginBase.ProfileInfo.IsHitBloqEnabled) _lsPPPredictor.Add(new Shared.Predictor.PPPredictor(Leaderboard.HitBloq, CalculatorInstance));
            if (PluginBase.ProfileInfo.IsAccSaberEnabled) _lsPPPredictor.Add(new Shared.Predictor.PPPredictor(Leaderboard.AccSaber, CalculatorInstance));
            if (PluginBase.ProfileInfo.IsAccSaberReloadedEnabled) _lsPPPredictor.Add(new Shared.Predictor.PPPredictor(Leaderboard.AccSaberReloaded, CalculatorInstance));
            if (_lsPPPredictor.Count == 0)
            {
                _lsPPPredictor.Add(new Shared.Predictor.PPPredictor(Leaderboard.NoLeaderboard, CalculatorInstance));
            }


            index = _lsPPPredictor.FindIndex(x => x.LeaderBoardName == PluginBase.ProfileInfo.LastLeaderBoardSelected);
            if (index >= 0)
            {
                _currentPPPredictor = _lsPPPredictor[index];
            }
            else
            {
                _currentPPPredictor = _lsPPPredictor[0];
            }
            foreach (IPPPredictor pPPredictor in _lsPPPredictor)
            {
                pPPredictor.Percentage = _percentage;
                pPPredictor.OnDataLoading += PPPredictor_OnDataLoading;
                pPPredictor.OnDisplayPPInfo += PPPredictor_OnDisplayPPInfo;
                pPPredictor.OnDisplaySessionInfo += PPPredictor_OnDisplaySessionInfo;
                pPPredictor.OnMapPoolRefreshed += PPPredictor_OnMapPoolRefreshed;
            }
            if(isConstructor && PluginBase.ProfileInfo.RefreshAllLeaderboards)
            {
                //Case when the leaderboard info is cleared while loading from json, start refresh of scores
                foreach (var item in _lsPPPredictor)
                {
                    if(item.LeaderBoardName == CurrentPPPredictor.LeaderBoardName)
                    {
                        CurrentPPPredictor.SetActive(true);
                    }
                    else
                    {
                        item.ResetDisplay(false);
                    }
                }
            }
            else
            {
                CurrentPPPredictor.SetActive(true);
            }
            SetNavigationArrowInteractivity();
            _websocketMgr.CreateScoreWebSockets();
        }

        private void Logging_OnMessage(object sender, LoggingMessage e)
        {
            switch (e.loggingType)
            {
                case LoggingMessage.LoggingType.Error:
                    PluginBase.ErrorPrint(e.message);
                    break;
                case LoggingMessage.LoggingType.DebugNetworkPrint:
                    PluginBase.DebugNetworkPrint(e.message, e.leaderboard);
                    break;
                default:
                    return;
            }
        }

        private void RefreshLeaderboardVisibilityByIPAPluginManager()
        {
            List<string> lsEnabledPlugin = GetEnabledPlugins();
            PluginBase.ProfileInfo.IsScoreSaberEnabled = lsEnabledPlugin.FirstOrDefault(x => x == Leaderboard.ScoreSaber.ToString()) != null;
            PluginBase.ProfileInfo.IsBeatLeaderEnabled = lsEnabledPlugin.FirstOrDefault(x => x == Leaderboard.BeatLeader.ToString()) != null;
            PluginBase.ProfileInfo.IsHitBloqEnabled = lsEnabledPlugin.FirstOrDefault(x => x == CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Leaderboard.HitBloq.ToString())) != null;
            PluginBase.ProfileInfo.IsAccSaberEnabled = PluginBase.ProfileInfo.IsScoreSaberEnabled && PluginBase.ProfileInfo.IsAccSaberEnabledManual;
            PluginBase.ProfileInfo.IsAccSaberReloadedEnabled = (PluginBase.ProfileInfo.IsBeatLeaderEnabled || PluginBase.ProfileInfo.IsScoreSaberEnabled) && PluginBase.ProfileInfo.IsAccSaberReloadedEnabledManual;
        }

        public void RestartOverlayServer()
        {
            _websocketMgr.RestartOverlayServer();
        }

        protected  void PPPredictor_OnMapPoolRefreshed(object sender, EventArgs e)
        {
            OnMapPoolRefreshed?.Invoke(this, null);
        }

        #region event handler
        protected  void PPPredictor_OnDisplaySessionInfo(object sender, DisplaySessionInfo displaySessionInfo)
        {
            OnDisplaySessionInfo?.Invoke(this, displaySessionInfo);
        }

        protected  void PPPredictor_OnDisplayPPInfo(object sender, DisplayPPInfo displayPPInfo)
        {
            OnDisplayPPInfo?.Invoke(this, displayPPInfo);
        }

        protected void PPPredictor_OnDataLoading(object sender, bool isDataLoading)
        {
            _loadingCounter = Math.Max(_loadingCounter + (isDataLoading ? +1 : -1), 0);
            if ((isDataLoading && _loadingCounter == 1) || (!isDataLoading && !IsDataLoading()))
            {
                OnDataLoading?.Invoke(this, isDataLoading);
            }
        }

        public bool IsDataLoading()
        {
            return _loadingCounter > 0;
        }
        #endregion

        public void CyclePredictors(int offset, bool triggerMapPoolRefresh = true)
        {
            _lsPPPredictor.ForEach(item => item.SetActive(false));
            index = Math.Min(Math.Max((index + offset), 0), _lsPPPredictor.Count() - 1);
            _currentPPPredictor = _lsPPPredictor[index];
            //CurrentPPPredictor.SetActive(true);
            PluginBase.ProfileInfo.LastLeaderBoardSelected = CurrentPPPredictor.LeaderBoardName;
            SetNavigationArrowInteractivity(triggerMapPoolRefresh);
        }

        private void SetNavigationArrowInteractivity(bool triggerMapPoolRefresh = true)
        {
            if (_lsPPPredictor.Count() == 1) isLeaderboardNavigationActive = false;
            else isLeaderboardNavigationActive = true;
            isLeftArrowActive = index > 0;
            isRightArrowActive = index < _lsPPPredictor.Count() - 1;
            isMapPoolDropDownActive = CurrentPPPredictor.MapPoolOptions.Count() > 1;
            CurrentPPPredictor.CalculatePP();
            if(triggerMapPoolRefresh) OnMapPoolRefreshed?.Invoke(this, null);
        }

        public void ChangeGameplayModifiers(PPPredictor.Core.DataType.BeatSaberEncapsulation.GameplayModifiers gameplayModifiers)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.ChangeGameplayModifiers(gameplayModifiers);
            }
        }

        public void DifficultyChanged(PPPBeatMapInfo beatMapInfo)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.DifficultyChanged(beatMapInfo);
            }
        }

        public void UpdateCurrentAndCheckResetSession(bool v)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.UpdateCurrentAndCheckResetSession(v);
            }
        }

        public void RefreshCurrentData(int v, bool refreshStars = false)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.RefreshCurrentData(v, refreshStars);
            }
        }

        public void SetPercentage(float percentage)
        {
            _percentage = percentage;
            foreach (var item in _lsPPPredictor)
            {
                item.Percentage = percentage;
            }
        }

        public double GetPercentage()
        {
            foreach (var item in _lsPPPredictor)
            {
                return item.Percentage;
            }
            return 0;
        }

        public void ResetDisplay(bool resetAll)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.ResetDisplay(resetAll);
            }
        }

        public double GetPPAtPercentageForCalculator(Leaderboard leaderBoardName, double percentage, bool levelFailed, bool levelPaused, PPPBeatMapInfo beatMapInfo)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.CalculatePPatPercentage(percentage, beatMapInfo, levelFailed, levelPaused); ;
            }
            return 0;
        }
        public string GetPPSuffixForLeaderboard(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.PPSuffix;
            }
            return string.Empty;
        }

        public double GetMaxPPForCalculator(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.CalculateMaxPP(); ;
            }
            return 0;
        }

        public PPPBeatMapInfo GetModifiedBeatMapInfo(Leaderboard leaderBoardName, PPPredictor.Core.DataType.BeatSaberEncapsulation.GameplayModifiers gameplayModifiers)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.GetModifiedBeatMapInfo(gameplayModifiers);

            }
            return new PPPBeatMapInfo();
        }

        public bool IsRanked(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.IsRanked();
            }
            return false;
        }

        public double GetPPGainForCalculator(Leaderboard leaderBoardName, double pp)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.CalculatePPGain(pp); ;
            }
            return 0;
        }

        public double? GetPersonalBest(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.GetPersonalBest(); ;
            }
            return null;
        }

        public string GetLeaderboardIcon(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.LeaderBoardIcon;
            }
            return string.Empty;
        }
        public string GetMapPoolIcon(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.MapPoolIcon;
            }
            return string.Empty;
        }

        public async Task<byte[]> GetLeaderboardIconData(Leaderboard leaderBoardName)
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

        public void ActivateView(bool activate)
        {
            ViewActivated?.Invoke(this, activate);
        }

        public List<object> GetMapPoolsFromLeaderboard(Leaderboard leaderBoardName)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderBoardName.ToString());
            if (predictor != null)
            {
                return predictor.MapPoolOptions;
            }
            return new List<object>();
        }

        public async Task UpdateCurrentBeatMapInfos(PPPBeatMapInfo beatMapInfo)
        {
            await Task.WhenAll(_lsPPPredictor.Select(predictor => predictor.UpdateCurrentBeatMapInfos(beatMapInfo)));
        }

        public void FindPoolWithSyncURL(string syncURL)
        {
            if (PluginBase.ProfileInfo.IsPredictorSwitchBySyncUrlEnabled)
            {
                {
                    foreach (IPPPredictor predictor in _lsPPPredictor)
                    {
                        if (syncURL.Contains(_beatleaderSyncUrlIdentifier) && predictor.LeaderBoardName == Leaderboard.BeatLeader.ToString())
                        {
                            index = _lsPPPredictor.FindIndex(x => x.LeaderBoardName == predictor.LeaderBoardName);
                            CyclePredictors(0);
                            break;
                        }
                        PPPMapPoolShort mapPool = predictor.FindPoolWithSyncURL(syncURL);
                        if(mapPool != null)
                        {
                            predictor.CurrentMapPool = mapPool;
                            index = _lsPPPredictor.FindIndex(x => x.LeaderBoardName == predictor.LeaderBoardName);
                            CyclePredictors(0, false);
                            _ = RefreshPoolDisplay(500);
                            break;
                        }
                    }
                }
            }
        }

        public async Task RefreshPoolDisplay(int msDelay)
        {
            await Task.Delay(msDelay);
            OnMapPoolRefreshed?.Invoke(this, null);
        }
        
        internal void WebsocketMgrOnOnScoreSet(object sender, PPPScoreSetData e)
        {
            this.ScoreSet(e.leaderboardName, e);
        }

        public void ScoreSet(string leaderboardName, PPPScoreSetData data)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderboardName);
            if (predictor != null)
            {
                predictor.ScoreSet(data);
            }
        }
    }
}
