using System;
using System.Collections;
using System.Collections.Generic;
using DVLib.Graphics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class EditorStepData
    {
        private bool _selected = false;

        private Color _color;

        private Color _selectedColor = new Color(1, 0.57f, 0);

        private Color _normalColor = new Color(0, 0.6f, 1);

        public List<EditorPolygon> Polygons { get; private set; } = new();

        public int Id;

        public int HashCode { get; private set; }

        public event Action<EditorPolygon> OnPolygonAdded;

        public event Action<EditorPolygon> OnPolygonRemoved;

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (_selected)
                {
                    _color = _selectedColor;
                }
                else
                {
                    _color = _normalColor;
                }
            }
        }

        public EditorStepData()
        {
            HashCode = GUID.Generate().GetHashCode();
        }

        public void Draw(MeshGenerationContext mc)
        {
            if (_selected)
            {
                foreach (var polygon in Polygons)
                {
                    GraphicX.Polygon.DrawFill(mc, polygon.MainPolygon, _selectedColor,
                        true, 2, Color.cyan);
                }
            }
        }

        private ScrollView _scrollView;
        public VisualElement GetVisual()
        {
            VisualElement container = new VisualElement();

            Label labelId = new Label($"Step: {Id}");
            container.Add(labelId);

            _scrollView = new ScrollView(ScrollViewMode.Vertical);
            container.Add(_scrollView);

            foreach (var polygon in Polygons)
            {
                EditorPolygonItem item = new EditorPolygonItem(polygon);
                item.OnSelected += PolygonItem_OnSelected;
                item.OnRemoved += PolygonItem_OnRemoved;
                _scrollView.Add(item);
            }

            return container;
        }

        private void PolygonItem_OnRemoved(EditorPolygonItem item)
        {
            RemovePolygon(item.Polygon);
            EditorEventManager.EntityEvents.OnPolygonRemovedToStepEvent(this,item.Polygon);
        }

        private EditorPolygonItem _selectedPolygon;

        private void PolygonItem_OnSelected(EditorPolygonItem item)
        {
            if (_selectedPolygon != null)
            {
                _selectedPolygon.Normal();
            }
            _selectedPolygon = item;
            _selectedPolygon.Selected();

            EditorEventManager.EntityEvents.OnPolygonItemSelectedEvent(item);
        }

        public void AddPolygon(EditorPolygon polygon)
        {
            if (!Polygons.Contains(polygon))
            {
                polygon.IsInStep = true;
                Polygons.Add(polygon);
                OnPolygonAdded?.Invoke(polygon);
            }
        }

        public void RemovePolygon(EditorPolygon polygon)
        {
            polygon.IsInStep = false;
            Polygons.Remove(polygon);
            OnPolygonRemoved?.Invoke(polygon);
        }

        public void ClearPolygons()
        {
            foreach (var polygon in Polygons)
            {
                polygon.IsInStep = false;
            }
            Polygons.Clear();
        }

        public bool ContainsPolygon(EditorPolygon polygon)
        {
            return Polygons.Contains(polygon);
        }

        public bool CheckMouseHit(Vector2 position,out EditorPolygon hitPolygon)
        {
            foreach (var polygon in Polygons)
            {
                if (polygon.CheckMouseClick(position, out var vertex))
                {
                    hitPolygon = polygon;
                    return true;
                }
            }

            hitPolygon = null;
            return false;
        }
    }
}

