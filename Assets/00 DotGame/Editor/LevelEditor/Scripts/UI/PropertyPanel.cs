using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class PropertyPanel : VisualElement
    {
        private VisualElement _container;

        private VisualElement _levelContainer;

        public PropertyPanel()
        {
            var asset = Resources.Load<VisualTreeAsset>("PropertyPanel");
            var child = asset.CloneTree();

            Add(child);

            _container = child.Q<VisualElement>("container");

            _levelContainer = child.Q<VisualElement>("levelPropertyContainer");
        }

        public void SetContent(VisualElement content)
        {
            _container.Clear();
            if (content != null)
            {
                _container.Add(content);
            }
        }

        public void SetLevelDataContent(VisualElement content)
        {
            _levelContainer?.Clear();
            if (content != null)
            {
                _levelContainer?.Add(content);
            }
        }
    }
}

