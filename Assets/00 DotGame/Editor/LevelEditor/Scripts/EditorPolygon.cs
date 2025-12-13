using System.Collections;
using System.Collections.Generic;
using DVLib.Graphics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class EditorPolygon
    {
        public List<EditorVertex> Vertices = new ();

        private bool _selected = false;

        private Color _color;

        private Color _strokeColor;

        private Color _normalColor = new Color(0, 1, 0, 0.2f);

        private Color _orphanColor = new Color(0.85f, 0, 0, 0.7f);

        private Color _selectedColor = new Color(0, 0.62f, 1);

        private Color _normalStrokeColor = Color.green;

        private Color _selectedStrokeColor = Color.yellow;

        private Vector2 _textPosition;

        private Vector2[] _mainPolygon;

        public int Id;

        public int FlowerId { get; private set; }

        public Vector2[] MainPolygon => _mainPolygon;

        public int HashCode { get; private set; }

        public bool IsInStep = false;

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

        public bool IsEmpty => Vertices.Count == 0;
        public EditorPolygon(int flowerId)
        {
            HashCode = GUID.Generate().GetHashCode();
            _mainPolygon = new Vector2[1];
            FlowerId = flowerId;
        }

        
        public void Draw(MeshGenerationContext mc)
        {
            if (!Selected)
            {
                Color strokeColor = _normalStrokeColor;
                float strokeWidth = 2;
                if (Vertices.Count < 3)
                {
                    strokeColor = Color.red;
                    strokeWidth = 4;
                }

                if (IsInStep)
                {
                    GraphicX.Polygon.DrawFill(mc, _mainPolygon, _normalColor,
                        true, strokeWidth, strokeColor);
                }
                else
                {
                    GraphicX.Polygon.DrawFill(mc, _mainPolygon, Color.magenta,
                        true, strokeWidth, strokeColor);
                }
            }
        }

        public void DrawSelected(MeshGenerationContext mc)
        {
            if (Selected)
            {
                Color strokeColor = _selectedStrokeColor;
                if (Vertices.Count < 3)
                {
                    strokeColor = Color.red;
                }
                GraphicX.Polygon.DrawFill(mc, _mainPolygon, _selectedColor,
                    true, 4, strokeColor);
            }

        }
        public void DrawText(MeshGenerationContext mc)
        {
            Color textColor = Color.white;
            if (Selected)
            {
                textColor = Color.blue;
                for (int i = 0; i < Vertices.Count; i++)
                {
                    mc.DrawText(i.ToString(), Vertices[i].Position + new Vector2(-3,-7), 11, Color.white);
                }
            }

            
            if (Vertices.Count < 3)
            {
                textColor = Color.red;
            }
            mc.DrawText($"Id: {Id}\nFid: {FlowerId}", _textPosition, 10, textColor);
        }

        public void AddVertex(EditorVertex vertex)
        {
            if (!Vertices.Contains(vertex))
            {
                vertex.OnPositionChanged += Vertex_OnPositionChanged;
                Vertices.Add(vertex);
                vertex.IsInPolygon = true;
                _mainPolygon = new Vector2[Vertices.Count];
                CalculatePolygon();
            }
        }

        public void InsertVertexAfter(EditorVertex pivotVertex, EditorVertex vertex)
        {
            var index = Vertices.IndexOf(pivotVertex);
            if (index > -1)
            {
                vertex.OnPositionChanged += Vertex_OnPositionChanged;
                Vertices.Insert(index+1,vertex);
                vertex.IsInPolygon = true;
                _mainPolygon = new Vector2[Vertices.Count];
                CalculatePolygon();
            }
            else
            {
                AddVertex(vertex);
            }
        }

        private void Vertex_OnPositionChanged(EditorVertex vertex, Vector2 position)
        {
            CalculatePolygon();
        }

        private void CalculatePolygon()
        {
            Vector2 mid = Vector2.zero;
            for (int i = 0; i < Vertices.Count; i++)
            {
                mid += Vertices[i].Position;
                _mainPolygon[i] = Vertices[i].Position;
            }

            _textPosition = mid / Vertices.Count + new Vector2(-12,-11);
        }

        public void RemoveVertex(EditorVertex vertex)
        {
            if (Vertices.Contains(vertex))
            {
                vertex.OnPositionChanged -= Vertex_OnPositionChanged;
                vertex.IsInPolygon = false;
                Vertices.Remove(vertex);
                _mainPolygon = new Vector2[Vertices.Count];
                CalculatePolygon();
            }

        }

        public void ClearVertices()
        {
            foreach (var vertex in Vertices)
            {
                vertex.OnPositionChanged-= Vertex_OnPositionChanged;
                vertex.IsInPolygon = false;
            }
            Vertices.Clear();
        }

        public bool ContainerPoint(Vector2 point)
        {
            return PolygonUtils.PointInPolygon(point, _mainPolygon);
        }

        public bool CheckMouseClick(Vector2 position, out EditorVertex hitVertex)
        {
            foreach (var vertex in Vertices)
            {
                if (vertex.CheckHit(position))
                {
                    hitVertex = vertex;
                    return true;
                }
            }

            hitVertex = null;
            return ContainerPoint(position);
        }

        public bool ContainerVertex(EditorVertex vertex)
        {
            return Vertices.Contains(vertex);
        }

        public override string ToString()
        {
            return $"{HashCode}: {Id} - {FlowerId}";
        }

        private Label _labelFlowerId;

        private Image _flowerImage;

        public VisualElement GetVisual()
        {
            VisualElement container = new VisualElement();

            Label labelId = new Label($"Id: {Id}");
            container.Add(labelId);

            Label labelVertexCount = new Label($"Vertices: {Vertices.Count}");
            container.Add(labelVertexCount);


            VisualElement flowerContainer = new VisualElement();
            flowerContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            flowerContainer.style.alignItems = new StyleEnum<Align>(Align.Center);
            container.Add(flowerContainer);

            _labelFlowerId = new Label($"Flower Id: {FlowerId}");
            flowerContainer.Add(_labelFlowerId);

            _flowerImage = new Image();
            _flowerImage.style.width = 50;
            _flowerImage.style.height = 50;
            SetFlowerId(FlowerId);
            flowerContainer.Add(_flowerImage);


            Button buttonSetFlowerId = new Button();
            buttonSetFlowerId.text = "Select Flower";
            buttonSetFlowerId.clicked += ButtonSetFlowerId_clicked;

            flowerContainer.Add(buttonSetFlowerId);

            return container;
        }

        private void ButtonSetFlowerId_clicked()
        {
            EditorEventManager.EntityEvents.OnSelectFlowerEvent(this,FlowerId);
        }

        public void SetFlowerId(int flowerId)
        {
            FlowerId = flowerId;

            if (_labelFlowerId != null)
            {
                if (FlowerId > 0)
                {
                    _labelFlowerId.text = $"Flower Id: {FlowerId}";
                }
                else
                {
                    _labelFlowerId.text = $"Flower Id: 0 - Random";
                }
            }

            if (_flowerImage != null)
            {
                if (FlowerId > 0)
                {
                    _flowerImage.sprite = GlobalResourceManager.GetFlowerData(FlowerId).sprite;
                }
            }

        }
    }
}

