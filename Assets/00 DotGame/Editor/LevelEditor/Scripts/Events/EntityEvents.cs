using DotPuzzle.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DotPuzzle.Editor
{
    public partial class EditorEventManager
    {
        public class EntityEvents
        {

            public static event Action<EditorVertex> OnVertexSelected;

            public static event Action<EditorVertex> OnVertexRemoved;

            public static event Action<EditorPolygon> OnPolygonSelected;

            public static event Action<EditorPolygon> OnPolygonRemoved;

            public static event Action<EditorStepData> OnStepCreated;

            public static event Action<EditorStepItem> OnStepItemSelected;

            public static event Action<EditorStepItem> OnStepItemRemoved;

            public static event Action<EditorStepData, EditorPolygon> OnPolygonAddedToStep;

            public static event Action<EditorStepData, EditorPolygon> OnPolygonRemovedToStep;

            public static event Action<EditorPolygonItem> OnPolygonItemSelected; 

            public static event Action<EditorStepData> OnStepSelected;

            public static event Action<EditorPolygon,int> OnSelectFlower;

            public static event Action<EditorPolygon,FlowerData> OnFlowerIdSelected;

            public static void OnVertexSelectedEvent(EditorVertex vertex)
            {
                OnVertexSelected?.Invoke(vertex);
            }

            public static void OnVertexRemovedEvent(EditorVertex vertex)
            {
                OnVertexRemoved?.Invoke(vertex);
            }

            public static void OnPolygonSelectedEvent(EditorPolygon polygon)
            {
                OnPolygonSelected?.Invoke(polygon);
            }

            public static void OnPolygonRemovedEvent(EditorPolygon polygon)
            {
                OnPolygonRemoved?.Invoke(polygon);
            }

            public static void OnStepCreatedEvent(EditorStepData stepData)
            {
                OnStepCreated?.Invoke(stepData);
            }

            public static void OnStepItemSelectedEvent(EditorStepItem selectedStepItem)
            {
                OnStepItemSelected?.Invoke(selectedStepItem);
            }

            public static void OnStepItemRemovedEvent(EditorStepItem selectedStepItem)
            {
                OnStepItemRemoved?.Invoke(selectedStepItem);
            }

            public static void OnPolygonAddedToStepEvent(EditorStepData step, EditorPolygon polygon)
            {
                OnPolygonAddedToStep?.Invoke(step, polygon);
            }

            public static void OnPolygonRemovedToStepEvent(EditorStepData step, EditorPolygon polygon)
            {
                OnPolygonRemovedToStep?.Invoke(step, polygon);
            }

            public static void OnPolygonItemSelectedEvent(EditorPolygonItem polygonItem)
            {
                OnPolygonItemSelected?.Invoke(polygonItem);
            }

            public static void OnStepSelectedEvent(EditorStepData step)
            {
                OnStepSelected?.Invoke(step);
            }

            public static void OnSelectFlowerEvent(EditorPolygon polygon, int currentFlowerId)
            {
                OnSelectFlower?.Invoke(polygon, currentFlowerId);
            }

            public static void OnFlowerIdSelectedEvent(EditorPolygon polygon, FlowerData flowerData)
            {
                OnFlowerIdSelected?.Invoke(polygon,flowerData);
            }
            public static void ClearEventHandlers()
            {
                if (OnVertexSelected != null)
                {
                    foreach (var handler in OnVertexSelected.GetInvocationList())
                    {
                        OnVertexSelected -= (Action<EditorVertex>)handler;
                    }
                }

                if (OnVertexRemoved != null)
                {
                    foreach (var handler in OnVertexRemoved.GetInvocationList())
                    {
                        OnVertexRemoved -= (Action<EditorVertex>)handler;
                    }
                }

                if (OnPolygonSelected != null)
                {
                    foreach (var handler in OnPolygonSelected.GetInvocationList())
                    {
                        OnPolygonSelected -= (Action<EditorPolygon>)handler;
                    }
                }

                if (OnPolygonRemoved != null)
                {
                    foreach (var handler in OnPolygonRemoved.GetInvocationList())
                    {
                        OnPolygonRemoved -= (Action<EditorPolygon>)handler;
                    }
                }

                if (OnStepCreated != null)
                {
                    foreach (var handler in OnStepCreated.GetInvocationList())
                    {
                        OnStepCreated -= (Action<EditorStepData>)handler;
                    }
                }

                if (OnStepItemSelected != null)
                {
                    foreach (var handler in OnStepItemSelected.GetInvocationList())
                    {
                        OnStepItemSelected -= (Action<EditorStepItem>)handler;
                    }
                }

                if (OnStepItemRemoved != null)
                {
                    foreach (var handler in OnStepItemRemoved.GetInvocationList())
                    {
                        OnStepItemRemoved -= (Action<EditorStepItem>)handler;
                    }
                }

                if (OnPolygonAddedToStep != null)
                {
                    foreach (var handler in OnPolygonAddedToStep.GetInvocationList())
                    {
                        OnPolygonAddedToStep -= (Action<EditorStepData, EditorPolygon>)handler;
                    }
                }

                if (OnPolygonRemovedToStep != null)
                {
                    foreach (var handler in OnPolygonRemovedToStep.GetInvocationList())
                    {
                        OnPolygonRemovedToStep -= (Action<EditorStepData, EditorPolygon>)handler;
                    }
                }

                if (OnPolygonItemSelected != null)
                {
                    foreach (var handler in OnPolygonItemSelected.GetInvocationList())
                    {
                        OnPolygonItemSelected -= (Action<EditorPolygonItem>)handler;
                    }
                }

                if (OnStepSelected != null)
                {
                    foreach (var handler in OnStepSelected.GetInvocationList())
                    {
                        OnStepSelected -= (Action<EditorStepData>)handler;
                    }
                }

                if (OnSelectFlower != null)
                {
                    foreach (var handler in OnSelectFlower.GetInvocationList())
                    {
                        OnSelectFlower -= (Action<EditorPolygon,int>)handler;
                    }
                }

                if (OnFlowerIdSelected != null)
                {
                    foreach (var handler in OnFlowerIdSelected.GetInvocationList())
                    {
                        OnFlowerIdSelected -= (Action<EditorPolygon,FlowerData>)handler;
                    }
                }
            }
        }

    }
}
