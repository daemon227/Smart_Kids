using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class ToolPanel : VisualElement
    {
        private ScrollView _scrollView;

        private Dictionary<EditorToolType, EditorToolItem> _dic = new();

        public event Action<EditorToolType, EditorToolType> OnToolTypeChanged;

        private EditorToolItem _currentItem;
        public ToolPanel()
        {
            var asset = Resources.Load<VisualTreeAsset>("ToolPanel");
            var child = asset.CloneTree();

            Add(child);

            _scrollView = child.Q<ScrollView>("scrollViewItems");
            foreach (var toolType in Enum.GetValues(typeof(EditorToolType)))
            {
                EditorToolItem item = new EditorToolItem((EditorToolType)toolType);
                item.OnClicked += Item_OnClicked;
                _scrollView.Add(item);
                _dic.Add(item.ToolType, item);
            }

            SelectTool(EditorToolType.Vertex);
            EventHandlers();
        }
        private void Item_OnClicked(EditorToolItem item)
        {
            SelectTool(item.ToolType);
        }

        public void SelectTool(EditorToolType toolType)
        {
            if (_dic.ContainsKey(toolType))
            {
                var lastToolType = EditorToolType.Vertex;
                if (_currentItem != null)
                {
                    lastToolType = _currentItem.ToolType;
                }

                _currentItem?.Normal();
                _currentItem = _dic[toolType];
                _currentItem.Selected();

                OnToolTypeChanged?.Invoke(toolType, lastToolType);
            }
        }

        void EventHandlers()
        {

        }
    }
}

