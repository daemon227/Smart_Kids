using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class EditorPolygonItem : VisualElement
    {
        private static VisualTreeAsset _treeAsset;

        public EditorPolygon Polygon { get; private set; }

        private Label _labelId;

        private Color _normalColor;

        private Color _selectedColor;

        public event Action<EditorPolygonItem> OnRemoved;

        public event Action<EditorPolygonItem> OnSelected;

        public EditorPolygonItem(EditorPolygon polygon)
        {
            Polygon = polygon;
            if (_treeAsset == null)
            {
                _treeAsset = Resources.Load<VisualTreeAsset>("EditorPolygonItem");
            }

            var child = _treeAsset.CloneTree();
            Add(child);

            _labelId = child.Q<Label>("labelId");
            _labelId.text = $"Polygon Id: {Polygon.Id}\nFlower Id: {Polygon.FlowerId}";

            var buttonRemove = child.Q<Button>("buttonRemove");
            buttonRemove.clicked += ButtonRemove_clicked;


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

        private void ButtonRemove_clicked()
        {
            OnRemoved?.Invoke(this);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            OnSelected?.Invoke(this);
        }


    }
}

