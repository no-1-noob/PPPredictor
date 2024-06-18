using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PPPredictor.UI.Test
{
    internal class TestGraph : Graphic
    {
        public string labelText = "PP";
        public Vector2 labelPosition = new Vector2(0.5f, 0.5f); // Center of the graphic
        public int fontSize = 14;
        public Color textColor = Color.black;

        protected override void Start()
        {
            base.Start();
            CreateLabel();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Plugin.Log?.Error("OnPopulateMesh");
            Vector2 corner1 = Vector2.zero;
            Vector2 corner2 = Vector2.zero;

            corner1.x = 0f;
            corner1.y = 0f;
            corner2.x = 1f;
            corner2.y = 1f;

            corner1.x -= rectTransform.pivot.x;
            corner1.y -= rectTransform.pivot.y;
            corner2.x -= rectTransform.pivot.x;
            corner2.y -= rectTransform.pivot.y;

            corner1.x *= rectTransform.rect.width;
            corner1.y *= rectTransform.rect.height;
            corner2.x *= rectTransform.rect.width;
            corner2.y *= rectTransform.rect.height;

            vh.Clear();

            UIVertex vert = UIVertex.simpleVert;

            vert.position = new Vector2(corner1.x, corner1.y);
            vert.color = color;
            vh.AddVert(vert);

            vert.position = new Vector2(corner1.x, corner2.y);
            vert.color = color;
            vh.AddVert(vert);

            vert.position = new Vector2(corner2.x, corner2.y);
            vert.color = color;
            vh.AddVert(vert);

            //vert.position = new Vector2(corner2.x, corner1.y);
            //vert.color = color;
            //vh.AddVert(vert);

            vh.AddTriangle(0, 1, 2);
            //vh.AddTriangle(2, 3, 0);
        }

        private void CreateLabel()
        {
            // Create a new GameObject for the label
            GameObject labelGameObject = new GameObject("Label");
            labelGameObject.transform.SetParent(transform, false);

            // Add Text component to the label GameObject
            Text labelTextComponent = labelGameObject.AddComponent<Text>();
            labelTextComponent.text = labelText;
            labelTextComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            labelTextComponent.fontSize = fontSize;
            labelTextComponent.color = textColor;
            labelTextComponent.alignment = TextAnchor.MiddleCenter;

            // Set RectTransform properties
            RectTransform rectTransform = labelGameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = labelPosition;
        }
    }
}
