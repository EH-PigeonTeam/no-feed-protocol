using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NoFeedProtocol.Runtime.UI.Utilities
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UILineRenderer : Graphic
    {
        public List<Vector2> Points = new();
        public float Thickness = 10f;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (Points == null || Points.Count < 2)
                return;

            for (int i = 0; i < Points.Count - 1; i++)
            {
                Vector2 start = Points[i];
                Vector2 end = Points[i + 1];

                DrawLineSegment(vh, start, end);
            }
        }

        private void DrawLineSegment(VertexHelper vh, Vector2 start, Vector2 end)
        {
            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * (Thickness * 0.5f);

            UIVertex v0 = UIVertex.simpleVert;
            UIVertex v1 = UIVertex.simpleVert;
            UIVertex v2 = UIVertex.simpleVert;
            UIVertex v3 = UIVertex.simpleVert;

            v0.color = color;
            v1.color = color;
            v2.color = color;
            v3.color = color;

            v0.position = start - normal;
            v1.position = start + normal;
            v2.position = end + normal;
            v3.position = end - normal;

            int index = vh.currentVertCount;

            vh.AddVert(v0);
            vh.AddVert(v1);
            vh.AddVert(v2);
            vh.AddVert(v3);

            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 2, index + 3, index + 0);
        }
    }
}
