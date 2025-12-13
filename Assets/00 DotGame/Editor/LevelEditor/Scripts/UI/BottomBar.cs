using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class BottomBar : VisualElement
    {
        private Label _labelInfo;

        public BottomBar()
        {
            var asset = Resources.Load<VisualTreeAsset>("BottomBar");
            var child = asset.CloneTree();

            Add(child);

            _labelInfo = child.Q<Label>("labelInfo");
            EventHandlers();
        }

        void EventHandlers()
        {
            EditorEventManager.LevelEvents.OnNewLevel += EditorEventManager_OnNewLevel;
            EditorEventManager.LevelEvents.OnSaveLevel += EditorEventManager_OnSaveLevel;
            EditorEventManager.LevelEvents.OnOpenLevel += EditorEventManager_OnOpenLevel;
        }

        private void EditorEventManager_OnOpenLevel(string path, LevelDataSave arg2)
        {
            _labelInfo.text = $"Level : {path}";
        }

        private void EditorEventManager_OnNewLevel()
        {
            _labelInfo.text = "Level : New";
        }

        private void EditorEventManager_OnSaveLevel(string path, EditorLevelDataSave arg2)
        {
            _labelInfo.text = $"Level : {path}";
        }
    }
}

