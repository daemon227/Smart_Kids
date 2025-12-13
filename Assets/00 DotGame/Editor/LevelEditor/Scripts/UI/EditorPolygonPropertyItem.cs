using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class EditorPolygonPropertyItem : VisualElement
    {
        private static VisualTreeAsset _treeAsset;

        private Label _labelId;

        private Color _normalColor;

        private Color _selectedColor;

        public EditorPolygon Polygon { get; }

        public event Action<EditorPolygonPropertyItem> OnRemove;

        public event Action<EditorPolygonPropertyItem> OnSelected;

        public EditorPolygonPropertyItem(EditorPolygon polygon)
        {
            Polygon = polygon;

            if (_treeAsset == null)
            {
                _treeAsset = Resources.Load<VisualTreeAsset>("EditorPolygonPropertyItem");
            }

            var child = _treeAsset.CloneTree();
            Add(child);

            _labelId = child.Q<Label>("labelId");
            _labelId.text = $"Id: {Polygon.Id}";
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
            if (Polygon.IsInStep)
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

