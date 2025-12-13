using System;
using DVLib.Graphics;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DotPuzzle.Editor
{
    public class MainCanvas : VisualElement
    {
        private EditorLevelDataSave _editorLevelData;

        private EditorToolType _toolType = EditorToolType.Vertex;

        public EditorToolType ToolType => _toolType;

        public EditorLevelDataSave EditorLevelData => _editorLevelData;

        private EditorStepData _selectedStep = null;

        private EditorPolygon _selectedPolygon = null;

        private EditorVertex _selectedVertex = null;

        private bool _isMouseDown = false;

        public MainCanvas()
        {
            style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
            generateVisualContent+= GenerateVisualContent;

            ResizeCanvas();

            this.focusable = true;
            this.Focus();
            this.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            this.RegisterCallback<MouseDownEvent>(OnMouseDown);
            this.RegisterCallback<MouseUpEvent>(OnMouseUp);
            this.RegisterCallback<KeyDownEvent>(OnKeyDown);
            this.RegisterCallback<KeyUpEvent>(OnKeyUp);


            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

            CreateNewLevel();

            EventHandlers();
        }

        private void EventHandlers()
        {
            EditorEventManager.EditorEvents.OnNeedUpdateCanvas += EditorEventManager_OnNeedUpdateCanvas;
            EditorEventManager.EditorEvents.OnGridSizeChanged += EditorEventManager_OnGridSizeChanged;
            EditorEventManager.EditorEvents.OnImagePreviewSelected += EditorEvents_OnImagePreviewSelected;
            EditorEventManager.EditorEvents.OnImagePreviewPositionChanged += EditorEvents_OnImagePreviewPositionChanged;
            EditorEventManager.EditorEvents.OnImagePreviewSizeChanged += EditorEvents_OnImagePreviewSizeChanged;
            EditorEventManager.EditorEvents.OnImagePreviewOpacityChanged += EditorEvents_OnImagePreviewOpacityChanged;

            EditorEventManager.EntityEvents.OnStepItemSelected += EditorEventManager_OnStepItemSelected;
            EditorEventManager.EntityEvents.OnStepItemRemoved += EditorEventManager_OnStepItemRemoved;
            EditorEventManager.EntityEvents.OnStepCreated += EditorEventManager_OnStepCreated;
            EditorEventManager.EntityEvents.OnStepSelected += EntityEvents_OnStepSelected;

            EditorEventManager.EntityEvents.OnPolygonAddedToStep += EditorEventManager_OnPolygonAddedToStep;
            EditorEventManager.EntityEvents.OnPolygonRemovedToStep += EditorEventManager_OnPolygonRemovedToStep;
            EditorEventManager.EntityEvents.OnPolygonItemSelected += EntityEvents_OnPolygonItemSelected;
            EditorEventManager.EntityEvents.OnPolygonSelected += EntityEvents_OnPolygonSelected;
            EditorEventManager.EntityEvents.OnPolygonRemoved += EntityEvents_OnPolygonRemoved;

            EditorEventManager.EntityEvents.OnVertexSelected += EntityEvents_OnVertexSelected;
            EditorEventManager.EntityEvents.OnVertexRemoved += EntityEvents_OnVertexRemoved;
            EditorEventManager.EntityEvents.OnFlowerIdSelected += EntityEvents_OnFlowerIdSelected;

        }

        private void EntityEvents_OnStepSelected(EditorStepData step)
        {
            SelectStep(step);
            SelectPolygon(null);
            SelectVertex(null);
            MarkDirtyRepaint();
        }

        private void EntityEvents_OnPolygonRemoved(EditorPolygon polygon)
        {
            MarkDirtyRepaint();
        }

        private void EntityEvents_OnPolygonSelected(EditorPolygon polygon)
        {
            if (_selectedPolygon != null)
            {
                _selectedPolygon.Selected = false;
            }
            _selectedPolygon = polygon;
            if (_selectedPolygon != null)
            {
                _selectedPolygon.Selected = true;
            }

            MarkDirtyRepaint();
        }

        private void EntityEvents_OnVertexRemoved(EditorVertex vertex)
        {
            //_editorLevelData.RemoveVertex(vertex);
            MarkDirtyRepaint();
        }

        private void EntityEvents_OnVertexSelected(EditorVertex vertex)
        {
            if (_selectedVertex != null)
            {
                _selectedVertex.Selected = false;
            }
            _selectedVertex = vertex;
            if (_selectedVertex != null)
            {
                _selectedVertex.Selected = true;
            }
            MarkDirtyRepaint();
        }

        private void EditorEvents_OnImagePreviewOpacityChanged(float opacity)
        {
            _previewImageColor.a = opacity;
            MarkDirtyRepaint();
        }

        private void EditorEvents_OnImagePreviewSizeChanged(Vector2 size)
        {
            _previewRect.size = size;
            MarkDirtyRepaint();
        }

        private void EditorEvents_OnImagePreviewPositionChanged(Vector2 position)
        {
            _previewRect.position = new Vector2(position.x + EditorConfig.OffsetX,position.y + EditorConfig.OffsetY);
            MarkDirtyRepaint();
        }

        private void EditorEvents_OnImagePreviewSelected(Texture texture)
        {
            _previewImage = texture;
            _previewRect = new Rect(EditorConfig.OffsetX, EditorConfig.OffsetY, texture.width, texture.height);
            MarkDirtyRepaint();
        }

        private void EntityEvents_OnFlowerIdSelected(EditorPolygon polygon, FlowerData flowerData)
        {
            polygon.SetFlowerId(flowerData.id);
            MarkDirtyRepaint();
        }

        private void EntityEvents_OnPolygonItemSelected(EditorPolygonItem polygonItem)
        {
            SelectPolygon(polygonItem.Polygon);
            MarkDirtyRepaint();
        }

        private void EditorEventManager_OnPolygonRemovedToStep(EditorStepData arg1, EditorPolygon polygon)
        {
            if (_selectedPolygon == polygon)
            {
                SelectPolygon(null);
            }
            
            MarkDirtyRepaint();
        }

        private void EditorEventManager_OnPolygonAddedToStep(EditorStepData arg1, EditorPolygon arg2)
        {
            MarkDirtyRepaint();
        }

        private void EditorEventManager_OnGridSizeChanged(int value)
        {
            EditorConfig.GridSize = value;
            MarkDirtyRepaint();
        }

        private void EditorEventManager_OnStepCreated(EditorStepData stepData)
        {
            _editorLevelData.AddStep(stepData);
        }

        private void EditorEventManager_OnStepItemRemoved(EditorStepItem stepItem)
        {
            _editorLevelData.RemoveStep(stepItem.StepData);
            SelectStep(null);
            MarkDirtyRepaint();
        }

        private void EditorEventManager_OnStepItemSelected(EditorStepItem stepItem)
        {
            SelectStep(stepItem.StepData);
            SelectPolygon(null);
            SelectVertex(null);
            MarkDirtyRepaint();
        }



        private void EditorEventManager_OnNeedUpdateCanvas()
        {
            MarkDirtyRepaint();
        }

        ~MainCanvas()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
        }

        public void CreateNewLevel()
        {
            _editorLevelData = new EditorLevelDataSave();
            _previewImage = null;
            MarkDirtyRepaint();
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            
        }

        public void SetToolType(EditorToolType toolType)
        {
            _toolType = toolType;
            SelectPolygon(null);
            SelectVertex(null);
            SelectStep(null);
            MarkDirtyRepaint();
        }
        #region Draw

        private void GenerateVisualContent(MeshGenerationContext mc)
        {
            DrawPreviewImage(mc);
            DrawGrid(mc);
            DrawCoordinates(mc);

            //if (_toolType == EditorToolType.Step)
            //{
            //    if (_selectedStep != null)
            //    {
            //        _selectedStep.Draw(mc);
            //    }
            //    _editorLevelData?.DrawInStepMode(mc);
            //}
            //else
            //{
            //    _editorLevelData?.Draw(mc);
            //}

            if (_editorLevelData != null)
            {
                _editorLevelData.Draw(mc);

            }

        }



        void DrawGrid(MeshGenerationContext mc)
        {
            EditorConfig.Columns = (int)style.width.value.value / EditorConfig.GridSize;
            EditorConfig.Rows = (int)style.height.value.value / EditorConfig.GridSize;

            int w = EditorConfig.Columns * EditorConfig.GridSize;
            int h = EditorConfig.Rows * EditorConfig.GridSize;
            for (int row = 0; row <= EditorConfig.Rows; row++)
            {
                GraphicX.Line.Draw(mc, new Vector2(0, row * EditorConfig.GridSize),
                    new Vector2(w, row * EditorConfig.GridSize),
                    1, Color.gray);

            }
            for (int col = 0; col <= EditorConfig.Columns; col++)
            {
                GraphicX.Line.Draw(mc, new Vector2(col * EditorConfig.GridSize, 0),
                    new Vector2(col * EditorConfig.GridSize, h),
                    1, Color.gray);
            }
        }

        void DrawCoordinates(MeshGenerationContext mc)
        {
            int startRows = (-EditorConfig.OffsetY / EditorConfig.GridSize);
            int endRows = (EditorConfig.CanvasHeight - EditorConfig.OffsetY)/EditorConfig.GridSize;

            int startCols = (-EditorConfig.OffsetX / EditorConfig.GridSize);
            int endCols = (EditorConfig.CanvasWidth - EditorConfig.OffsetX) / EditorConfig.GridSize;

            for (int y = startRows; y <= endRows; y += 5)
            {
                var pos = EditorConfig.ConvertIndexToPosition(0, y);
                pos.x -= 15;
                mc.DrawText(y.ToString(), pos, 10, Color.cyan);
            }

            for (int x = startCols; x < 0; x += 5)
            {
                var pos = EditorConfig.ConvertIndexToPosition(x, 0);
                mc.DrawText(x.ToString(), pos, 10, Color.cyan);
            }

            for (int x = 5; x <= endCols; x += 5)
            {
                var pos = EditorConfig.ConvertIndexToPosition(x, 0);
                mc.DrawText(x.ToString(), pos, 10, Color.cyan);
            }


            GraphicX.Line.Draw(mc,
                new Vector2(EditorConfig.OffsetX,0),
                new Vector2(EditorConfig.OffsetX, EditorConfig.CanvasHeight),
                1,
                Color.cyan);

            GraphicX.Line.Draw(mc,
                new Vector2(0, EditorConfig.OffsetY),
                new Vector2(EditorConfig.CanvasWidth, EditorConfig.OffsetY),
                1,
                Color.cyan);
        }

        private Color _previewImageColor = new Color(1,1,1,0.35f);
        private Texture _previewImage;
        private Rect _previewRect = new Rect(0,0,400,400);
        void DrawPreviewImage(MeshGenerationContext mc)
        {
            if (_previewImage != null)
            {
                GraphicX.TextureElement.DrawFlipY(mc, _previewImage, _previewRect, _previewImageColor);
            }
        }

        #endregion

        private bool _controlKeyDown = false;
        private bool _shiftKeyDown = false;
        private bool _addActionKey = false;

        private void OnKeyUp(KeyUpEvent evt)
        {
            _controlKeyDown = evt.ctrlKey;
            _shiftKeyDown = evt.shiftKey;
            _addActionKey = false;
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            _controlKeyDown = evt.ctrlKey;
            _shiftKeyDown = evt.shiftKey;
            if (evt.keyCode == KeyCode.N)
            {
                _addActionKey = true;
            }
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            _isMouseDown = false;
            MarkDirtyRepaint();
        }


        private void OnMouseDown(MouseDownEvent evt)
        {
            MarkDirtyRepaint();
            _isMouseDown = true;
            if (_toolType== EditorToolType.Vertex)
            {
                VertexMouseDownProcess(evt);
            }
            else if (_toolType== EditorToolType.Polygon)
            {
                PolygonMouseDownProcess(evt);
            }
            else if (_toolType == EditorToolType.Step)
            {
                StepMouseDownProcess(evt);
            }
        }

        #region Vertex Mode

        private void VertexMouseDownProcess(MouseDownEvent evt)
        {
            //left mouse button
            if (evt.button == 0)
            {
                if (_addActionKey)
                {
                    var vertex = AddVertexWithoutSnap(evt.localMousePosition);
                    SelectVertex(vertex);
                }
                else
                {
                    //select Vertex
                    if (_editorLevelData.CheckHitVertex(evt.localMousePosition, out EditorVertex vertex))
                    {

                        if (_selectedVertex != vertex)
                        {
                            SelectVertex(vertex);
                        }
                    }
                    else
                    {
                        //unselect vertex
                        SelectVertex(null);
                    }
                }

            }
            //right mouse button
            else if(evt.button == 1)
            {
                if (_editorLevelData.CheckHitVertex(evt.localMousePosition, out EditorVertex vertex))
                {
                    RemoveVertex(vertex);
                }
            }
        }

        EditorVertex AddVertexSnap(Vector2 position)
        {
            var pos = position;
            float x = Mathf.RoundToInt(pos.x / EditorConfig.GridSize) * EditorConfig.GridSize;
            float y = Mathf.RoundToInt(pos.y / EditorConfig.GridSize) * EditorConfig.GridSize;
            var vertex = new EditorVertex(new Vector2(x, y));
            _editorLevelData.AddVertex(vertex);
            return vertex;
        }

        EditorVertex AddVertexWithoutSnap(Vector2 position)
        {
            var vertex = new EditorVertex(position);
            _editorLevelData.AddVertex(vertex);
            return vertex;
        }

        void RemoveVertex(EditorVertex vertex)
        {
            //_editorLevelData.RemoveVertex(vertex);
            EditorEventManager.EntityEvents.OnVertexRemovedEvent(vertex);
        }

        void SelectVertex(EditorVertex vertex)
        {
            EditorEventManager.EntityEvents.OnVertexSelectedEvent(vertex);
        }
        #endregion

        #region Polygon Mode


        void PolygonMouseDownProcess(MouseDownEvent evt)
        {
            //left mouse button
            if (evt.button == 0)
            {
                if (_addActionKey)
                {
                    AddPolygonProcess(evt);
                }
                else
                {
                    SelectPolygonProcess(evt);
                }

            }
            //right mouse button
            else if (evt.button == 1)
            {
                if (_selectedPolygon != null)
                {
                    SelectVertex(null);
                    RemovePolygon(_selectedPolygon);
                    /*
                    if (_controlKeyDown)
                    {
                        if (_editorLevelData.CheckHitVertex(evt.localMousePosition, out var vertex))
                        {
                            //_levelData.RemoveVertex(vertex);
                            if (_selectedPolygon.ContainerVertex(vertex))
                            {
                                _selectedPolygon.RemoveVertex(vertex);
                                if (_selectedPolygon.Vertices.Count<=2)
                                {
                                    RemovePolygon(_selectedPolygon);
                                    SelectPolygon(null);
                                    SelectVertex(null);
                                }
                            }
                        }
                    }
                    else
                    {
                        SelectVertex(null);
                        RemovePolygon(_selectedPolygon);
                    }
                    */
                }

            }
        }

        private void RemovePolygon(EditorPolygon polygon)
        {
            _editorLevelData.RemovePolygon(polygon);
            EditorEventManager.EntityEvents.OnPolygonRemovedEvent(polygon);
        }
        private void SelectPolygonProcess(MouseDownEvent evt)
        {
            if (_selectedPolygon != null)
            {
                if (_editorLevelData.CheckHitVertex(evt.localMousePosition, out var vertex))
                {
                    if (_selectedPolygon.ContainerVertex(vertex))
                    {
                        SelectVertex(vertex);
                    }
                    else
                    {
                        CheckSelectPolygon(evt);
                    }
                }
                else
                {
                    CheckSelectPolygon(evt);
                }
            }
            else
            {
                CheckSelectPolygon(evt);
            }
        }

        private void AddPolygonProcess(MouseDownEvent evt)
        {
            if (_editorLevelData.CheckHitVertex(evt.localMousePosition, out var vertex))
            {
                //create new polygon
                if (_selectedPolygon == null)
                {
                    EditorPolygon polygon = new EditorPolygon(0);
                    polygon.AddVertex(vertex);
                    _editorLevelData.AddPolygon(polygon);

                    SelectPolygon(polygon);
                }
                //add selected vertex to polygon
                else
                {
                    if (!_selectedPolygon.ContainerVertex(vertex))
                    {
                        //_selectedPolygon.AddVertex(vertex);
                        _selectedPolygon.InsertVertexAfter(_selectedVertex, vertex);
                    }
                }

                SelectVertex(vertex);
            }
        }

        void CheckSelectPolygon(MouseDownEvent evt)
        {
            if (_editorLevelData.CheckHitPolygon(evt.localMousePosition, out var polygon, out var vertex))
            {
                SelectPolygon(polygon);
                _selectedPolygon = polygon;
                if (vertex != null)
                {
                    SelectVertex(vertex);
                }
                else
                {
                    SelectVertex(_selectedPolygon.Vertices[^1]);
                }

            }
            else
            {
                SelectPolygon(null);
                SelectVertex(null);
            }
        }
        void SelectPolygon(EditorPolygon polygon)
        {
            EditorEventManager.EntityEvents.OnPolygonSelectedEvent(polygon);
        }

        #endregion

        #region Step Mode

        private void StepMouseDownProcess(MouseDownEvent evt)
        {
            if (evt.button == 0)
            {
                if (_addActionKey)
                {
                    if (_selectedStep != null)
                    {
                        AddPolygonToStep(evt);
                    }
                }
                else
                {
                    if (_editorLevelData.CheckHitStep(evt.localMousePosition, out var step, out var polygon))
                    {
                        if (_selectedStep != step)
                        {
                            EditorEventManager.EntityEvents.OnStepSelectedEvent(step);
                        }
                    }
                    //else
                    //{
                    //    //SelectStep(null);
                    //    EditorEventManager.EntityEvents.OnStepSelectedEvent(null);
                    //}
                }
            }
        }

        private void AddPolygonToStep(MouseDownEvent evt)
        {
            if(_editorLevelData.CheckHitPolygon(evt.localMousePosition,out var polygon, out var vertex))
            {
                if (!polygon.IsInStep)
                {
                    _selectedStep.AddPolygon(polygon);
                    EditorEventManager.EntityEvents.OnPolygonAddedToStepEvent(_selectedStep,polygon);
                }
                
            }
        }


        private void SelectStep(EditorStepData step)
        {
            if (_selectedStep != null)
            {
                _selectedStep.Selected = false;
            }

            _selectedStep = step;
            if (_selectedStep != null)
            {
                _selectedStep.Selected = true;
            }
        }
        #endregion

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (evt.button == 0 && _isMouseDown)
            {
                MarkDirtyRepaint();

                if (_toolType == EditorToolType.Vertex)
                {
                    if (_selectedVertex != null)
                    {
                        var pos = evt.localMousePosition;
                        if (!_controlKeyDown)
                        {
                            float x = Mathf.RoundToInt(pos.x / EditorConfig.GridSize) * EditorConfig.GridSize;
                            float y = Mathf.RoundToInt(pos.y / EditorConfig.GridSize) * EditorConfig.GridSize;
                            _selectedVertex.SetPosition(new Vector2(x, y));
                        }
                        else
                        {
                            _selectedVertex.SetPosition(pos);
                        }

                    }
                }

            }
            
        }

        private void ResizeCanvas()
        {
            style.width = EditorConfig.CanvasWidth;
            style.height = EditorConfig.CanvasHeight;

            MarkDirtyRepaint();
        }

        public string GetLevelText(out bool haveOrphans)
        {
            string text = String.Empty;
            LevelDataSave levelDataSave = new LevelDataSave
            {
                maxHp = _editorLevelData.MaxHp,
                insectLifeTime = _editorLevelData.InsectLifeTime
            };
            levelDataSave.allUniqueVertices = new List<VertexDataSave>(_editorLevelData.Vertices.Count);


            int idCount = 0;
            int orphans = 0;
            foreach (var vertex in _editorLevelData.Vertices)
            {
                if (!vertex.IsInPolygon)
                {
                    orphans++;
                }
                vertex.Id = idCount;
                idCount++;

                VertexDataSave vertexDataSave = new VertexDataSave
                {
                    id = vertex.Id,
                    position = new Vector3((vertex.Position.x - EditorConfig.OffsetX) / EditorConfig.LevelSizeRatio,
                        (vertex.Position.y - EditorConfig.OffsetY) / EditorConfig.LevelSizeRatio)
                };
                levelDataSave.allUniqueVertices.Add(vertexDataSave);

            }

            idCount = 0;
            foreach (var polygon in _editorLevelData.Polygons)
            {
                if (!polygon.IsEmpty)
                {
                    if (!polygon.IsInStep)
                    {
                        orphans++;
                    }
                    polygon.Id = idCount;
                    idCount++;
                }

            }

            
            levelDataSave.steps = new List<StepDataSave>();
            foreach (var step in _editorLevelData.Steps)
            {
                if (step.Polygons.Count > 0)
                {
                    StepDataSave stepDataSave = new StepDataSave
                    {
                        stepId = step.Id
                    };

                    foreach (var polygon in step.Polygons)
                    {

                        PolygonDataSave polygonDataSave = new PolygonDataSave
                        {
                            flowerId = polygon.FlowerId,
                            polygonId = polygon.Id
                        };
                        foreach (var vertex in polygon.Vertices)
                        {
                            polygonDataSave.verticeIds.Add(vertex.Id);
                        }
                        stepDataSave.polygons.Add(polygonDataSave);
                    }
                    levelDataSave.steps.Add(stepDataSave);
                }
            }

            if (orphans > 0)
            {
                haveOrphans = true;
            }
            else
            {
                haveOrphans = false;
            }
            text = JsonUtility.ToJson(levelDataSave);
            return text;
        }

        public void SetLevelDataSave(LevelDataSave levelData)
        {
            //Level Properties
            EditorLevelDataSave editorLevelDataSave = new EditorLevelDataSave
            {
                MaxHp = levelData.maxHp,
                InsectLifeTime = levelData.insectLifeTime,
                Vertices = new List<EditorVertex>(levelData.allUniqueVertices.Count),
                Steps = new List<EditorStepData>(levelData.steps.Count),
                Polygons = new List<EditorPolygon>()
            };

            //Level Vertices
            foreach (var vertexDataSave in levelData.allUniqueVertices)
            {
                var pos = new Vector2(EditorConfig.OffsetX + vertexDataSave.position.x * EditorConfig.LevelSizeRatio,
                    EditorConfig.OffsetY + vertexDataSave.position.y * EditorConfig.LevelSizeRatio);
                EditorVertex vertex = new EditorVertex(pos)
                {
                    Id = vertexDataSave.id
                };
                editorLevelDataSave.AddVertexToData(vertex);
            }

            //Level Steps
            foreach (var levelDataStep in levelData.steps)
            {
                EditorStepData stepData = new EditorStepData
                {
                    Id = levelDataStep.stepId
                };

                foreach (var polygonDataSave in levelDataStep.polygons)
                {
                    EditorPolygon editorPolygon = new EditorPolygon(polygonDataSave.flowerId)
                    {
                        Id = polygonDataSave.polygonId,
                        IsInStep = true
                    };

                    foreach (var vertexId in polygonDataSave.verticeIds)
                    {
                        if (vertexId >= 0 && vertexId < editorLevelDataSave.Vertices.Count)
                        {
                            EditorVertex vertex = editorLevelDataSave.Vertices[vertexId];
                            editorPolygon.AddVertex(vertex);
                        }
                    }

                    editorLevelDataSave.AddPolygonToData(editorPolygon);
                    stepData.Polygons.Add(editorPolygon);
                }

                editorLevelDataSave.AddStepToData(stepData);
            }
            _editorLevelData = editorLevelDataSave;

            MarkDirtyRepaint();
        }

    }
}

