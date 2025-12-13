using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DotPuzzle.Editor
{
    public class PolygonUtils
    {
        public static bool PointInPolygon(Vector2 point, Vector2[] polygon)
        {
            int n = polygon.Length;
            bool inside = false;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                bool intersect = ((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                                 (point.x < (polygon[j].x - polygon[i].x) *
                                     (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x);

                if (intersect)
                    inside = !inside;
            }

            return inside;
        }

    }
}

