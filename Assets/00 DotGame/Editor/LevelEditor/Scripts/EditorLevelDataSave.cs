using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class EditorLevelDataSave
    {
        public List<EditorVertex> Vertices = new ();

        public List<EditorPolygon> Polygons = new ();

        public List<EditorStepData> Steps = new ();

        public int MaxHp = 3;

        public int InsectLifeTime = 15;


        public EditorLevelDataSave()
        {
            EditorEventManager.EntityEvents.OnVertexSelected += EntityEvents_OnVertexSelected;
            EditorEventManager.EntityEvents.OnVertexRemoved += EntityEvents_OnVertexRemoved;

            EditorEventManager.EntityEvents.OnPolygonSelected += EntityEvents_OnPolygonSelected;
            EditorEventManager.EntityEvents.OnPolygonRemoved += EntityEvents_OnPolygonRemoved;
        }

        ~EditorLevelDataSave()
        {
            EditorEventManager.EntityEvents.OnVertexSelected -= EntityEvents_OnVertexSelected;
            EditorEventManager.EntityEvents.OnVertexRemoved -= EntityEvents_OnVertexRemoved;

            EditorEventManager.EntityEvents.OnPolygonSelected -= EntityEvents_OnPolygonSelected;
            EditorEventManager.EntityEvents.OnPolygonRemoved -= EntityEvents_OnPolygonRemoved;
        }

        private void EntityEvents_OnPolygonRemoved(EditorPolygon polygon)
        {
            RemovePolygon(polygon);

            if (_dicPolygon != null && _dicPolygon.ContainsKey(polygon))
            {
                var item = _dicPolygon[polygon];
                if (_polygonScrollView.Contains(item))
                {
                    _polygonScrollView.Remove(item);
                }
                _dicPolygon.Remove(polygon);
            }
        }

        private void EntityEvents_OnPolygonSelected(EditorPolygon polygon)
        {
            if (polygon != null && _dicPolygon != null && _dicPolygon.ContainsKey(polygon))
            {
                if (_selectedPolygonPropertyItem != null)
                {
                    _selectedPolygonPropertyItem.Normal();
                }

                var item = _dicPolygon[polygon];
                _selectedPolygonPropertyItem = item;
                _selectedPolygonPropertyItem.Selected();

                if (_polygonScrollView != null && _polygonScrollView.Contains(item))
                {
                    _polygonScrollView.ScrollTo(item);
                }
            }
            else if(_polygonScrollView!=null)
            {
                foreach (var child in _polygonScrollView.Children())
                {
                    var item = child as EditorPolygonPropertyItem;
                    item.Normal();
                }
            }
        }

        private void EntityEvents_OnVertexRemoved(EditorVertex vertex)
        {
            RemoveVertex(vertex);
            if (_dicVertices != null && _dicVertices.ContainsKey(vertex))
            {
                var item = _dicVertices[vertex];
                if (_vertexScrollView.Contains(item))
                {
                    _vertexScrollView.Remove(item);
                }
                _dicVertices.Remove(vertex);
            }
        }

        private void EntityEvents_OnVertexSelected(EditorVertex vertex)
        {
            if (vertex!=null && _dicVertices!=null && _dicVertices.ContainsKey(vertex))
            {
                if (_selectedVertexPropertyItem != null)
                {
                    _selectedVertexPropertyItem.Normal();
                }
                var item = _dicVertices[vertex];
                _selectedVertexPropertyItem = item;
                _selectedVertexPropertyItem.Selected();

                if (_vertexScrollView != null && _vertexScrollView.Contains(item))
                {
                    _vertexScrollView.ScrollTo(item);
                }
            }
            else if(_vertexScrollView!=null)
            {
                foreach (var child in _vertexScrollView.Children())
                {
                    var item = child as EditorVertexPropertyItem;
                    item.Normal();
                }
            }
        }

        public void Draw(MeshGenerationContext mc)
        {
            foreach (var polygon in Polygons)
            {
                polygon.Draw(mc);
            }

            foreach (var step in Steps)
            {
                step.Draw(mc);
            }

            foreach (var polygon in Polygons)
            {
                polygon.DrawSelected(mc);
            }

            foreach (var vertex in Vertices)
            {
                vertex.Draw(mc);
            }

            DrawText(mc);
        }

        private void DrawText(MeshGenerationContext mc)
        {
            foreach (var polygon in Polygons)
            {
                polygon.DrawText(mc);
            }
        }

        public void ClearVertices()
        {
            Vertices.Clear();
        }

        public void ClearPolygons()
        {
            Polygons.Clear();
        }

        public void Clear()
        {
            ClearVertices();
            ClearPolygons();
        }

        public void AddVertexToData(EditorVertex vertex)
        {
            if(Vertices.Contains(vertex)) 
                return;

            Vertices.Add(vertex);
        }

        public void AddVertex(EditorVertex vertex)
        {
            if (Vertices.Contains(vertex))
                return;

            vertex.Id = FindAValidVertexId();

            Vertices.Insert(vertex.Id, vertex);

            var item = CreateNewVertexPropertyItem(vertex);

            if (_vertexScrollView != null)
            {
                _vertexScrollView.Insert(vertex.Id, item);
                _vertexScrollView.ScrollTo(item);
            }

            if (_dicVertices != null)
            {
                _dicVertices.Add(vertex, item);
            }
        }

        public int FindAValidVertexId()
        {
            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                if (Vertices[i].Id != i)
                {
                    return i;
                }
            }
            return Vertices.Count;
        }

        public void RemoveVertex(EditorVertex vertex)
        {
            if (Vertices.Contains(vertex))
            {
                Vertices.Remove(vertex);
                foreach (var polygon in Polygons)
                {
                    polygon.RemoveVertex(vertex);
                }

                Polygons.RemoveAll((polygon => polygon.IsEmpty));
            }

        }

        public void AddPolygonToData(EditorPolygon polygon)
        {
            if(Polygons.Contains(polygon)) 
                return;

            Polygons.Add(polygon);
        }

        public void AddPolygon(EditorPolygon polygon)
        {
            if (Polygons.Contains(polygon))
                return;

            polygon.Id = FindAValidPolygonId();

            Polygons.Insert(polygon.Id, polygon);

            var item = CreateNewPolygonPropertyItem(polygon);

            if (_polygonScrollView != null)
            {
                _polygonScrollView.Insert(polygon.Id, item);
                _polygonScrollView.ScrollTo(item);
            }

            if (_dicPolygon != null)
            {
                _dicPolygon.Add(polygon, item);
            }
        }

        public int FindAValidPolygonId()
        {
            for (int i = 0; i < Polygons.Count-1; i++)
            {
                if (Polygons[i].Id != i)
                {
                    return i;
                }
            }
            return Polygons.Count;
        }

        public void RemovePolygon(EditorPolygon polygon)
        {
            if (Polygons.Contains(polygon))
            {
                polygon.ClearVertices();
                Polygons.Remove(polygon);
                if (polygon.IsInStep)
                {
                    foreach (var step in Steps)
                    {
                        if (step.ContainsPolygon(polygon))
                        {
                            step.RemovePolygon(polygon);
                        }
                    }
                }

            }

        }

        public void AddStepToData(EditorStepData step)
        {
            if (Steps.Contains(step))
            {
                return;
            }
            Steps.Add(step);
        }

        public void AddStep(EditorStepData step)
        {
            if (Steps.Contains(step))
            {
                return;
            }

            step.Id = FindAValidStepId();
            Steps.Insert(step.Id, step);
        }

        public int FindAValidStepId()
        {
            for (int i = 0; i < Steps.Count - 1; i++)
            {
                if (Steps[i].Id != i)
                {
                    return i;
                }
            }
            return Steps.Count;
        }

        public void RemoveStep(EditorStepData step)
        {
            if (Steps.Contains(step))
            {
                step.ClearPolygons();
                Steps.Remove(step);
            }

        }

        public bool CheckHitVertex(Vector2 mousePosition, out EditorVertex vertex)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (Vertices[i].CheckHit(mousePosition))
                {
                    vertex = Vertices[i];
                    return true;
                }
            }
            vertex = null;
            return false;
        }

        public bool CheckHitPolygon(Vector2 mousePosition, out EditorPolygon hitPolygon, out EditorVertex hitVertex)
        {
            foreach (var editorPolygon in Polygons)
            {
                if (editorPolygon.CheckMouseClick(mousePosition, out var vertex))
                {
                    hitPolygon = editorPolygon;
                    hitVertex = vertex;
                    return true;
                }
            }
            hitPolygon = null;
            hitVertex = null;
            return false;
        }

        public bool CheckHitStep(Vector2 mousePosition, out EditorStepData hitStep, out EditorPolygon hitPolygon)
        {
            foreach (var step in Steps)
            {
                if (step.CheckMouseHit(mousePosition, out var polygon))
                {
                    hitPolygon = polygon;
                    hitStep = step;
                    return true;
                }
            }
            hitPolygon = null;
            hitStep = null;
            return false;
        }

        #region Polygon Property

        private EditorPolygonPropertyItem _selectedPolygonPropertyItem;

        private ScrollView _polygonScrollView;

        private Dictionary<EditorPolygon, EditorPolygonPropertyItem> _dicPolygon; 
        public VisualElement GetPolygonsVisual()
        {
            _vertexScrollView = null;
            _polygonScrollView?.Clear();
            _dicVertices = null;
            _dicPolygon = new();

            VisualElement container = new VisualElement();

            Label label = new Label("Polygons:");
            container.Add(label);

            _polygonScrollView = new ScrollView(ScrollViewMode.Vertical);
            container.Add(_polygonScrollView);

            foreach (var polygon in Polygons)
            {
                var item = CreateNewPolygonPropertyItem(polygon);
                _polygonScrollView.Add(item);
                _dicPolygon.Add(polygon, item);
            }

            return container;
        }


        private EditorPolygonPropertyItem CreateNewPolygonPropertyItem(EditorPolygon polygon)
        {
            var item = new EditorPolygonPropertyItem(polygon);
            item.OnSelected += Item_OnSelected;
            item.OnRemove += Item_OnRemove;
            return item;
        }

        private void Item_OnRemove(EditorPolygonPropertyItem item)
        {
            EditorEventManager.EntityEvents.OnPolygonRemovedEvent(item.Polygon);
        }



        private void Item_OnSelected(EditorPolygonPropertyItem item)
        {
            EditorEventManager.EntityEvents.OnPolygonSelectedEvent(item.Polygon);
        }
        #endregion


        #region Vertex Property

        private EditorVertexPropertyItem _selectedVertexPropertyItem;

        private ScrollView _vertexScrollView;

        private Dictionary<EditorVertex, EditorVertexPropertyItem> _dicVertices;

        public VisualElement GetVerticesVisual()
        {
            _polygonScrollView = null;
            _vertexScrollView?.Clear();
            _dicVertices = new();

            VisualElement container = new VisualElement();

            Label label = new Label("Vertices:");
            container.Add(label);

            _vertexScrollView = new ScrollView(ScrollViewMode.Vertical);
            container.Add(_vertexScrollView);

            foreach (var vertex in Vertices)
            {
                var item = CreateNewVertexPropertyItem(vertex);
                _vertexScrollView.Add(item);
                _dicVertices.Add(vertex, item);
            }

            return container;
        }

        EditorVertexPropertyItem CreateNewVertexPropertyItem(EditorVertex vertex)
        {
            var item = new EditorVertexPropertyItem(vertex);
            item.OnSelected += Item_OnSelected;
            item.OnRemove += Item_OnRemove;
            return item;
        }
        private void Item_OnRemove(EditorVertexPropertyItem item)
        {
            EditorEventManager.EntityEvents.OnVertexRemovedEvent(item.Vertex);
        }

        

        private void Item_OnSelected(EditorVertexPropertyItem item)
        {
            EditorEventManager.EntityEvents.OnVertexSelectedEvent(item.Vertex);
        }

        #endregion

    }
}

