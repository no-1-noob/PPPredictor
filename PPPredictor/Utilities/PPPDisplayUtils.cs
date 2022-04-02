using PPPredictor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SplitSaber
{
    class PPPDisplayUtils
    {
        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return CreateText(parent, text, anchoredPosition, new Vector2(60f, 10f));
        }

        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            GameObject gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);
            TextMeshProUGUI textMesh = gameObj.AddComponent<TextMeshProUGUI>();
            //textMesh.font = UnityEngine.Object.Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Segoe UI"));
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textMesh;
        }

        public static Slider CreateSlider(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return CreateSlider(parent, text, anchoredPosition, new Vector2(60f, 10f));
        }

        public static Slider CreateSlider(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            GameObject gameObj = new GameObject("CustomSlider");
            gameObj.SetActive(false);
            Slider sliderMesh = gameObj.AddComponent<Slider>();
            /*sliderMesh.useGUILayout = false;
            sliderMesh.fillRect = parent;
            sliderMesh.direction = Slider.Direction.LeftToRight;
            sliderMesh.enabled = true;
            sliderMesh.wholeNumbers = false;*/
            /*sliderMesh.t = text;
            sliderMesh.fontSize = 4;
            sliderMesh.color = Color.white;

            sliderMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            sliderMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            sliderMesh.rectTransform.sizeDelta = sizeDelta;
            sliderMesh. = anchoredPosition;*/

            gameObj.SetActive(true);
            Plugin.Log?.Info($"CustomSlider {sliderMesh.isActiveAndEnabled}");
            return sliderMesh;
        }

    }
}
