
using System.Collections.Generic;
using UnityEngine;

public class EdgeManager
{
    public void MergeEdgesToCompledEdges(List<Edge> allEdges, List<Edge> edges)
    {
        foreach (var edge in edges)
        {
            if (!allEdges.Contains(edge))
            {
                allEdges.Add(edge);
            }
        }          
        edges.Clear();
    }

    public bool CheckEdgeInteraction(List<Edge> allEdges, List<Edge> currentEdges, Edge edge)
    {
        return CheckEdge(currentEdges, edge) || CheckEdge(allEdges, edge);
    }

    public bool CheckEdge(List<Edge> edges, Edge currentEdge)
    {
        foreach (var edge in edges)
        {
            if (edge.a == currentEdge.a || edge.a == currentEdge.b ||
                edge.b == currentEdge.a || edge.b == currentEdge.b)
                continue;

            if (LinesIntersect(edge.a.transform.position, edge.b.transform.position,
                               currentEdge.a.transform.position, currentEdge.b.transform.position))
            {
                return true;
            }
        }
        return false;
    }

    public static bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d1 = Cross(p4 - p3, p1 - p3);
        float d2 = Cross(p4 - p3, p2 - p3);
        float d3 = Cross(p2 - p1, p3 - p1);
        float d4 = Cross(p2 - p1, p4 - p1);

        // Trường hợp giao nhau chuẩn (khác phía)
        if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
            ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
            return true;
        
        // Collinear + overlap
        if (Mathf.Approximately(d1, 0) && OnSegment(p3, p4, p1)) return true;
        if (Mathf.Approximately(d2, 0) && OnSegment(p3, p4, p2)) return true;
        if (Mathf.Approximately(d3, 0) && OnSegment(p1, p2, p3)) return true;
        if (Mathf.Approximately(d4, 0) && OnSegment(p1, p2, p4)) return true;

        return false;
    }

    private static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

    private static bool OnSegment(Vector2 a, Vector2 b, Vector2 p)
    {
        return p.x >= Mathf.Min(a.x, b.x) && p.x <= Mathf.Max(a.x, b.x) &&
               p.y >= Mathf.Min(a.y, b.y) && p.y <= Mathf.Max(a.y, b.y);
    }

}
