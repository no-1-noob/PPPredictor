using PPPredictor.Interfaces;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PPPredictor.Counter
{
    class PPPCounter : CountersPlus.Counters.Custom.BasicCustomCounter
    {
        [Inject] private readonly IPPPredictorMgr ppPredictorMgr;
        [Inject] private readonly GamePlayMgr gamePlayMgr;
        private List<CounterInfoHolder> lsCounterInfoHolder = new List<CounterInfoHolder>();
        private bool _iconMoved = false;
        private bool _isCounterCreated = false;
        private readonly float originalLineOffset = 0.15f;

        public override void CounterInit()
        {
            gamePlayMgr.OnPaused += GamePlayMgr_OnPaused;
            gamePlayMgr.OnResumed += GamePlayMgr_OnResumed;
            gamePlayMgr.OnLastNoteHit += GamePlayMgr_OnLastNoteHit;
            gamePlayMgr.OnFirstNoteHit += GamePlayMgr_OnFirstNoteHit;
            gamePlayMgr.OnGameplayInfoChanged += GamePlayMgr_OnGameplayInfoChanged;
            DisplayInfo(gamePlayMgr.GetStatus());
        }

        public override void CounterDestroy()
        {
            gamePlayMgr.OnPaused -= GamePlayMgr_OnPaused;
            gamePlayMgr.OnResumed -= GamePlayMgr_OnResumed;
            gamePlayMgr.OnLastNoteHit -= GamePlayMgr_OnLastNoteHit;
            gamePlayMgr.OnFirstNoteHit -= GamePlayMgr_OnFirstNoteHit;
            gamePlayMgr.OnGameplayInfoChanged -= GamePlayMgr_OnGameplayInfoChanged;
        }

        #region eventhandler
        private void GamePlayMgr_OnResumed(object sender, EventArgs e)
        {
            lsCounterInfoHolder.ForEach(item => _ = item.MoveTextWithAnimation(AnimateableCounterText.PPGAIN, 100f, new Vector3(0, 0, 0), false, true, true, true, item.IsPersonalBestAnimationRunning));
        }

        private void GamePlayMgr_OnPaused(object sender, EventArgs e)
        {
            lsCounterInfoHolder.ForEach(item => _ = item.MoveTextWithAnimation(AnimateableCounterText.PPGAIN, 100f, new Vector3(0, .3f, 0), true, true, true, true, item.IsPersonalBestAnimationRunning));
        }

        private void GamePlayMgr_OnLastNoteHit(object sender, EventArgs e)
        {
            lsCounterInfoHolder.ForEach(item => _ = item.MoveTextWithAnimation(AnimateableCounterText.PPGAIN, 100f, new Vector3(0, .3f, 0), true, true, true, true, item.IsPersonalBestAnimationRunning));
        }

        private void GamePlayMgr_OnFirstNoteHit(object sender, EventArgs e)
        {
            lsCounterInfoHolder.ForEach(item =>
            {
                if (!item.IsPersonalBestAnimationDone())
                {
                    _ = item.StartPersonalBestAnimation(0);
                }
            });
        }

        private void GamePlayMgr_OnGameplayInfoChanged(object sender, GamePlayInfo gameplayInfo)
        {
            DisplayInfo(gameplayInfo);
        }

        private void DisplayInfo(GamePlayInfo gameplayInfo)
        {
            if (gameplayInfo == null) return;
            SetupCounter(gameplayInfo);
            DisplayCounterText(gameplayInfo);
        }
        #endregion

        private void SetupCounter(GamePlayInfo gamePlayInfo)
        {
            if (_isCounterCreated) return;
#if DEBUG
                //Center Helper for development
                /*float offsetPlus = 0.1f;
                for (int i = 0; i < 5; i++)
                {
                    TMP_Text testPlus = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(offsetPlus * -(i + 1), ((i % 2) + 1) * offsetPlus, 0));
                    testPlus.alignment = TextAlignmentOptions.Center;
                    testPlus.text = "+";
                }
                for (int i = 0; i < 5; i++)
                {
                    TMP_Text testPlus = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(offsetPlus * (i + 1), ((i % 2) + 1) * offsetPlus, 0));
                    testPlus.alignment = TextAlignmentOptions.Center;
                    testPlus.text = "+";
                }*/
                //debugPercentage = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0, 0, 0));
                //debugPercentage.alignment = TextAlignmentOptions.Center;
#endif


            var canvas = CanvasUtility.GetCanvasFromID(this.Settings.CanvasID);
            float positionScale = CanvasUtility.GetCanvasSettingsFromCanvas(canvas).PositionScale;
            lsCounterInfoHolder = new List<CounterInfoHolder>();
            int scoreboardCount = gamePlayInfo.scoreboardCount;
            float lineOffset = (originalLineOffset * (scoreboardCount / 2)) + (originalLineOffset * (scoreboardCount % 2));
            int id = 0;
            LeaderBoardGameplayInfo infoScoreSaber = gamePlayInfo.lsInfo.FirstOrDefault(x => x.leaderboard == Leaderboard.ScoreSaber);
            LeaderBoardGameplayInfo infoBeatLeader = gamePlayInfo.lsInfo.FirstOrDefault(x => x.leaderboard == Leaderboard.BeatLeader);
            LeaderBoardGameplayInfo infoHitBloq = gamePlayInfo.lsInfo.FirstOrDefault(x => x.leaderboard == Leaderboard.HitBloq);
            if (infoScoreSaber != null)
            {
                lsCounterInfoHolder.Add(new CounterInfoHolder(id, Leaderboard.ScoreSaber, Settings, ppPredictorMgr, canvas, CanvasUtility, lineOffset, originalLineOffset, positionScale, infoScoreSaber));
                lineOffset -= originalLineOffset * 2;
                id++;
            }
            if (infoBeatLeader != null)
            {
                lsCounterInfoHolder.Add(new CounterInfoHolder(id, Leaderboard.BeatLeader, Settings, ppPredictorMgr, canvas, CanvasUtility, lineOffset, originalLineOffset, positionScale, infoBeatLeader));
                lineOffset -= originalLineOffset * 2;
                id++;
            }
            if (infoHitBloq != null)
            {
                lsCounterInfoHolder.Add(new CounterInfoHolder(id, Leaderboard.HitBloq, Settings, ppPredictorMgr, canvas, CanvasUtility, lineOffset, originalLineOffset, positionScale, infoHitBloq));
                lineOffset -= originalLineOffset * 2;
                id++;
            }
            _isCounterCreated = true;
        }

        private void DisplayCounterText(GamePlayInfo gameplayInfo)
        {
            foreach (var info in gameplayInfo.lsInfo)
            {
                var counterInfoHolder = lsCounterInfoHolder.FirstOrDefault(x => x.Leaderboard == info.leaderboard);
                if(counterInfoHolder != null)
                {
                    counterInfoHolder.UpdateCounterText(info);
                }

                //Maybe move to initial creation?
                if (!_iconMoved && !gameplayInfo.lsInfo.Where(x => x.maxPP == -1).Any())
                {
                    _iconMoved = true;
                    double maxMaxPP = gameplayInfo.lsInfo.Select(x => x.maxPP).Max();
                    lsCounterInfoHolder.ForEach(item => item.MoveIconForLongMaxPP(Math.Truncate(maxMaxPP).ToString().Length));
                }
            }
        }
    }
}
