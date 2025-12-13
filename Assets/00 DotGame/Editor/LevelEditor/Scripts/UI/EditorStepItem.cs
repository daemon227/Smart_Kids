using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class EditorStepItem:VisualElement
    {
        private static VisualTreeAsset _treeAsset;

        private Label _labelId;

        private Color _normalColor;

        private Color _selectedColor;



        public event Action<EditorStepItem> OnRemove;

        public event Action<EditorStepItem> OnSelected;

        public EditorStepData StepData { get; private set; }
        public EditorStepItem(EditorStepData stepData)
        {
            StepData = stepData;
            StepData.OnPolygonAdded += StepData_OnPolygonAdded;
            StepData.OnPolygonRemoved += StepData_OnPolygonRemoved;
            if (_treeAsset == null)
            {
                _treeAsset = Resources.Load<VisualTreeAsset>("EditorStepItem");
            }

            var child = _treeAsset.CloneTree();
            Add(child);

            _labelId = child.Q<Label>("labelId");
            UpdateText();

            var button = child.Q<Button>("buttonRemove");
            button.clicked += Button_clicked;

            this.RegisterCallback<MouseDownEvent>(OnMouseDown);

            _normalColor = child.style.backgroundColor.value;
            _selectedColor = new Color(0, 0.6f, 1);
        }

        private void StepData_OnPolygonRemoved(EditorPolygon polygon)
        {
            UpdateText();
        }

        private void StepData_OnPolygonAdded(EditorPolygon polygon)
        {
            UpdateText();
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

        public void SetId(int id)
        {
            StepData.Id = id;
            UpdateText();
        }

        public void UpdateText()
        {
            _labelId.text = $"Step: {StepData.Id}\nPolygons: {StepData.Polygons.Count}";
            if (StepData.Polygons.Count > 0)
            {
                _labelId.style.color = Color.white;
            }
            else
            {
                _labelId.style.color = Color.red;
            }
        }
    }
}

