using SplitSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PPPredictor
{
    class PPPDisplay : MonoBehaviour
    {
        private Canvas _canvas;
        private TMP_Text _text;
        private Slider _slider;
        private static readonly Vector3 Position = new Vector3(0, 2.5f, 2.5f);
        private static readonly Vector3 Rotation = new Vector3(0, 0, 0);
        private static readonly Vector3 Scale = new Vector3(0.01f, 0.01f, 0.01f);
        private static readonly Vector2 CanvasSize = new Vector2(100, 50);
        private static readonly Vector2 HeaderPosition = new Vector2(10, 15);
        private static readonly Vector2 HeaderPositionSlider = new Vector2(10, 25);
        private static readonly Vector2 HeaderSize = new Vector2(100, 20);
        private const float HeaderFontSize = 15f;

        public static PPPDisplay Create()
        {
            return new GameObject("SplitSaberDebugText").AddComponent<PPPDisplay>();
        }

        private void Awake()
        {
            Plugin.Log?.Info("Awake1");
            gameObject.transform.position = Position;
            gameObject.transform.eulerAngles = Rotation;
            gameObject.transform.localScale = Scale;
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.enabled = true;
            var rectTransform = _canvas.transform as RectTransform;
            var rectTransform2 = _canvas.transform as RectTransform;
            rectTransform.sizeDelta = CanvasSize;
            rectTransform2.sizeDelta = CanvasSize;

            _text = PPPDisplayUtils.CreateText(_canvas.transform as RectTransform, "PPPDisplay", HeaderPosition);
            rectTransform = _text.transform as RectTransform;
            rectTransform.SetParent(_canvas.transform, false);
            rectTransform.anchoredPosition = HeaderPosition;
            rectTransform.sizeDelta = HeaderSize;
            _text.text = "SplitSaberDebug";
            _text.fontSize = HeaderFontSize;

            /*_slider = PPPDisplayUtils.CreateSlider(_canvas.transform as RectTransform, "PPPDisplay", HeaderPosition);
            rectTransform2 = _slider.transform as RectTransform;
            rectTransform2.SetParent(_canvas.transform, false);
            rectTransform2.anchoredPosition = HeaderPosition;
            rectTransform2.sizeDelta = HeaderSize;*/

            /*Slider sliderMesh = gameObject.AddComponent<Slider>();
            var rectTransform3 = sliderMesh.transform as RectTransform;
            rectTransform3.SetParent(gameObject.transform, false);
            rectTransform3.anchoredPosition = HeaderPosition;
            rectTransform3.sizeDelta = HeaderSize;*/
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

            DontDestroyOnLoad(gameObject);
        }

        public void showMessage(string message)
        {
            _text.text = message;
        }
    }
}
