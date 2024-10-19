using CountersPlus.Custom;
using CountersPlus.Utils;
using HMUI;
using PPPredictor.Interfaces;
using PPPredictor.Utilities;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace PPPredictor.Counter
{
    class CounterInfoHolder
    {
        private readonly int id = 0;
        private readonly int fontSize = 3;
        private readonly TMP_Text ppText;
        private readonly TMP_Text personalBestText;
        private readonly TMP_Text ppGainText;
        private readonly TMP_Text headerText;
        private readonly ImageView icon;
        private readonly bool showInfo;
        private readonly bool useIcon;
        private readonly Leaderboard leaderboard;
        private readonly CustomConfigModel settings;
        private readonly CanvasUtility canvasUtility;
        private readonly float positionScale;
        private readonly IPPPredictorMgr ppPredictorMgr;

        private bool _isPersonalBestAnimationFinished = false;
        public Leaderboard Leaderboard { get => leaderboard; }

        public CounterInfoHolder(int id, Leaderboard leaderboard, CustomConfigModel settings, IPPPredictorMgr ppPredictorMgr, Canvas canvas, CanvasUtility canvasUtility, float lineOffset, float offsetByLine, float positionScale, LeaderBoardGameplayInfo leaderBoardGameplayInfo) //CHECK WHEN NO C+ is installed??
        {
            this.id = id;
            this.leaderboard = leaderboard;
            this.settings = settings;
            this.ppPredictorMgr = ppPredictorMgr;
            this.canvasUtility = canvasUtility;
            this.positionScale = positionScale;
            float positionScaleFactor = 10 / positionScale;
            lineOffset *= positionScaleFactor;
            TextAlignmentOptions gainAlignment = TextAlignmentOptions.BottomLeft;
            float centerOffset = GetCenterOffset();
            float iconTextOffset = (Plugin.ProfileInfo.CounterUseIcons ? -.9f : 0f);
            float displayTypeOffset = 0;
            if (Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.PPNoSuffix || Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.PPAndGainNoSuffix || Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.PPAndGainNoBracketsNoSuffix || Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.GainNoBracketsNoSuffix)
                displayTypeOffset = -.2f;
            if (Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.GainNoBrackets)
            {
                displayTypeOffset = -0.2f;
                gainAlignment = TextAlignmentOptions.BottomRight;
            }
            if(Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.GainNoBracketsNoSuffix)
            {
                displayTypeOffset = -0.4f;
                gainAlignment = TextAlignmentOptions.BottomRight;
            }
            //Ugh this shit is so ugly
            useIcon = (canvas != null && Plugin.ProfileInfo.CounterUseIcons);
            showInfo = leaderBoardGameplayInfo.isRanked || !Plugin.ProfileInfo.CounterHideWhenUnranked;
            headerText = canvasUtility.CreateTextFromSettings(settings);
            headerText.rectTransform.anchoredPosition += new Vector2(((-1f + centerOffset) * positionScaleFactor), lineOffset) * positionScale;
            ppText = canvasUtility.CreateTextFromSettings(settings);
            ppText.rectTransform.anchoredPosition += new Vector2((0.9f + iconTextOffset + displayTypeOffset + centerOffset) * positionScaleFactor, lineOffset) * positionScale;
            ppGainText = canvasUtility.CreateTextFromSettings(settings);
            ppGainText.rectTransform.anchoredPosition += new Vector2((1.2f + iconTextOffset + displayTypeOffset + centerOffset) * positionScaleFactor, lineOffset) * positionScale;
            personalBestText = canvasUtility.CreateTextFromSettings(settings);
            personalBestText.rectTransform.anchoredPosition += new Vector2((1.2f + iconTextOffset + displayTypeOffset + centerOffset) * positionScaleFactor, lineOffset) * positionScale;
            headerText.alignment = TextAlignmentOptions.BottomLeft;
            ppGainText.alignment = gainAlignment;
            ppText.alignment = personalBestText.alignment = TextAlignmentOptions.BottomRight;
            headerText.fontSize = ppText.fontSize = ppGainText.fontSize = personalBestText.fontSize = fontSize;
            string iconPath = leaderBoardGameplayInfo.iconPath;
            if (useIcon)
            {
                icon = CreateIcon(canvas, iconPath, new Vector3((-1f + centerOffset) * positionScaleFactor, lineOffset, 0), Math.Abs(offsetByLine));
                LoadImage(icon, iconPath);
            }
            personalBestText.text = FormatPBText(leaderBoardGameplayInfo);
            ppText.enabled = false;
            ppGainText.enabled = false;

            _ = StartPersonalBestAnimation(5000);
        }

        private float GetCenterOffset()
        {
            switch (Plugin.ProfileInfo.CounterDisplayType)
            {
                case CounterDisplayType.PP:
                    return 0.5f;
                case CounterDisplayType.PPNoSuffix:
                    return 0.6f;
                case CounterDisplayType.PPAndGain:
                    return 0f;
                case CounterDisplayType.PPAndGainNoSuffix:
                    return 0.3f;
                case CounterDisplayType.PPAndGainNoBrackets:
                    return 0f;
                case CounterDisplayType.PPAndGainNoBracketsNoSuffix:
                    return 0.3f;
                case CounterDisplayType.GainNoBrackets:
                    return 0.4f;
                case CounterDisplayType.GainNoBracketsNoSuffix:
                    return 0.6f;
                default:
                    return 0;
            }
        }

        public void UpdateCounterText(LeaderBoardGameplayInfo gamePlayInfo)
        {
            string percentageThresholdColor = DisplayHelper.GetDisplayColor(0, false);
            if (gamePlayInfo.percentage > gamePlayInfo.targetPercentage && Plugin.ProfileInfo.CounterHighlightTargetPercentage)
            {
                percentageThresholdColor = DisplayHelper.GetDisplayColor(1, false);
            }

            if (showInfo && !Plugin.ProfileInfo.CounterUseIcons) headerText.text = $"<color=\"{percentageThresholdColor}\">{leaderboard}</color>";
            if (showInfo)
            {
                if (Plugin.ProfileInfo.CounterUseIcons) icon.enabled = true;

                string maxPPReachedPrefix = string.Empty;
                string maxPPReachedSuffix = string.Empty;

                if(gamePlayInfo.maxPP > 0 && gamePlayInfo.pp >= gamePlayInfo.maxPP)
                {
                    maxPPReachedPrefix = "<color=\"yellow\">";
                    maxPPReachedSuffix = "</color>";
                }
                switch (Plugin.ProfileInfo.CounterDisplayType)
                {
                    case CounterDisplayType.PP:
                        ppText.text = $"{maxPPReachedPrefix}{gamePlayInfo.pp:F2}{gamePlayInfo.ppSuffix}{maxPPReachedSuffix}";
                        break;
                    case CounterDisplayType.PPAndGain:
                        ppText.text = $"{maxPPReachedPrefix}{gamePlayInfo.pp:F2}{gamePlayInfo.ppSuffix}{maxPPReachedSuffix}";
                        ppGainText.text = $"[<color=\"{DisplayHelper.GetDisplayColor(gamePlayInfo.ppGain, false, true)}\">{gamePlayInfo.ppGain:F2}{gamePlayInfo.ppSuffix}</color>]";
                        break;
                    case CounterDisplayType.PPAndGainNoBrackets:
                        ppText.text = $"{maxPPReachedPrefix}{gamePlayInfo.pp:F2}pp{maxPPReachedSuffix}";
                        ppGainText.text = $"<color=\"{DisplayHelper.GetDisplayColor(gamePlayInfo.ppGain, false, true)}\">{gamePlayInfo.ppGain:F2}{gamePlayInfo.ppSuffix}</color>";
                        break;
                    case CounterDisplayType.GainNoBrackets:
                        ppGainText.text = $"<color=\"{DisplayHelper.GetDisplayColor(gamePlayInfo.ppGain, false, true)}\">{gamePlayInfo.ppGain:F2}{gamePlayInfo.ppSuffix}</color>";
                        break;
                    case CounterDisplayType.PPNoSuffix:
                        ppText.text = $"{maxPPReachedPrefix}{gamePlayInfo.pp:F2}{maxPPReachedSuffix}";
                        break;
                    case CounterDisplayType.PPAndGainNoSuffix:
                        ppText.text = $"{maxPPReachedPrefix}{gamePlayInfo.pp:F2}{maxPPReachedSuffix}";
                        ppGainText.text = $"[<color=\"{DisplayHelper.GetDisplayColor(gamePlayInfo.ppGain, false, true)}\">{gamePlayInfo.ppGain:F2}</color>]";
                        break;
                    case CounterDisplayType.PPAndGainNoBracketsNoSuffix:
                        ppText.text = $"{maxPPReachedPrefix}{gamePlayInfo.pp:F2}{maxPPReachedSuffix}";
                        ppGainText.text = $"<color=\"{DisplayHelper.GetDisplayColor(gamePlayInfo.ppGain, false, true)}\">{gamePlayInfo.ppGain:F2}</color>";
                        break;
                    case CounterDisplayType.GainNoBracketsNoSuffix:
                        ppGainText.text = $"<color=\"{DisplayHelper.GetDisplayColor(gamePlayInfo.ppGain, false, true)}\">{gamePlayInfo.ppGain:F2}</color>";
                        break;
                    default:
                        break;
                }
            }
        }

        private string FormatPBText(LeaderBoardGameplayInfo gamePlayInfo)
        {
            string pbs = gamePlayInfo.personalBest.HasValue ? gamePlayInfo.personalBest.Value.ToString("F2") : Constants.NoPBFound;
            string suffix = string.Empty;
            if(Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.PP 
                || Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.PPAndGain
                || Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.PPAndGainNoBrackets
                || Plugin.ProfileInfo.CounterDisplayType == CounterDisplayType.GainNoBrackets)
            {
                suffix = gamePlayInfo.ppSuffix;
            }
            if(string.IsNullOrEmpty(suffix))
                return $"{pbs} PB";
            return $"{pbs} {suffix} PB";
        }

        private ImageView CreateIcon(Canvas canvas, string imageIdent, Vector3 offset, float lineOffset)
        {
            GameObject imageGameObject = new GameObject(imageIdent, typeof(RectTransform));
            ImageView newImage = imageGameObject.AddComponent<ImageView>();
            newImage.rectTransform.SetParent(canvas.transform, false);
            newImage.rectTransform.anchoredPosition = positionScale * (canvasUtility.GetAnchoredPositionFromConfig(settings) + offset + new Vector3(0, (lineOffset / (positionScale * 0.125f)) + (0.15f / positionScale), 0));
            newImage.rectTransform.sizeDelta = new Vector2(2.5f, 2.5f);
            newImage.enabled = false;
            var noGlowMat = new Material(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").First())
            {
                name = "UINoGlowCustom"
            };
            newImage.material = noGlowMat;
            return newImage;
        }

        private async void LoadImage(ImageView newImage, string imageIdent)
        {
            byte[] data = null;
            if (Plugin.ProfileInfo.CounterUseCustomMapPoolIcons && imageIdent.Contains("http"))
            {
                data = await ppPredictorMgr.GetLeaderboardIconData(leaderboard);
            }
            if(data == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                System.IO.Stream stream = assembly.GetManifestResourceStream(ppPredictorMgr.GetLeaderboardIcon(leaderboard));
                data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
            }
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(data);
            texture.Apply();
            newImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        internal void MoveIconForLongMaxPP(int digits)
        {
            if(digits > 3)
            {
                icon.rectTransform.anchoredPosition -= new Vector2((digits - 3) * 1f, 0);
            }
        }

        #region animation stuff
        public async Task StartPersonalBestAnimation(int delay)
        {
            await Task.Delay(delay);
            if (_isPersonalBestAnimationFinished) return;
            _isPersonalBestAnimationFinished = true;
            Task t1 = MoveTextWithAnimation(AnimateableCounterText.PP, 100f, new Vector3(-.6f, 0, 0), true, true, true, true);
            Task t2 = MoveTextWithAnimation(AnimateableCounterText.PPGAIN, 100f, new Vector3(-.6f, 0, 0), true, true, !Plugin.ProfileInfo.IsCounterGainSilentModeEnabled, !Plugin.ProfileInfo.IsCounterGainSilentModeEnabled);
            Task t3 = MoveTextWithAnimation(AnimateableCounterText.PERSONALBEST, 100f, new Vector3(.6f, 0, 0), false, false, true, false);
        }

        internal bool IsPersonalBestAnimationDone()
        {
            return _isPersonalBestAnimationFinished;
        }
        internal bool IsPersonalBestAnimationRunning()
        {
            return !_isPersonalBestAnimationFinished;
        }
        public async Task MoveTextWithAnimation(AnimateableCounterText animateableCounterText, float animationDelayById, Vector3 offset, bool isStartOffset, bool isEaseOut, bool isVisibleAtStart, bool isVisibleAtEnd, Func<bool> cancelRunFuncion = null)
        {
            if (cancelRunFuncion != null && cancelRunFuncion()) return;
            TMP_Text tmpText = GetTMPText(animateableCounterText);
            Vector3 originalPPGainPosition = tmpText.transform.position;
            await Task.Delay((int)(animationDelayById * id));
            tmpText.enabled = isVisibleAtStart;
            int steps = 25;
            tmpText.transform.position = isStartOffset ? originalPPGainPosition - offset : originalPPGainPosition;
            for (int i = 0; i < steps; i++)
            {
                float t = (float)i / (float)steps;
                if(isEaseOut) t = Mathf.Sin(t * Mathf.PI * 0.5f); //ease out
                if (!isEaseOut) t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f); //ease in
                if (!isStartOffset) t = 1f - t; //inverseAnimation
                tmpText.transform.position = originalPPGainPosition - Vector3.Lerp(offset, new Vector3(), t);
                tmpText.alpha = t;
                await Task.Delay(10);
            }
            tmpText.enabled = isVisibleAtEnd;
            tmpText.transform.position = isStartOffset ? originalPPGainPosition : originalPPGainPosition - offset;
        }

        private TMP_Text GetTMPText(AnimateableCounterText animateableCounterText)
        {
            switch (animateableCounterText)
            {
                case AnimateableCounterText.PPGAIN :
                    return ppGainText;
                case AnimateableCounterText.PP :
                    return ppText;
                case AnimateableCounterText.PERSONALBEST :
                    return personalBestText;
                default: return ppText;
            }
        }
        #endregion
    }

    enum AnimateableCounterText
    {
        PPGAIN,
        PP,
        PERSONALBEST
    }
}
