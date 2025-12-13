using DG.DemiEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class FlowerSelectionPopup:EditorWindow
    {
        private Dictionary<int, FlowerSelectionItem> _items;

        private EditorPolygon _editorPolygon;

        private FlowerSelectionItem _selectedItem;

        private ScrollView scrollView;
        private void CreateGUI()
        {
            scrollView = new ScrollView(ScrollViewMode.Vertical);
            rootVisualElement.Add(scrollView);

            //int count = GlobalResourceManager.GetFlowerDataCount();

            _items = new();

            FlowerData randomFlowerData = new FlowerData
            {
                id = 0,
                sprite = null
            };

            CreateNewItem(randomFlowerData);

            foreach (var flowerData in GlobalResourceManager.Flowers)
            {
                CreateNewItem(flowerData);
            }

            if (_items.ContainsKey(_editorPolygon.FlowerId))
            {
                _selectedItem = _items[_editorPolygon.FlowerId];
                _selectedItem.Selected();
            }

        }

        void CreateNewItem(FlowerData flowerData)
        {
            var item = new FlowerSelectionItem(flowerData);
            item.OnSelected += Item_OnSelected;
            _items.Add(flowerData.id, item);
            scrollView.Add(item);
        }

        private void Item_OnSelected(FlowerSelectionItem item)
        {
            if (_selectedItem != item)
            {
                if (_selectedItem != null)
                {
                    _selectedItem.Normal();
                }
                _selectedItem = item;
                _selectedItem.Selected();

                //_editorPolygon.SetFlowerId(_selectedItem.Data.id);
                EditorEventManager.EntityEvents.OnFlowerIdSelectedEvent(_editorPolygon, _selectedItem.Data);
            }
        }

        public void Show(EditorPolygon polygon)
        {
            _editorPolygon = polygon;
            titleContent = new GUIContent("Select Flower");
            ShowModal();
        }
    }
}

