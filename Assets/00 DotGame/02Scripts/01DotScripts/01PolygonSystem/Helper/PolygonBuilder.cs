﻿using Inwave.DongA.DotPuzzle.Entity;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.Manager;
using System.Collections.Generic;
using System.Linq;

namespace Inwave.DongA.DotPuzzle.Helper
{
    public class PolygonBuilder
    {
        public void BuildPolygon(Polygon polygon)
        {
            foreach (var vertice in polygon.vertices)
            {
                vertice.ChangeVerticeSprite(false, false);
                vertice.CurrentConnectCount += 2;
                if (!vertice.IsCanConnect())
                {
                    /*vertice.SetVerticeColor(Color.gray);*/
                    vertice.gameObject.SetActive(false);
                }
            }
            polygon.IsComplete = true;

            LineManager.Instance.DrawPolygon(polygon);
            polygon.VerticeCountText.gameObject.SetActive(false);

            EventManager.Instance.OnPolygonComplete?.Invoke(polygon);
        }
        

        public void CancelPolygon(Polygon polygon, bool isFault, bool isDrawLine = true)
        {
            LineManager.Instance.ClearDrawLine();
            foreach(var vertice in polygon.vertices)
            {
                vertice.ChangeVerticeSprite(false, isFault);
            }
            polygon.vertices.Clear();
            polygon.edges.Clear();

            if (isFault)
            {
                EventManager.Instance.OnTakeDame?.Invoke();
            }
        }
        
        public void CreateAllEdges(List<Polygon> allPolygons, List<Edge> allEdges)
        {
            foreach (Polygon polygon in allPolygons)
            {
                polygon.CreateEdge();
                foreach (var edge in polygon.edges)
                {
                    if (!allEdges.Exists(e => e.EqualsIgnoreDirection(edge)))
                    {
                        allEdges.Add(edge);
                    }
                }
            }
        }

        public void UpdateConnectionsForStep(List<Polygon> stepPolygons)
        {
            var stepVertices = stepPolygons.SelectMany(p => p.vertices).Distinct();
            foreach (var v in stepVertices)
            {
                v.MaxVerticeCanConnect = 0;
                v.CurrentConnectCount = 0;
                if (!v.gameObject.activeSelf)
                {
                    v.gameObject.SetActive(true);
                }
                v.ChangeVerticeSprite(false, false);
            }
            
            foreach (Polygon polygon in stepPolygons)
            {
                polygon.UpdateMaxVerticeConnect();
            }
        }
   
    }
}
