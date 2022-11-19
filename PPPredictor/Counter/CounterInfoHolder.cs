using CountersPlus.Custom;
using CountersPlus.Utils;
using HMUI;
using PPPredictor.Utilities;
using System;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace PPPredictor.Counter
{
    class CounterInfoHolder
    {
        private readonly int fontSize = 3;
        private readonly TMP_Text ppText;
        private readonly TMP_Text ppGainText;
        private readonly TMP_Text headerText;
        private readonly ImageView icon;
        private readonly bool showInfo;
        private readonly bool useIcon;
        private readonly Leaderboard leaderboard;
        private readonly CustomConfigModel settings;
        private readonly CanvasUtility canvasUtility;

        public CounterInfoHolder(Leaderboard leaderboard, CustomConfigModel settings, string iconPath, Canvas canvas, CanvasUtility canvasUtility, float lineOffset) //CHECK WHEN NO C+ is installed??
        {
            this.leaderboard = leaderboard;
            this.settings = settings;
            this.canvasUtility = canvasUtility;
            float iconTextOffset = Plugin.ProfileInfo.CounterUseIcons ? -.9f : 0f;
            useIcon = (canvas != null && Plugin.ProfileInfo.CounterUseIcons);
            showInfo = Plugin.pppViewController.ppPredictorMgr.IsRanked(leaderboard) || !Plugin.ProfileInfo.CounterHideWhenUnranked;
            headerText = canvasUtility.CreateTextFromSettings(settings, new Vector3(-1f, lineOffset, 0));
            ppText = canvasUtility.CreateTextFromSettings(settings, new Vector3(0.9f + iconTextOffset, lineOffset, 0));
            ppGainText = canvasUtility.CreateTextFromSettings(settings, new Vector3(1.2f + iconTextOffset, lineOffset, 0));
            headerText.alignment = ppGainText.alignment = TextAlignmentOptions.BottomLeft;
            ppText.alignment = TextAlignmentOptions.BottomRight;
            headerText.fontSize = ppText.fontSize = ppGainText.fontSize = fontSize;
            if (useIcon)
            {
                icon = CreateIcon(canvas, iconPath, new Vector3(-1f, lineOffset, 0), Math.Abs(lineOffset));
            }
        }

        public void UpdateCounterText(double percentage)
        {
            string percentageThresholdColor = DisplayHelper.GetDisplayColor(0, false);
            if (percentage > Plugin.pppViewController.ppPredictorMgr.GetPercentage() && Plugin.ProfileInfo.CounterHighlightTargetPercentage)
            {
                percentageThresholdColor = DisplayHelper.GetDisplayColor(1, false);
            }

            if (showInfo && !Plugin.ProfileInfo.CounterUseIcons) headerText.text = $"<color=\"{percentageThresholdColor}\">{leaderboard}</color>";
            if (showInfo)
            {
                if (Plugin.ProfileInfo.CounterUseIcons) icon.enabled = true;
                double pp = Plugin.pppViewController.ppPredictorMgr.GetPPAtPercentageForCalculator(leaderboard, percentage);
                double ppGain = Math.Round(Plugin.pppViewController.ppPredictorMgr.GetPPGainForCalculator(leaderboard, pp), 2);
                ppText.text = $"{pp:F2}pp";
                if (Plugin.ProfileInfo.CounterShowGain) ppGainText.text = $"[<color=\"{DisplayHelper.GetDisplayColor(ppGain, false)}\">{ppGain:F2}</color>]";
            }
        }

        private ImageView CreateIcon(Canvas canvas, string imageIdent, Vector3 offset, float lineOffset)
        {
            GameObject imageGameObject = new GameObject(imageIdent, typeof(RectTransform));
            ImageView newImage = imageGameObject.AddComponent<ImageView>();
            float posScaleFactor = 10;
            newImage.rectTransform.SetParent(canvas.transform, false);
            newImage.rectTransform.anchoredPosition = posScaleFactor * (canvasUtility.GetAnchoredPositionFromConfig(settings) + offset + new Vector3(0, lineOffset / 1.25f, 0));
            newImage.rectTransform.sizeDelta = new Vector2(2.5f, 2.5f);
            newImage.enabled = false;
            var noGlowMat = new Material(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").First())
            {
                name = "UINoGlowCustom"
            };
            newImage.material = noGlowMat;
            //Load Image
            var assembly = Assembly.GetExecutingAssembly();
            System.IO.Stream stream = assembly.GetManifestResourceStream(imageIdent);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(data);
            texture.Apply();
            newImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            return newImage;
        }
    }
}
