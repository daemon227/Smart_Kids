using System;
using DotPuzzle.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DotPuzzle.Editor
{
    public partial class EditorEventManager
    {
        public class LevelEvents
        {
            public static event Action OnNewLevel;

            public static event Action<string, LevelDataSave> OnOpenLevel;

            public static event Action<string, EditorLevelDataSave> OnSaveLevel;

            public static void OnNewLevelEvent()
            {
                OnNewLevel?.Invoke();
            }

            public static void OnOpenLevelEvent(string path, LevelDataSave levelDataSave)
            {
                OnOpenLevel?.Invoke(path, levelDataSave);
            }

            public static void OnSaveLevelEvent(string path, EditorLevelDataSave levelDataSave)
            {
                OnSaveLevel?.Invoke(path, levelDataSave);
            }

            public static void ClearEventHandlers()
            {
                if (OnNewLevel != null)
                {
                    foreach (var handler in OnNewLevel.GetInvocationList())
                    {
                        OnNewLevel -= (Action)handler;
                    }
                }

                if (OnOpenLevel != null)
                {
                    foreach (var handler in OnOpenLevel.GetInvocationList())
                    {
                        OnOpenLevel -= (Action<string, LevelDataSave>)handler;
                    }
                }

                if (OnSaveLevel != null)
                {
                    foreach (var handler in OnSaveLevel.GetInvocationList())
                    {
                        OnSaveLevel -= (Action<string, EditorLevelDataSave>)handler;
                    }
                }
            }
        }

    }
}

