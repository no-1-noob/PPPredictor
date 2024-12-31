using BeatSaberPlaylistsLib.Types;
using IPA.Loader;
using PPPredictor.Core;
using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Data;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Utilities
{
    internal class PPPredictorMgr : IInitializable, IDisposable, IPPPredictorMgr

    {
        private readonly WebSocketMgr _websocketMgr;
        private const string _beatleaderSyncUrlIdentifier = ".beatleader.";
        private const string _customDataSyncUrl = "syncURL";
        private List<IPPPredictor> _lsPPPredictor;
        private int index = 0;
        private IPPPredictor _currentPPPredictor;
        private bool isLeftArrowActive = false;
        private bool isRightArrowActive = false;
        private bool isMapPoolDropDownActive = true;
        private bool isLeaderboardNavigationActive = false;

        public event EventHandler<bool> ViewActivated;
        public event EventHandler<bool> OnDataLoading;
        public event EventHandler<DisplaySessionInfo> OnDisplaySessionInfo;
        public event EventHandler<DisplayPPInfo> OnDisplayPPInfo;
        public event EventHandler OnMapPoolRefreshed;

        public IPPPredictor CurrentPPPredictor { get => _currentPPPredictor; }
        public bool IsLeftArrowActive { get => isLeftArrowActive; }
        public bool IsRightArrowActive { get => isRightArrowActive; }
        public bool IsMapPoolDropDownActive { get => isMapPoolDropDownActive; }
        public bool IsLeaderboardNavigationActive { get => isLeaderboardNavigationActive; }

        public WebSocketMgr WebsocketMgr { get => _websocketMgr; }

        internal static Instance instance;
        private float _percentage;

        internal PPPredictorMgr()
        {
            this._websocketMgr = new WebSocketMgr(this);
            ResetPredictors();
        }

        public async void ResetPredictors()
        {
            RefreshLeaderboardVisibilityByIPAPluginManager();
            _lsPPPredictor = new List<IPPPredictor>();
            _currentPPPredictor = new DummPredictor();

            instance = await Instance.CreateAsync(
                new Settings(
                    Plugin.ProfileInfo.IsScoreSaberEnabled,
                    Plugin.ProfileInfo.IsBeatLeaderEnabled,
                    Plugin.ProfileInfo.IsHitBloqEnabled,
                    Plugin.ProfileInfo.IsAccSaberEnabled,
                    (await Plugin.GetUserInfoBS()).platformUserId,
                    Plugin.ProfileInfo.PpGainCalculationType,
                    Plugin.ProfileInfo.HitbloqMapPoolSorting,
                    //Plugin.ProfileInfo.platformUserId,
                    "",
                    ProfileInfo.RefetchMapInfoAfterDays,
                    Plugin.ProfileInfo.LastSessionReset,
                    Plugin.ProfileInfo.ResetSessionHours
                ),
                Plugin.ProfileInfo.DctleaderBoardData
            );
#warning not really clean i think
            Logging.OnMessage += Logging_OnMessage;
            if (Plugin.ProfileInfo.IsScoreSaberEnabled) _lsPPPredictor.Add(new PPPredictor(Leaderboard.ScoreSaber, instance));
            if (Plugin.ProfileInfo.IsBeatLeaderEnabled) _lsPPPredictor.Add(new PPPredictor(Leaderboard.BeatLeader, instance));
            if (Plugin.ProfileInfo.IsHitBloqEnabled) _lsPPPredictor.Add(new PPPredictor(Leaderboard.HitBloq, instance));
            if (Plugin.ProfileInfo.IsAccSaberEnabled) _lsPPPredictor.Add(new PPPredictor(Leaderboard.AccSaber, instance));
            if (_lsPPPredictor.Count == 0)
            {
                _lsPPPredictor.Add(new PPPredictor(Leaderboard.NoLeaderboard, instance));
            }


            index = _lsPPPredictor.FindIndex(x => x.LeaderBoardName == Plugin.ProfileInfo.LastLeaderBoardSelected);
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
            CurrentPPPredictor.SetActive(true);
            SetNavigationArrowInteractivity();
            _websocketMgr.CreateScoreWebSockets();
        }

        private void Logging_OnMessage(object sender, LoggingMessage e)
        {
            switch (e.loggingType)
            {
                case LoggingMessage.LoggingType.Error:
                    Plugin.ErrorPrint(e.message);
                    break;
                case LoggingMessage.LoggingType.DebugNetworkPrint:
                    Plugin.DebugNetworkPrint(e.message);
                    break;
                default:
                    return;
            }
        }

        private void RefreshLeaderboardVisibilityByIPAPluginManager()
        {
            List<PluginMetadata> lsEnabledPlugin = PluginManager.EnabledPlugins.ToList();
            //Plugin.ProfileInfo.IsScoreSaberEnabled = lsEnabledPlugin.FirstOrDefault(x => x.Name == Leaderboard.ScoreSaber.ToString()) != null;
            //Plugin.ProfileInfo.IsBeatLeaderEnabled = lsEnabledPlugin.FirstOrDefault(x => x.Name == Leaderboard.BeatLeader.ToString()) != null;
            //Plugin.ProfileInfo.IsHitBloqEnabled = lsEnabledPlugin.FirstOrDefault(x => x.Name == CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Leaderboard.HitBloq.ToString())) != null;
            //Plugin.ProfileInfo.IsAccSaberEnabled = Plugin.ProfileInfo.IsScoreSaberEnabled && Plugin.ProfileInfo.IsAccSaberEnabledManual;

            Plugin.ProfileInfo.IsScoreSaberEnabled = false; // lsEnabledPlugin.FirstOrDefault(x => x.Name == Leaderboard.ScoreSaber.ToString()) != null;
            Plugin.ProfileInfo.IsBeatLeaderEnabled = lsEnabledPlugin.FirstOrDefault(x => x.Name == Leaderboard.BeatLeader.ToString()) != null;
            Plugin.ProfileInfo.IsHitBloqEnabled = false; //lsEnabledPlugin.FirstOrDefault(x => x.Name == CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Leaderboard.HitBloq.ToString())) != null;
            Plugin.ProfileInfo.IsAccSaberEnabled = false; //Plugin.ProfileInfo.IsScoreSaberEnabled && Plugin.ProfileInfo.IsAccSaberEnabledManual;
        }

        public void RestartOverlayServer()
        {
            _websocketMgr.RestartOverlayServer();
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
            _currentPPPredictor = _lsPPPredictor[index];
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

        public void DifficultyChanged(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmapKey)
        {
            foreach (var item in _lsPPPredictor)
            {
                item.DifficultyChanged(selectedBeatmapLevel, beatmapKey);
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

        public PPPBeatMapInfo GetModifiedBeatMapInfo(Leaderboard leaderBoardName, GameplayModifiers gameplayModifiers)
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

        public async Task UpdateCurrentBeatMapInfos(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmapKey)
        {
            await Task.WhenAll(_lsPPPredictor.Select(predictor => predictor.UpdateCurrentBeatMapInfos(selectedBeatmapLevel, beatmapKey)));
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
            _websocketMgr.Dispose();
        }

        public async Task FindPoolWithSyncURL(IPlaylist playlist)
        {
            if (playlist != null && Plugin.ProfileInfo.IsPredictorSwitchBySyncUrlEnabled)
            {
                if (playlist.TryGetCustomData(_customDataSyncUrl, out object outSyncURL) && !string.IsNullOrEmpty(outSyncURL as string))
                {
                    string syncUrl = outSyncURL.ToString();
                    foreach (IPPPredictor predictor in _lsPPPredictor)
                    {
                        if (syncUrl.Contains(_beatleaderSyncUrlIdentifier) && predictor.LeaderBoardName == Leaderboard.BeatLeader.ToString())
                        {
                            index = _lsPPPredictor.FindIndex(x => x.LeaderBoardName == predictor.LeaderBoardName);
                            CyclePredictors(0);
                            break;
                        }
                        PPPMapPoolShort mapPool = await predictor.FindPoolWithSyncURL(outSyncURL as string);
                        if(mapPool != null)
                        {
                            predictor.CurrentMapPool = mapPool;
                            index = _lsPPPredictor.FindIndex(x => x.LeaderBoardName == predictor.LeaderBoardName);
                            CyclePredictors(0);
                            break;
                        }
                    }
                }
            }
        }

        public void ScoreSet(string leaderboardName, PPPScoreSetData data)
        {
            IPPPredictor predictor = _lsPPPredictor.Find(x => x.LeaderBoardName == leaderboardName);
            if (predictor != null)
            {
                predictor.ScoreSet(data).GetAwaiter().GetResult();
            }
        }
    }
}
