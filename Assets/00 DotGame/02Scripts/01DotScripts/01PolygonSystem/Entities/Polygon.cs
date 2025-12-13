using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.Entity
{
    public class Polygon : MonoBehaviour
    {
        public int polygonId;
        public int flowerId;
        public List<Vertice> vertices;
        public List<Edge> edges = new List<Edge>();
        [HideInInspector] public List<GameObject> flowerObjects; 
        
        private TextMeshProUGUI verticeCountText;
        private bool isComplete = false;

        public bool IsComplete { get => isComplete; set => isComplete = value; }
        public TextMeshProUGUI VerticeCountText { get => verticeCountText; set => verticeCountText = value; }

        public Vector2 GetCentroid()
        {
            float A = 0;
            float Cx = 0;
            float Cy = 0;
            int n = vertices.Count;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                float cross = vertices[i].transform.position.x * vertices[j].transform.position.y - vertices[j].transform.position.x * vertices[i].transform.position.y;
                A += cross;
                Cx += (vertices[i].transform.position.x + vertices[j].transform.position.x) * cross;
                Cy += (vertices[i].transform.position.y + vertices[j].transform.position.y) * cross;
            }

            A *= 0.5f;
            float factor = 1 / (6 * A);
            return new Vector2(Cx * factor, Cy * factor);
        }

        public bool IsCentroidVisible()
        {
            var centroid = GetCentroid();
            Vector3 viewPos = Camera.main.WorldToViewportPoint(centroid);

            return (viewPos.x >= 0f && viewPos.x <= 1f &&
                    viewPos.y >= 0f && viewPos.y <= 1f &&
                    viewPos.z > 0f);
        }

        public void CreateEdge()
        {
            edges.Clear();
            int n = vertices.Count;
            for (int i = 0; i < n; i++)
            {
                var a = vertices[i];
                var b = vertices[(i + 1) % n];
                edges.Add(new Edge(a, b));
            }
        }
        public void AddEdgeIfNotExist(Edge edge)
        {
            if (!edges.Any(e => e.EqualsIgnoreDirection(edge)))
            {
                edges.Add(edge);
            }
        }
        public void ClearTemp()
        {
            vertices.Clear();
            edges.Clear();
        }
        public void UpdateMaxVerticeConnect()
        {     
            foreach (var edge in edges)
            {
                edge.a.MaxVerticeCanConnect++;
                edge.b.MaxVerticeCanConnect++;
            }
        }
        public bool IsInsidePolygonBase(Vector2 point)
        {
            int count = this.vertices.Count;
            bool inside = false;

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                Vector2 vi = this.vertices[i].transform.position;
                Vector2 vj = this.vertices[j].transform.position;

                bool intersect =
                    ((vi.y > point.y) != (vj.y > point.y)) &&
                    (point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y) + vi.x);

                if (intersect) inside = !inside;
            }
            return inside;
        }
    }
}

