using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class EditorVertexPropertyItem : VisualElement
    {
        private static VisualTreeAsset _treeAsset;

        private Label _labelId;

        private Color _normalColor;

        private Color _selectedColor;

        public EditorVertex Vertex { get; }

        public event Action<EditorVertexPropertyItem> OnRemove;

        public event Action<EditorVertexPropertyItem> OnSelected;

        public EditorVertexPropertyItem(EditorVertex vertex)
        {
            Vertex = vertex;

            if (_treeAsset == null)
            {
                _treeAsset = Resources.Load<VisualTreeAsset>("EditorVertexPropertyItem");
            }

            var child = _treeAsset.CloneTree();
            Add(child);

            _labelId = child.Q<Label>("labelId");
            _labelId.text = $"Id: {vertex.Id}";
            UpdateText();

            var button = child.Q<Button>("buttonRemove");
            button.clicked += Button_clicked;

            this.RegisterCallback<MouseDownEvent>(OnMouseDown);

            _normalColor = child.style.backgroundColor.value;
            _selectedColor = new Color(0, 0.6f, 1);
        }

        public void Normal()
        {
            style.backgroundColor = _normalColor;
        }

        public void Selected()
        {
            style.backgroundColor = _selectedColor;
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            OnSelected?.Invoke(this);
        }

        private void Button_clicked()
        {
            OnRemove?.Invoke(this);
        }

        public void UpdateText()
        {
            if (Vertex.IsInPolygon)
            {
                _labelId.style.color = new StyleColor(Color.white);
            }
            else
            {
                _labelId.style.color = new StyleColor(Color.red);
            }
        }
    }
}

