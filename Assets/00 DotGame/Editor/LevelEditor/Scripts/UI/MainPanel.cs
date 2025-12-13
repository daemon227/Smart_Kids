using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class MainPanel : VisualElement
    {
        private Button _buttonPlay;

        public event Action OnButtonSaveLevel;

        public event Action OnButtonNewLevel;

        public event Action OnButtonOpenLevel;

        public event Action OnButtonPlayLevel;

        private IntegerField _intFieldMaxHp;

        private IntegerField _intFieldInsectLifeTime;

        private EditorLevelDataSave _levelDataSave;

        private Button _buttonAddStep;

        private Vector2Field _imagePositionField;

        private Vector2Field _imageSizeField;
        public MainPanel()
        {
            var asset = Resources.Load<VisualTreeAsset>("MainPanel");
            var child = asset.CloneTree();

            Add(child);

            var buttonNewLevel = child.Q<Button>("buttonNewLevel");
            buttonNewLevel.clicked += ButtonNewLevel_clicked;

            var buttonOpenLevel = child.Q<Button>("buttonOpenLevel");
            buttonOpenLevel.clicked += ButtonOpenLevel_clicked;

            var buttonSaveLevel = child.Q<Button>("buttonSaveLevel");
            buttonSaveLevel.clicked += ButtonSaveLevel_clicked;

            _buttonPlay = child.Q<Button>("buttonPlay");
            _buttonPlay.clicked += _buttonPlay_clicked;

            var integerFieldGridSize = child.Q<IntegerField>("intFieldGridSize");
            integerFieldGridSize.SetValueWithoutNotify(EditorConfig.GridSize);
            integerFieldGridSize.RegisterValueChangedCallback((evt =>
            {
                EditorEventManager.EditorEvents.OnGridSizeChangedEvent(evt.newValue);
            }));

            //var intFieldLevelSizeRatio = child.Q<IntegerField>("intFieldLevelSizeRatio");
            //intFieldLevelSizeRatio.SetValueWithoutNotify(EditorConfig.LevelSizeRatio);
            //intFieldLevelSizeRatio.RegisterValueChangedCallback((evt =>
            //{
            //    EditorConfig.LevelSizeRatio = evt.newValue;
            //}));

            _intFieldMaxHp = child.Q<IntegerField>("intFieldMaxHp");
            _intFieldMaxHp.RegisterValueChangedCallback((evt =>
            {
                _levelDataSave.MaxHp = evt.newValue;
            }));

            _intFieldInsectLifeTime = child.Q<IntegerField>("intFieldInsectLifeTime");
            _intFieldInsectLifeTime.RegisterValueChangedCallback((evt =>
            {
                _levelDataSave.InsectLifeTime = evt.newValue;
            }));

            _buttonAddStep = child.Q<Button>("buttonAddStep");
            _buttonAddStep.clicked += ButtonAddStep_clicked;

            _scrollViewSteps = child.Q<ScrollView>("scrollViewSteps");

            _imagePositionField = child.Q<Vector2Field>("vector2FieldImagePosition");
            _imagePositionField.RegisterValueChangedCallback((evt =>
            {
                EditorEventManager.EditorEvents.OnImagePreviewPositionChangedEvent(evt.newValue);
            }));

            _imageSizeField = child.Q<Vector2Field>("vector2FieldImageSize");
            _imageSizeField.RegisterValueChangedCallback((evt =>
            {
                EditorEventManager.EditorEvents.OnImagePreviewSizeChangedEvent(evt.newValue);
            }));

            var buttonSelectPreviewImage = child.Q<Button>("buttonSelectPreviewImage");
            buttonSelectPreviewImage.clicked += ButtonSelectPreviewImage_clicked;

            var sliderOpacity = child.Q<Slider>("sliderOpacity");
            sliderOpacity.value = 0.35f;
            sliderOpacity.RegisterValueChangedCallback((evt =>
            {
                EditorEventManager.EditorEvents.OnImagePreviewOpacityChangedEvent(evt.newValue);
            }));
            EventHandlers();
        }

        private void ButtonSelectPreviewImage_clicked()
        {
            EditorEventManager.EditorEvents.OnSelectImagePreviewEvent();
        }

        void EventHandlers()
        {
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

            EditorEventManager.EntityEvents.OnPolygonAddedToStep += EditorEventManager_OnPolygonAddedToStep;
            EditorEventManager.EntityEvents.OnPolygonRemovedToStep += EditorEventManager_OnPolygonRemovedToStep;

            EditorEventManager.EntityEvents.OnStepSelected += EntityEvents_OnStepSelected;

            EditorEventManager.EditorEvents.OnToolTypeChanged += EditorEventManager_OnToolTypeChanged;
            EditorEventManager.EditorEvents.OnImagePreviewSelected += EditorEvents_OnImagePreviewSelected; ;
        }

        private void EditorEvents_OnImagePreviewSelected(Texture texture)
        {
            _imageSizeField.value = new Vector2(texture.width, texture.height);
        }

        private void EntityEvents_OnStepSelected(EditorStepData step)
        {
            if (step != null)
            {
                foreach (var child in _scrollViewSteps.Children())
                {
                    var childItem = child as EditorStepItem;
                    if (childItem.StepData == step)
                    {
                        StepItem_OnSelected(childItem);
                        _scrollViewSteps.ScrollTo(childItem);
                    }
                }
            }
            else
            {
                foreach (var child in _scrollViewSteps.Children())
                {
                    var childItem = child as EditorStepItem;
                    childItem.Normal();
                }
            }
        }

        private void EditorEventManager_OnToolTypeChanged(EditorToolType newTool, EditorToolType oldTool)
        {
            if (newTool == EditorToolType.Step)
            {
                _buttonAddStep.SetEnabled(true);
                _scrollViewSteps.SetEnabled(true);
            }
            else
            {
                _buttonAddStep.SetEnabled(false);
                _scrollViewSteps.SetEnabled(false);
                if (_selectedStepItem != null)
                {
                    _selectedStepItem.Normal();
                    _selectedStepItem = null;
                }
            }
        }

        private void EditorEventManager_OnPolygonRemovedToStep(EditorStepData arg1, EditorPolygon arg2)
        {
            if (_selectedStepItem != null)
            {
                _selectedStepItem.UpdateText();
            }
        }

        private void EditorEventManager_OnPolygonAddedToStep(EditorStepData arg1, EditorPolygon arg2)
        {
            if (_selectedStepItem != null)
            {
                _selectedStepItem.UpdateText();
            }
        }
        
        private ScrollView _scrollViewSteps;
        private void ButtonAddStep_clicked()
        {
            var stepItem = CreateNewStepItem(null);
            _scrollViewSteps.Add(stepItem);
            ResetStepIds();

            EditorEventManager.EntityEvents.OnStepCreatedEvent(stepItem.StepData);
            SelectStepItem(stepItem);
            EditorEventManager.EntityEvents.OnStepItemSelectedEvent(stepItem);
        }

        private EditorStepItem CreateNewStepItem(EditorStepData stepData)
        {
            if (stepData==null)
            {
                stepData = new EditorStepData();
            }
            EditorStepItem stepItem = new EditorStepItem(stepData);
            stepItem.OnSelected += StepItem_OnSelected;
            stepItem.OnRemove += StepItem_OnRemove;
            return stepItem;
        }

        private EditorStepItem _selectedStepItem = null;
        private void StepItem_OnRemove(EditorStepItem item)
        {
            SelectStepItem(null);
            _scrollViewSteps.Remove(item);
            ResetStepIds();
            EditorEventManager.EntityEvents.OnStepItemRemovedEvent(item);
        }

        private void ResetStepIds()
        {
            int id = 0;
            foreach (var child in _scrollViewSteps.Children())
            {
                var childItem = child as EditorStepItem;
                childItem.SetId(id);
                id++;
            }
        }

        private void StepItem_OnSelected(EditorStepItem item)
        {
            SelectStepItem(item);
            EditorEventManager.EntityEvents.OnStepItemSelectedEvent(item);
        }

        private void SelectStepItem(EditorStepItem item)
        {
            if (_selectedStepItem != null)
            {
                _selectedStepItem.Normal();
            }

            if (item != null)
            {
                _selectedStepItem = item;
                _selectedStepItem.Selected();
            }

        }


        private void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                _buttonPlay.text = "Stop";
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                _buttonPlay.text = "Play";
            }
        }

        private void ButtonNewLevel_clicked()
        {
            OnButtonNewLevel?.Invoke();
        }

        private void ButtonOpenLevel_clicked()
        {
            OnButtonOpenLevel?.Invoke();
        }

        private void ButtonSaveLevel_clicked()
        {
            OnButtonSaveLevel?.Invoke();
        }

        private void _buttonPlay_clicked()
        {
            OnButtonPlayLevel?.Invoke();
        }

        public void SetEditorLevelData(EditorLevelDataSave levelData)
        {
            _levelDataSave = levelData;

            _intFieldMaxHp.SetValueWithoutNotify(_levelDataSave.MaxHp);
            _intFieldInsectLifeTime.SetValueWithoutNotify(_levelDataSave.InsectLifeTime);

            _scrollViewSteps.Clear();
            foreach (var editorStepData in _levelDataSave.Steps)
            {
                var stepItem = CreateNewStepItem(editorStepData);
                _scrollViewSteps.Add(stepItem);
            }
        }
    }
}

