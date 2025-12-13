using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DotPuzzle.Editor
{
    public partial class EditorEventManager
    {
        public class EditorEvents
        {
            public static event Action<int> OnGridSizeChanged;

            public static event Action<EditorToolType, EditorToolType> OnToolTypeChanged;

            public static event Action OnNeedUpdateCanvas;

            public static event Action OnSelectImagePreview;

            public static event Action<Texture> OnImagePreviewSelected;

            public static event Action<Vector2> OnImagePreviewPositionChanged;

            public static event Action<Vector2> OnImagePreviewSizeChanged;

            public static event Action<float> OnImagePreviewOpacityChanged; 

            public static void OnGridSizeChangedEvent(int value)
            {
                OnGridSizeChanged?.Invoke(value);
            }

            public static void OnToolTypeChangedEvent(EditorToolType newTool, EditorToolType oldTool)
            {
                OnToolTypeChanged?.Invoke(newTool, oldTool);
            }

            public static void OnNeedUpdateCanvasEvent()
            {
                OnNeedUpdateCanvas?.Invoke();
            }

            public static void OnSelectImagePreviewEvent()
            {
                OnSelectImagePreview?.Invoke();
            }

            public static void OnImagePreviewSelectedEvent(Texture texture)
            {
                OnImagePreviewSelected?.Invoke(texture);
            }

            public static void OnImagePreviewPositionChangedEvent(Vector2 size)
            {
                OnImagePreviewPositionChanged?.Invoke(size);
            }

            public static void OnImagePreviewSizeChangedEvent(Vector2 size)
            {
                OnImagePreviewSizeChanged?.Invoke(size);
            }

            public static void OnImagePreviewOpacityChangedEvent(float opacity)
            {
                OnImagePreviewOpacityChanged?.Invoke(opacity);
            }
            public static void ClearEventHandlers()
            {

                if (OnGridSizeChanged != null)
                {
                    foreach (var handler in OnGridSizeChanged.GetInvocationList())
                    {
                        OnGridSizeChanged -= (Action<int>)handler;
                    }
                }

                if (OnToolTypeChanged != null)
                {
                    foreach (var handler in OnToolTypeChanged.GetInvocationList())
                    {
                        OnToolTypeChanged -= (Action<EditorToolType, EditorToolType>)handler;
                    }
                }

                if (OnNeedUpdateCanvas != null)
                {
                    foreach (var handler in OnNeedUpdateCanvas.GetInvocationList())
                    {
                        OnNeedUpdateCanvas -= (Action)handler;
                    }
                }

                if (OnSelectImagePreview != null)
                {
                    foreach (var handler in OnSelectImagePreview.GetInvocationList())
                    {
                        OnSelectImagePreview -= (Action)handler;
                    }
                }

                if (OnImagePreviewSelected != null)
                {
                    foreach (var handler in OnImagePreviewSelected.GetInvocationList())
                    {
                        OnImagePreviewSelected -= (Action<Texture>)handler;
                    }
                }

                if (OnImagePreviewPositionChanged != null)
                {
                    foreach (var handler in OnImagePreviewPositionChanged.GetInvocationList())
                    {
                        OnImagePreviewPositionChanged -= (Action<Vector2>)handler;
                    }
                }

                if (OnImagePreviewSizeChanged != null)
                {
                    foreach (var handler in OnImagePreviewSizeChanged.GetInvocationList())
                    {
                        OnImagePreviewSizeChanged -= (Action<Vector2>)handler;
                    }
                }

                if (OnImagePreviewOpacityChanged != null)
                {
                    foreach (var handler in OnImagePreviewOpacityChanged.GetInvocationList())
                    {
                        OnImagePreviewOpacityChanged -= (Action<float>)handler;
                    }
                }
            }
        }
    }


}
