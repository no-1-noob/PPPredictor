using BeatSaberMarkupLanguage;
using IPA.Config.Data;
using IPA.Utilities.Async;
using PPPredictor.Data.DisplayInfos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PPPredictor.UI.Graph
{
    internal class PPGraph : Graphic
    {
        public string labelText = "Graph Label";
        public Vector2 labelPosition = new Vector2(0.5f, 0.5f); // Center of the graphic
        public int fontSize = 14;
        public Color textColor = Color.black;
        public int xScaleMarkers = 10;
        public int yScaleMarkers = 10;
        private float lineWidth = 0.5f;
        private DisplayGraphInfo _displayGraphInfo;
        private DisplayPPInfo _displayPPInfo;

        private IEnumerator coroutine;

        private List<UIVertex> graphVertices = new List<UIVertex>();
        private List<UIVertex> positionVertices = new List<UIVertex>();

        private bool isDirty = true;

        public DisplayGraphInfo DisplayGraphInfo
        {
            set
            {
                //Plugin.DebugPrint("DisplayGraphInfo");
                _displayGraphInfo = value;
                RefreshGraph();
            }
        }

        internal DisplayPPInfo DisplayPPInfo { set
            {
                _displayPPInfo = value;
                RefreshPosition();
            }
        }

        protected override void Start()
        {
            base.Start();
            Plugin.DebugPrint("protected override void Start");
            coroutine = RefreshCheck();
            StartCoroutine(coroutine);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {   
            vh.Clear();
            List<UIVertex> lsCombined = new List<UIVertex>(graphVertices);
            lsCombined.AddRange(positionVertices);
            vh.AddUIVertexTriangleStream(lsCombined);
            isDirty = false;
        }

        private void RefreshGraph()
        {
            DrawGraph();
            RefreshPosition();
        }

        private void RefreshPosition()
        {
            DrawPosition();
            isDirty = true;
        }

        private IEnumerator RefreshCheck()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                if (isDirty)
                {
                    SetVerticesDirty();
                }
            }
        }

        private void DrawGraph()
        {
            graphVertices.Clear();
            Rect rect = GetPixelAdjustedRect();
            Plugin.DebugPrint($"OnPopulateMesh rect x{rect.xMin} {rect.xMax} rect y {rect.yMin} {rect.yMax}");

            // Draw X and Y axes
            DrawLine(graphVertices, new Vector2(rect.xMin - (lineWidth / 2.0f), rect.yMin), new Vector2(rect.xMax, rect.yMin), lineWidth, Color.white);
            DrawLine(graphVertices, new Vector2(rect.xMin, rect.yMin - (lineWidth / 2.0f)), new Vector2(rect.xMin, rect.yMax), lineWidth, Color.white);

            if (_displayGraphInfo != null)
            {
                List<Vector2> verts = new List<Vector2>();
                double xMin, xMax, yMin, yMax;
                GetMinMaxValues(out xMin, out xMax, out yMin, out yMax);
                foreach (var item in _displayGraphInfo.LsPoints)
                {
                    Vector2 point = new Vector2(RemapToScale(item.X, xMin, xMax, rect.xMin, rect.xMax), RemapToScale(item.Y, yMin, yMax, rect.yMin, rect.yMax));
                    if (point.x < rect.xMax && point.y <= rect.yMax)
                    {
                        verts.Add(point);
                    }
                }

                for (int i = 0; i + 1 < verts.Count; i++)
                {
                    DrawLine(graphVertices, verts[i], verts[i + 1], lineWidth, Color.white);
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

        private void GetMinMaxValues(out double xMin, out double xMax, out double yMin, out double yMax)
        {
            xMin = _displayGraphInfo.DisplayGraphSettings.MinX;
            xMax = _displayGraphInfo.DisplayGraphSettings.MaxX;
            yMin = _displayGraphInfo.DisplayGraphSettings.MinY;
            yMax = Math.Ceiling(_displayGraphInfo.DisplayGraphSettings.MaxY / 50) * 50;
        }

        private void DrawPosition()
        {
            positionVertices.Clear();
            if (_displayGraphInfo != null)
            {
                Rect rect = GetPixelAdjustedRect();
                double xMin, xMax, yMin, yMax;
                GetMinMaxValues(out xMin, out xMax, out yMin, out yMax);
                Vector2 point = new Vector2(RemapToScale(_displayPPInfo.PercentValue, xMin, xMax, rect.xMin, rect.xMax), RemapToScale(_displayPPInfo.PPRawValue, yMin, yMax, rect.yMin, rect.yMax));
                DrawLine(positionVertices, new Vector2(rect.xMin - (lineWidth / 2.0f), point.y), new Vector2(point.x, point.y), lineWidth / 2f, Color.white);
                DrawLine(positionVertices, new Vector2(point.x, rect.yMin - (lineWidth / 2.0f)), new Vector2(point.x, point.y), lineWidth / 2f, Color.white);
            }
        }

        float RemapToScale(double value, double min, double max, double a, double b)
        {
            if (min == max)
                throw new ArgumentException($"'min'{min} and 'max'{max} can not be the same value");
            return (float)(a + ((value - min) * (b - a) / (max - min)));
            //return (float)((value - min) / (max - min));
        }

        private void DrawLine(List<UIVertex> vertices, Vector2 start, Vector2 end, float width, Color color)
        {
            UIVertex vert = UIVertex.simpleVert;
            vert.color = color;

            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * width * 0.5f;

            Vector2[] lineVertices = new Vector2[4];
            lineVertices[0] = start - normal;
            lineVertices[1] = start + normal;
            lineVertices[2] = end + normal;
            lineVertices[3] = end - normal;

            List<UIVertex> lsVertex = new List<UIVertex>();

            vert.position = lineVertices[0];
            lsVertex.Add(vert);

            vert.position = lineVertices[1];
            lsVertex.Add(vert);

            vert.position = lineVertices[2];
            lsVertex.Add(vert);

            vert.position = lineVertices[3];
            lsVertex.Add(vert);

            int index = lsVertex.Count - 4;
            vertices.AddRange(new UIVertex[] { lsVertex[index], lsVertex[index + 1], lsVertex[index + 2] });
            vertices.AddRange(new UIVertex[] { lsVertex[index + 2], lsVertex[index + 3], lsVertex[index] });


        }
    }
}
