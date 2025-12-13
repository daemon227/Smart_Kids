using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class EditorToolItem : VisualElement
    {
        private Label _labelName;

        private static VisualTreeAsset _treeAsset;

        public event Action<EditorToolItem> OnClicked;

        private Color _normalColor;

        private Color _selectedColor;

        public EditorToolType ToolType { get; private set; }

        public EditorToolItem(EditorToolType toolType)
        {
            if (_treeAsset == null)
            {
                _treeAsset = Resources.Load<VisualTreeAsset>("EditorToolItem");
            }

            var child = _treeAsset.CloneTree();
            Add(child);

            _labelName = child.Q<Label>("labelName");
            SetToolType(toolType);

            RegisterCallback<MouseDownEvent>(OnMouseDown);

            _normalColor = child.style.backgroundColor.value;
            _selectedColor = new Color(0, 0.6f, 1);
        }

        public void SetToolType(EditorToolType toolType)
        {
            ToolType = toolType;
            string name = String.Empty;
            switch (toolType)
            {
                case EditorToolType.Vertex:
                    name = "Vertex";
                    break;
                case EditorToolType.Polygon:
                    name = "Polygon";
                    break;
                case EditorToolType.Step:
                    name = "Step";
                    break;
                //case EditorToolType.Flower:
                //    name = "Flower";
                //    break;
                default:
                    break;
            }

            _labelName.text = name;
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            OnClicked?.Invoke(this);
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

