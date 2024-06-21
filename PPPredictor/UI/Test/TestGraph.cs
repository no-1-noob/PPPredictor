using BeatSaberMarkupLanguage;
using IPA.Config.Data;
using PPPredictor.Data.DisplayInfos;
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
        public string labelText = "Graph Label";
        public Vector2 labelPosition = new Vector2(0.5f, 0.5f); // Center of the graphic
        public int fontSize = 14;
        public Color textColor = Color.black;
        public int xScaleMarkers = 10;
        public int yScaleMarkers = 10;
        private float lineWidth = 0.5f;
        private DisplayGraphData _displayGraphData;
        public DisplayGraphData displayGraphData
        {
            set
            {
                _displayGraphData = value;
                SetAllDirty();
            }
        }

        protected override void Start()
        {
            base.Start();
            //CreateLabel();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Plugin.DebugPrint($"OnPopulateMesh {_displayGraphData?.LsPoints?.Count} {_displayGraphData?.LsPoints.Select(x => x.Y).Max()}");
            vh.Clear();

            Rect rect = GetPixelAdjustedRect();
            Plugin.DebugPrint($"OnPopulateMesh rect x{rect.xMin} {rect.xMax} rect y {rect.yMin} {rect.yMax}");

            //// Define the rectangle corners
            //Vector2 corner1 = new Vector2(rect.xMin, rect.yMin);
            //Vector2 corner2 = new Vector2(rect.xMin, rect.yMax);
            //Vector2 corner3 = new Vector2(rect.xMax, rect.yMax);
            //Vector2 corner4 = new Vector2(rect.xMax, rect.yMin);

            //// Convert corners to vertices
            //UIVertex vert = UIVertex.simpleVert;
            //vert.color = Color.yellow;

            //vert.position = corner1;
            //vh.AddVert(vert);

            //vert.position = corner2;
            //vh.AddVert(vert);

            //vert.position = corner3;
            //vh.AddVert(vert);

            //vert.position = corner4;
            //vh.AddVert(vert);

            //// Define triangles
            //vh.AddTriangle(0, 1, 2);
            ////vh.AddTriangle(2, 3, 0);

            // Draw X and Y axes
            DrawLine(vh, new Vector2(rect.xMin - (lineWidth / 2.0f), rect.yMin), new Vector2(rect.xMax, rect.yMin), lineWidth, Color.white);
            DrawLine(vh, new Vector2(rect.xMin, rect.yMin - (lineWidth / 2.0f)), new Vector2(rect.xMin, rect.yMax), lineWidth, Color.white);

            if(_displayGraphData != null)
            {
                List<Vector2> verts = new List<Vector2>();

                double xMin = _displayGraphData.DisplayGraphSettings.MinX;
                double xMax = _displayGraphData.DisplayGraphSettings.MaxX;
                double yMin = _displayGraphData.DisplayGraphSettings.MinY;
                double yMax = Math.Ceiling(_displayGraphData.DisplayGraphSettings.MaxY / 50) * 50;
                foreach (var item in _displayGraphData.LsPoints)
                {
                    Vector2 point = new Vector2(RemapToScale(item.X, xMin, xMax, rect.xMin, rect.xMax), RemapToScale(item.Y, yMin, yMax, rect.yMin, rect.yMax));
                    if(point.x < rect.xMax && point.y <= rect.yMax)
                    {
                        verts.Add(point);
                    }
                }

                for (int i = 0; i + 1 < verts.Count; i++)
                {
                    DrawLine(vh, verts[i], verts[i+1], lineWidth, Color.white);
                }
            }

            //// Draw scale markers on X and Y axes
            //for (int i = 1; i <= xScaleMarkers; i++)
            //{
            //    float x = i / (float)xScaleMarkers;
            //    DrawLine(vh, new Vector2(x, 0), new Vector2(x, -0.05f), Color.black);
            //}

            //for (int i = 1; i <= yScaleMarkers; i++)
            //{
            //    float y = i / (float)yScaleMarkers;
            //    DrawLine(vh, new Vector2(0, y), new Vector2(-0.05f, y), Color.black);
            //}
        }

        float RemapToScale(double value, double min, double max, double a, double b)
        {
            if (min == max)
                throw new ArgumentException($"'min'{min} and 'max'{max} can not be the same value");
            return (float)(a + ((value - min) * (b - a) / (max - min)));
            //return (float)((value - min) / (max - min));
        }

        private void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, float width, Color color)
        {
            UIVertex vert = UIVertex.simpleVert;
            vert.color = color;

            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * width * 0.5f;

            vert.position = start - normal;
            vh.AddVert(vert);

            vert.position = start + normal;
            vh.AddVert(vert);

            vert.position = end + normal;
            vh.AddVert(vert);

            vert.position = end - normal;
            vh.AddVert(vert);

            int index = vh.currentVertCount - 4;
            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index + 2, index + 3, index);
        }

        //private void CreateLabel()
        //{
        //    // Create a new GameObject for the label
        //    GameObject labelGameObject = new GameObject("Label");
        //    labelGameObject.transform.SetParent(transform, false);

        //    // Add Text component to the label GameObject
        //    Text labelTextComponent = labelGameObject.AddComponent<Text>();
        //    labelTextComponent.text = labelText;
        //    labelTextComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        //    labelTextComponent.fontSize = fontSize;
        //    labelTextComponent.color = textColor;
        //    labelTextComponent.alignment = TextAnchor.MiddleCenter;

        //    // Set RectTransform properties
        //    RectTransform rectTransform = labelGameObject.GetComponent<RectTransform>();
        //    rectTransform.anchorMin = new Vector2(0, 0);
        //    rectTransform.anchorMax = new Vector2(1, 1);
        //    rectTransform.sizeDelta = Vector2.zero;
        //    rectTransform.anchoredPosition = labelPosition;
        //}
    }
}
