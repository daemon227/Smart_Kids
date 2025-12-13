using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class FlowerSelectionItem : VisualElement
    {
        private static VisualTreeAsset _treeAsset;

        private Label _labelId;

        private VisualElement _icon;

        private Color _normalColor;

        private Color _selectedColor;

        public event Action<FlowerSelectionItem> OnSelected;

        public FlowerData Data { get; private set; }

        public FlowerSelectionItem(FlowerData data)
        {
            Data = data;
            if (_treeAsset == null)
            {
                _treeAsset = Resources.Load<VisualTreeAsset>("FlowerSelectionItem");
            }

            var child = _treeAsset.CloneTree();
            Add(child);

            _labelId = child.Q<Label>("labelId");
            _labelId.text = $"Id: {Data.id}";

            _icon = child.Q<VisualElement>("icon");
            _icon.style.backgroundImage = new StyleBackground(Data.sprite);

            this.RegisterCallback<MouseDownEvent>(OnMouseDown);

            _normalColor = child.style.backgroundColor.value;
            _selectedColor = new Color(0, 0.6f, 1);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            OnSelected?.Invoke(this);
        }

        public void Normal()
        {
            style.backgroundColor = _normalColor;
        }

        public void Selected()
        {
            style.backgroundColor = _selectedColor;
        }
    }
}

