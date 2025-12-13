using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.Helper
{
    public class PathFindingAlgorithm
    {
        public bool IsPathExist(Vertice start, Vertice end, List<Edge> edges)
        {
            var visited = new HashSet<Vertice>();
            var queue = new Queue<Vertice>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == end) return true;

                foreach (var edge in edges)
                {
                    Vertice neighbor = null;
                    if (edge.a == current) neighbor = edge.b;
                    else if (edge.b == current) neighbor = edge.a;

                    if (neighbor != null && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return false;
        }
    } 
}

