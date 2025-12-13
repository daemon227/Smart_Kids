using Inwave.DongA.DotPuzzle.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.Helper
{
    public class PolygonChecker
    {

        public bool AreEdgeListsSame(List<Edge> list1, List<Edge> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            var used = new HashSet<int>(); // tránh match trùng 2 lần

            foreach (var e1 in list1)
            {
                bool found = false;

                for (int i = 0; i < list2.Count; i++)
                {
                    if (used.Contains(i)) continue;

                    if (e1.EqualsIgnoreDirection(list2[i]))
                    {
                        used.Add(i);
                        found = true;
                        break;
                    }
                }

                if (!found) return false;
            }

            return true;
        }

        public Polygon GetSamePolygonByEdge(List<Polygon> allPolygons, Polygon tempPolygon)
        {
            foreach (var poly in allPolygons)
            {
                if (poly.IsComplete) continue;

                if (AreEdgeListsSame(poly.edges, tempPolygon.edges))
                    return poly;
            }

            return null;
        }

        public List<Polygon> GetDetectedPolygons(List<Polygon> allPolygons, List<Edge> allCompletedEdges,
            List<Edge> tempCompletedEdges)
        {
            var result = new List<Polygon>();

            foreach (var polygon in allPolygons)
            {
                if (polygon.IsComplete) continue;

                // take edge is not complete
                var notCompleted = polygon.edges
                    .Where(e => !allCompletedEdges.Any(c => c.EqualsIgnoreDirection(e)))
                    .ToList();

                if (ContainsAllEdges(notCompleted, tempCompletedEdges))
                    result.Add(polygon);
            }

            return result;
        }


        public bool ContainsAllEdges(List<Edge> list1, List<Edge> list2)
        {
            if (list2.Count < list1.Count)
                return false;

            var used = new HashSet<int>();

            foreach (var e1 in list1)
            {
                bool found = false;
                for (int i = 0; i < list2.Count; i++)
                {
                    if (used.Contains(i)) continue;

                    if (e1.EqualsIgnoreDirection(list2[i]))
                    {
                        used.Add(i);
                        found = true;
                        break;
                    }
                }

                if (!found) return false;
            }

            return true;
        }

        public bool DoAllTempEdgesBelongToSamePolygon(List<Edge> tempEdges, List<Polygon> allPolygons)
        {
            if (tempEdges == null || tempEdges.Count == 0) return false;

            Edge firstEdge = tempEdges[0];

            foreach (var poly in allPolygons)
            {
                if (poly.edges.Exists(e => e.EqualsIgnoreDirection(firstEdge)))
                {
                    bool allMatch = true;
                    foreach (var tEdge in tempEdges)
                    {
                        if (!poly.edges.Exists(e => e.EqualsIgnoreDirection(tEdge)))
                        {
                            allMatch = false;
                            break;
                        }
                    }

                    if (allMatch) return true;
                }
            }
            return false;
        }
    }
}
