using System;
using System.Collections;
using System.Collections.Generic;
using DVLib.Graphics;
using MyExtension.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class EditorVertex
    {
        private Vector2 _position;

        private float _radius = 8;

        private Color _color;

        private Color _selectedColor = new Color(1,0.57f,0);

        private Color _normalColor = new Color(0,0.6f,1);

        private bool _selected = false;

        public Vector2 Position => _position;

        private float _r2;

        public int HashCode { get; private set; }

        public int Id;

        public float Radius => _radius;

        public bool IsInPolygon = false;

        public event Action<EditorVertex,Vector2> OnPositionChanged;

        public bool Selected
        {
            get=>_selected;
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

        public EditorVertex(Vector2 position)
        {
            HashCode = GUID.Generate().GetHashCode();
            _color = _normalColor;
            _r2 = _radius * _radius;
            SetPosition(position);

        }
        public void Draw(MeshGenerationContext mc)
        {
            if (_selected)
            {
                GraphicX.Circle.DrawFill(mc, _position, _radius, _selectedColor, true,
                    2, Color.white, true);
            }
            else
            {
                if (IsInPolygon)
                {
                    GraphicX.Circle.DrawFill(mc, _position, _radius, _color, true,
                        2, Color.white, true);
                }
                else
                {
                    GraphicX.Circle.DrawFill(mc, _position, _radius, Color.magenta, true,
                        2, Color.white, true);
                }

            }

        }

        public void DrawInPolygon(MeshGenerationContext mc)
        {
            GraphicX.Circle.DrawFill(mc, _position, _radius, _color, true,
                2, Color.white, true);
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
            positionField?.SetValueWithoutNotify(_position);
            OnPositionChanged?.Invoke(this,_position);
        }

        public bool CheckHit(Vector2 position)
        {
            var r2 = (_position - position).sqrMagnitude;
            if (r2 < _r2)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{HashCode}: {Id} - {_position}";
        }

        private Vector2Field positionField;
        public VisualElement GetVisual()
        {
            VisualElement container = new VisualElement();

            Label labelId = new Label($"Id: {Id}");
            container.Add(labelId);

            positionField = new Vector2Field("Position");
            positionField.value = _position;
            positionField.RegisterValueChangedCallback((evt =>
            {
                _position = evt.newValue;
                OnPositionChanged?.Invoke(this,_position);
                EditorEventManager.EditorEvents.OnNeedUpdateCanvasEvent();
            }));

            container.Add(positionField);
            return container;
        }
    }
}

