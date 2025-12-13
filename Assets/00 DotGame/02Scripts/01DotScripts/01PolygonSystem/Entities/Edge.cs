using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Vertice a;
    public Vertice b;

    public Edge(Vertice a, Vertice b)
    {
        this.a = a;
        this.b = b;
    }

    public Vector2 PosA => a.transform.position;
    public Vector2 PosB => b.transform.position;

    public bool EqualsIgnoreDirection(Edge other)
    {
        return (a == other.a && b == other.b)
            || (a == other.b && b == other.a);
    }

    public override int GetHashCode()
    {
        // hash không phân biệt hướng
        int h1 = a.GetHashCode() ^ b.GetHashCode();
        int h2 = b.GetHashCode() ^ a.GetHashCode();
        return h1 ^ h2;
    }
}
