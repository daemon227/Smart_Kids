using DG.Tweening;
using Inwave.DongA.DotPuzzle.Entity;
using Inwave.DongA.DotPuzzle.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.Manager
{
    public class LineManager : MonoBehaviour
    {
        public static LineManager Instance;

        public LineRenderer drawLinePrefab;
        public LineRenderer suggetLinePrefab;

        private LineRenderer drawLine;
        private LineRenderer suggetLine;
        private Polygon polygonShowingSugget;
        private bool isShowingSuggetLine = false;

        public bool IsShowingSuggetLine
        {
            get => isShowingSuggetLine;
            set => isShowingSuggetLine = value;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(Instance);
            }
            
        }

        void Start()
        {
            drawLine = CreateLine(drawLinePrefab);
            suggetLine = CreateLine(suggetLinePrefab);
        }
        
        public LineRenderer CreateLine(LineRenderer linePrefab)
        {
            var line = Instantiate(linePrefab);
            line.positionCount = 0;
            return line;
        }
        public void DrawPolygon(Polygon polygon)
        {
            if(polygonShowingSugget != null && polygonShowingSugget==polygon)
            {
                ClearSuggetLine();
            }
            ClearDrawLine();
        }
        public void DrawLine(List<Vertice> vertices)
        {
            if(vertices.Count < 1) return;
            drawLine.positionCount = vertices.Count;
            for(int i = 0;i < vertices.Count;i++)
            {
                drawLine.SetPosition(i, vertices[i].transform.position);
            }
        }
        public void DrawLineWithMouse(List<Vertice> vertices, Vector3 mousePosition)
        {
            if (vertices.Count < 1) return;
            drawLine.positionCount = vertices.Count + 1;
            for (int i = 0; i < vertices.Count; i++)
            {
                drawLine.SetPosition(i, vertices[i].transform.position);
            }
            Vector3 endPos = mousePosition;
            endPos.z = vertices[vertices.Count - 1].transform.position.z; 
            drawLine.SetPosition(vertices.Count, endPos);
        }
        public void ClearDrawLine()
        {
            drawLine.positionCount = 0;
        }
        public IEnumerator DrawSuggetLine(Polygon polygon)
        {
            if (isShowingSuggetLine) yield break;
            var vertices = polygon.vertices;
            suggetLine.positionCount = vertices.Count + 1;
            for (int i = 0; i < vertices.Count; i++)
            {
                suggetLine.SetPosition(i, vertices[i].transform.position);
            }
            suggetLine.SetPosition(vertices.Count, vertices[0].transform.position);
            isShowingSuggetLine = true;
            
            while (isShowingSuggetLine)
            {
                suggetLine.enabled = true;
                yield return new WaitForSeconds(1f);
                suggetLine.enabled = false;
                yield return new WaitForSeconds(0.4f);
                yield return null;
            }
        }
        public void ClearSuggetLine()
        {
            isShowingSuggetLine = false;
            suggetLine.positionCount = 0;
        }

        public void ShowDrawSuggetLine(Polygon polygon)
        {
            polygonShowingSugget = polygon;
            StartCoroutine(DrawSuggetLine(polygon));
        }
    }
}

