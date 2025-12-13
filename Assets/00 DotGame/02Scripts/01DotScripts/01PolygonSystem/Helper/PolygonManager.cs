
using System.Collections.Generic;
using Inwave.DongA.DotPuzzle.Entity;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.Manager
{
    public class PolygonManager
    {
        public Polygon GetRandomCompletedPolygon(List<Polygon> polygons)
        {
            List<Polygon> completedPolygons = polygons.FindAll(p => p.IsComplete);
            
            if (completedPolygons.Count == 0)
                return null;
            
            int index = Random.Range(0, completedPolygons.Count);
            return completedPolygons[index];
        }

        public (Vector2 xRange, Vector2 yRange) GetGlobalPolygonRange()
        {
            Vector2 xRange = new Vector2(float.MaxValue, float.MinValue);
            Vector2 yRange = new Vector2(float.MaxValue, float.MinValue);

            var polygons = new List<Polygon>();
            for (int i = 0; i <= GameManager.Instance.currentStepIndex; i++)
            {
                polygons.AddRange(GameManager.Instance.levelData.allStep[i].polygons);
            }
            foreach (var polygon in polygons)
            {
                foreach (var vertice in polygon.vertices)
                {
                    var pos = vertice.transform.position;

                    xRange.x = Mathf.Min(xRange.x, pos.x);
                    xRange.y = Mathf.Max(xRange.y, pos.x);

                    yRange.x = Mathf.Min(yRange.x, pos.y);
                    yRange.y = Mathf.Max(yRange.y, pos.y);
                }
            }

            return (xRange, yRange);
        }

        public Vector2 GetGlobalCentroid()
        {
            var polygons = GameManager.Instance.levelData.GetAllPolygons();

            if (polygons == null || polygons.Count == 0)
                return Vector2.zero;

            Vector2 sum = Vector2.zero;

            foreach (var polygon in polygons)
            {
                sum += polygon.GetCentroid();
            }

            return sum / polygons.Count;
        }

    }
}

