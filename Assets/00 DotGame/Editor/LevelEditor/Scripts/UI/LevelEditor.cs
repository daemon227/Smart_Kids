using System;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class LevelEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("DotPuzzle/LevelEditor")]
        public static void Show()
        {
            LevelEditor wnd = GetWindow<LevelEditor>();
            wnd.titleContent = new GUIContent("LevelEditor");
        }


        MainPanel _mainPanel;
        MainCanvas _mainCanvas;
        ToolPanel _toolPanel;
        BottomBar _bottomBar;
        PropertyPanel _propertyPanel;
        public void CreateGUI()
        {
            EditorEventManager.ClearAllEventHandlers();
            GlobalResourceManager.LoadFlowerSO();


            VisualElement root = rootVisualElement;

            TwoPaneSplitView _paneSplitView1 = new TwoPaneSplitView(0,
                300,
                TwoPaneSplitViewOrientation.Horizontal);

            root.Add(_paneSplitView1);

            _bottomBar = new BottomBar();
            root.Add(_bottomBar);

            _mainPanel = new MainPanel();
            _mainPanel.OnButtonNewLevel += _mainPanel_OnButtonNewLevel;
            _mainPanel.OnButtonOpenLevel += _mainPanel_OnButtonOpenLevel;
            _mainPanel.OnButtonSaveLevel += _mainPanel_OnButtonSaveLevel;
            _mainPanel.OnButtonPlayLevel += _mainPanel_OnButtonPlayLevel;
            _paneSplitView1.Add(_mainPanel);

            VisualElement mainContainer = new VisualElement();
            _paneSplitView1.Add(mainContainer);

            _toolPanel = new ToolPanel();
            _toolPanel.OnToolTypeChanged += _toolPanel_OnToolTypeChanged;
            mainContainer.Add(_toolPanel);

            TwoPaneSplitView paneSplitView2 = new TwoPaneSplitView(1,
                250, TwoPaneSplitViewOrientation.Horizontal);
            mainContainer.Add(paneSplitView2);

            ScrollView scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            paneSplitView2.Add(scrollView);

            _mainCanvas = new MainCanvas();
            scrollView.Add(_mainCanvas);

            _propertyPanel = new PropertyPanel();
            paneSplitView2.Add(_propertyPanel);

            EventHandlers();

            CreateNewLevel();
        }

        private void EventHandlers()
        {
            EditorEventManager.EntityEvents.OnVertexRemoved += EditorEventManager_OnVertexRemoved;
            EditorEventManager.EntityEvents.OnVertexSelected += EditorEventManager_OnVertexSelected;

            EditorEventManager.EntityEvents.OnPolygonSelected += EditorEventManager_OnPolygonSelected;
            EditorEventManager.EntityEvents.OnPolygonRemoved += EditorEventManager_OnPolygonRemoved;

            EditorEventManager.EntityEvents.OnStepItemSelected += EditorEventManager_OnStepItemSelected;
            EditorEventManager.EntityEvents.OnStepItemRemoved += EditorEventManager_OnStepItemRemoved;
            EditorEventManager.EntityEvents.OnStepSelected += EntityEvents_OnStepSelected;

            EditorEventManager.EntityEvents.OnPolygonAddedToStep += EditorEventManager_OnPolygonAddedToStep;
            EditorEventManager.EntityEvents.OnPolygonRemovedToStep += EditorEventManager_OnPolygonRemovedToStep;

            EditorEventManager.EntityEvents.OnSelectFlower += EntityEvents_OnSelectFlower;

            EditorEventManager.EditorEvents.OnSelectImagePreview += EditorEvents_OnSelectImagePreview;

            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

        }

        private void EntityEvents_OnStepSelected(EditorStepData step)
        {
            if (_mainCanvas.ToolType == EditorToolType.Step)
            {
                if (step != null)
                {
                    _propertyPanel.SetContent(step.GetVisual());
                }
                else
                {
                    _propertyPanel.SetContent(null);
                }
                
            }
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (!string.IsNullOrEmpty(_texturePath))
                {
                    LoadPreviewImage();
                }
            }
        }

        private void _toolPanel_OnToolTypeChanged(EditorToolType newToolType, EditorToolType oldToolType)
        {
            _mainCanvas.SetToolType(newToolType);

            if (newToolType == EditorToolType.Vertex)
            {
                _propertyPanel.SetLevelDataContent(_mainCanvas.EditorLevelData.GetVerticesVisual());
            }
            else if(newToolType == EditorToolType.Polygon)
            {
                _propertyPanel.SetLevelDataContent(_mainCanvas.EditorLevelData.GetPolygonsVisual());
            }
            else
            {
                _propertyPanel.SetLevelDataContent(null);
            }
            

            EditorEventManager.EditorEvents.OnToolTypeChangedEvent(newToolType, oldToolType);
        }

        private string _texturePath;
        private void EditorEvents_OnSelectImagePreview()
        {
            string filePath = EditorUtility.OpenFilePanel("Open Image", string.Empty, "png,jpg");
            if (!string.IsNullOrEmpty(filePath))
            {
                _texturePath = filePath;
                LoadPreviewImage();
            }
        }

        void LoadPreviewImage()
        {
            if (!File.Exists(_texturePath))
            {
                Debug.LogError("Find not found: " + _texturePath);
            }

            byte[] fileData = File.ReadAllBytes(_texturePath);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            EditorEventManager.EditorEvents.OnImagePreviewSelectedEvent(tex);
        }
        private FlowerSelectionPopup _flowerSelectionPopup;
        private void EntityEvents_OnSelectFlower(EditorPolygon polygon, int flowerId)
        {
            if (_flowerSelectionPopup == null)
            {
                _flowerSelectionPopup = ScriptableObject.CreateInstance<FlowerSelectionPopup>();
            }

            _flowerSelectionPopup.Show(polygon);
        }

        private void _mainPanel_OnButtonPlayLevel()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
            else
            {
                var levelText = _mainCanvas.GetLevelText(out var haveOrphans);
                if (!haveOrphans)
                {
                    PlayerPrefs.SetInt("TEST", 1);
                    PlayerPrefs.SetString("TEST_LEVEL", levelText);

                    PlayerPrefs.Save();

                    EditorApplication.isPlaying = true;
                }
                else
                {
                    EditorUtility.DisplayDialog("Level", "Invalid level!", "OK");
                }
            }

        }

        private void EditorEventManager_OnPolygonRemovedToStep(EditorStepData stepData, EditorPolygon polygon)
        {
            _propertyPanel.SetContent(stepData.GetVisual());
        }

        private void EditorEventManager_OnPolygonAddedToStep(EditorStepData stepData, EditorPolygon polygon)
        {
            _propertyPanel.SetContent(stepData.GetVisual());
        }

        private void _mainPanel_OnButtonSaveLevel()
        {
            SaveLevel();
        }

        private LevelDataSave _levelData;
        void SaveLevel()
        {
            var path = EditorUtility.SaveFilePanel("Save Level", String.Empty, "Level","json");
            if (!string.IsNullOrEmpty(path))
            {
                var text = _mainCanvas.GetLevelText(out var haveOrphans);
                if (!haveOrphans)
                {
                    File.WriteAllText(path, text);

                    EditorEventManager.LevelEvents.OnSaveLevelEvent(path, _mainCanvas.EditorLevelData);

                    EditorUtility.DisplayDialog("Save Level", $"Level saved: {path}", "OK");
                    AssetDatabase.Refresh();
                }
                else
                {
                    EditorUtility.DisplayDialog("Level", "Invalid level!", "OK");
                }

            }
            else
            {
                EditorUtility.DisplayDialog("Save Level", $"Cannot save level: {path}", "OK");
            }
        }


        private void _mainPanel_OnButtonOpenLevel()
        {
            var path = EditorUtility.OpenFilePanel("Open Level", String.Empty, "Json");
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Open Level", $"Invalid level path", "OK");
                return;
            }

            var text = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(text))
            {
                _levelData = JsonConvert.DeserializeObject<LevelDataSave>(text);
                if (_levelData != null)
                {
                    _mainCanvas.SetLevelDataSave(_levelData);

                    _mainPanel.SetEditorLevelData(_mainCanvas.EditorLevelData);

                    EditorEventManager.LevelEvents.OnOpenLevelEvent(path, _levelData);
                    _toolPanel.SelectTool(EditorToolType.Vertex);
                }
                else
                {
                    EditorUtility.DisplayDialog("Open Level", $"Invalid level: {path}", "OK");
                }
            }
        }

        private void _mainPanel_OnButtonNewLevel()
        {
            CreateNewLevel();
            EditorEventManager.LevelEvents.OnNewLevelEvent();
        }

        private void CreateNewLevel()
        {
            _mainCanvas.CreateNewLevel();
            _mainPanel.SetEditorLevelData(_mainCanvas.EditorLevelData);
            _toolPanel.SelectTool(EditorToolType.Vertex);
        }

        private void EditorEventManager_OnStepItemRemoved(EditorStepItem stepItem)
        {
            if (_mainCanvas.ToolType == EditorToolType.Step)
            {
                _propertyPanel.SetContent(null);
            }
        }

        private void EditorEventManager_OnStepItemSelected(EditorStepItem stepItem)
        {
            if (_mainCanvas.ToolType == EditorToolType.Step)
            {
                _propertyPanel.SetContent(stepItem.StepData.GetVisual());
            }
        }

        private void EditorEventManager_OnPolygonRemoved(EditorPolygon polygon)
        {
            if (_mainCanvas.ToolType == EditorToolType.Polygon)
            {
                _propertyPanel.SetContent(null);
            }
        }

        private void EditorEventManager_OnPolygonSelected(EditorPolygon polygon)
        {
            if (_mainCanvas.ToolType == EditorToolType.Polygon)
            {
                if (polygon != null)
                {
                    _propertyPanel.SetContent(polygon.GetVisual());
                }
                else
                {
                    _propertyPanel.SetContent(null);
                }
            }
        }

        private void EditorEventManager_OnVertexSelected(EditorVertex vertex)
        {
            if (_mainCanvas.ToolType == EditorToolType.Vertex)
            {
                if (vertex != null)
                {
                    _propertyPanel.SetContent(vertex.GetVisual());
                }
                else
                {
                    _propertyPanel.SetContent(null);
                }

            }

        }

        private void EditorEventManager_OnVertexRemoved(EditorVertex vertex)
        {
            _propertyPanel.SetContent(null);
        }
    }
}

