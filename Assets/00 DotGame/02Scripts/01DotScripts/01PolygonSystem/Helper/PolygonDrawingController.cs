using System.Collections;
using Inwave.DongA.DotPuzzle.Entity;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.Helper;
using Inwave.DongA.Manager;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace Inwave.DongA.DotPuzzle.Manager
{
    public class PolygonDrawingController : MonoBehaviour
    {
        public Polygon polygonPrefab;
        private List<Polygon> allPolygons = new List<Polygon>();
        private List<Edge> allEdges = new List<Edge>();
        private List<Edge> completedEdges = new List<Edge>();
        
        // Step System
        private StepManager stepManager;
        private List<StepData> steps = new List<StepData>();
        private int currentStepIndex;
        
        private bool isDragging = false;
        private bool isClickEmptySpace = false;
        private Vector3 mouseDownPosition;
        private float dragThreshold = 0.5f; 
        private bool canDrawLine = true; 
        
        // service
        private Polygon tempPolygon;
        private PolygonBuilder polygonBuilder;
        private PolygonChecker polygonChecker;
        private EdgeManager edgeManager;
        private PathFindingAlgorithm pathFindingAlg;

        #region Unity Methods
        
        private void Start()
        {
            GameManager.Instance.OnLevelLoadedEvent += InitializeLevel;
            
            EventManager.Instance.FillAllPolygons += FillAllPolygons;
        }

        private void OnDestroy() 
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelLoadedEvent -= InitializeLevel;
            }
            EventManager.Instance.FillAllPolygons -= FillAllPolygons;
        }

        public IEnumerator OnLevelLoadedHandle()
        {
            yield return new WaitUntil(() => GameManager.Instance.isLevelLoaded);
            if (GameManager.Instance.levelData != null)
            {
                steps = GameManager.Instance.levelData.allStep;
                allPolygons = GameManager.Instance.levelData.GetAllPolygons();
            }
            else
            {
                Debug.LogError("levelData is null");
                yield break;
            }

            allEdges.Clear();
            completedEdges.Clear();
            
            FirstSetup();
            
            if (steps.Count > 0)
            {
                LineManager.Instance.ShowDrawSuggetLine(steps[0].polygons[0]);
                EventManager.Instance.OnGameStart?.Invoke(steps[currentStepIndex].polygons[0]);
            }
        }

        private void InitializeLevel()
        {
            StartCoroutine(OnLevelLoadedHandle());
        }
        
        private void Update()
        {
            if (GameManager.Instance.isGameOver || !GameManager.Instance.canInteract) return;
            HandleInput();
        }
        #endregion

        #region Polygon Building Methods
        
        private void FirstSetup()
        {
            stepManager = new StepManager();
            polygonBuilder = new PolygonBuilder();
            polygonChecker = new PolygonChecker();
            edgeManager = new EdgeManager();
            pathFindingAlg = new PathFindingAlgorithm();
            
            if (tempPolygon == null)
            {
                tempPolygon = Instantiate(polygonPrefab);
            }
            else
            {
                tempPolygon.ClearTemp(); 
                tempPolygon.vertices.Clear();
                tempPolygon.edges.Clear();
            }
            
            polygonBuilder.CreateAllEdges(allPolygons, allEdges);
            
            if (steps.Count > 0)
            {
                foreach (var step in steps)
                {
                    stepManager.ShowVerticesOfStep(step, false); // Hide all step vertices initially
                }
                currentStepIndex = 0;
                polygonBuilder.UpdateConnectionsForStep(steps[currentStepIndex].polygons);
                stepManager.ShowVerticesOfStep(steps[currentStepIndex], true);
                EventManager.Instance.ShowLevelData?.Invoke(steps[currentStepIndex].polygons);
            }
        }

        private void HandleInput()
        {
            if (Camera.main == null) return;
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = false;
                canDrawLine = true; // Reset flag
                mouseDownPosition = Input.mousePosition;

                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var vertice = GetVerticeClicked(mousePos, 0.15f);

                if (vertice != null)
                {
                    HandleVerticeClick(vertice);
                }
                else
                {
                    isClickEmptySpace = true;
                    if (isDragging)
                    {
                        canDrawLine = false;
                    }
                }
            }
            // Drag connect
            else if (Input.GetMouseButton(0))
            {
                float dragDistance = Vector3.Distance(Input.mousePosition, mouseDownPosition);
                
                if (dragDistance > dragThreshold)
                {
                    isDragging = true;
                    if (tempPolygon.vertices.Count == 0)
                    {
                        canDrawLine = false;
                    }
                    else
                    {
                        var dragStartPos = Camera.main.ScreenToWorldPoint(mouseDownPosition);
                        var dragStartVertice = GetVerticeClicked(dragStartPos, 0.15f);
                        if (dragStartVertice == null)
                        {
                            canDrawLine = false;
                            LineManager.Instance.DrawLine(tempPolygon.vertices);
                        }
                    }
                }

                if (tempPolygon.vertices.Count > 0 && canDrawLine)
                {
                    var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePos.z = 0;

                    var vertice = GetVerticeClicked(mousePos, 0.15f);
                    Vertice lastVertice = tempPolygon.vertices[tempPolygon.vertices.Count - 1];
                    
                    if (vertice == lastVertice)
                    {
                        LineManager.Instance.DrawLine(tempPolygon.vertices);
                    }
                    else
                    {
                        if(canDrawLine && isClickEmptySpace == false)LineManager.Instance.DrawLineWithMouse(tempPolygon.vertices, mousePos);
                    }
                    
                    if (vertice != null)
                    {
                        if (vertice != lastVertice)
                        {
                            HandleDragConnect(vertice);
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (tempPolygon.vertices.Count > 0)
                {
                    LineManager.Instance.DrawLine(tempPolygon.vertices);
                }

                if (isDragging && tempPolygon.vertices.Count > 0)
                {
                    var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var vertice = GetVerticeClicked(mousePos, 0.15f);
                    CheckReleasedPath();
                }
                else if (!isDragging && tempPolygon.vertices.Count > 0)
                {
                    var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var vertice = GetVerticeClicked(mousePos, 0.15f);
                    if (vertice == null)
                    {
                        polygonBuilder.CancelPolygon(tempPolygon, false, false);
                    }
                }
                
                // Reset trạng thái
                isDragging = false;
                canDrawLine = true;
                isClickEmptySpace = false;
            }
        }

        private Vertice GetVerticeClicked(Vector3 position, float radius)
        {
            Collider2D hit = Physics2D.OverlapCircle(position, radius);

            return hit != null ? hit.GetComponent<Vertice>() : null;
        }

        private void HandleVerticeClick(Vertice vertice)
        {
            if (!vertice.IsCanConnect()) return;

            vertice.ChangeVerticeSprite();

            if (tempPolygon.vertices.Contains(vertice))
                HandleClickOnExistingVertice(vertice);
            else
                HandleClickOnNewVertice(vertice);
        }


        private void HandleClickOnExistingVertice(Vertice vertice)
        {
            List<Vertice> verts = tempPolygon.vertices;
            int count = verts.Count;
            if (vertice == verts[count - 1]) return;

            if (count > 1 && vertice == verts[count - 2])
            {
                
                verts[count - 1].ChangeVerticeSprite(false, false);
                tempPolygon.vertices.RemoveAt(count - 1);
                if (tempPolygon.edges.Count > 0)
                {
                    tempPolygon.edges.RemoveAt(tempPolygon.edges.Count - 1);
                }
                LineManager.Instance.DrawLine(tempPolygon.vertices);
                return;
            }

            if (count < 3 )
            {
                polygonBuilder.CancelPolygon(tempPolygon, true);
                return;
            }

            if (vertice == verts[0])
            {
                TryCompletePolygon(vertice);
            }
            else
            {
                polygonBuilder.CancelPolygon(tempPolygon, true);
            }
        }

        private void TryCompletePolygon(Vertice first)
        {
            var verts = tempPolygon.vertices;
            int count = verts.Count;

            Edge closingEdge = new Edge(verts[count - 1], first);
            tempPolygon.AddEdgeIfNotExist(closingEdge);

            var polygon = polygonChecker.GetSamePolygonByEdge(allPolygons, tempPolygon);
            if (polygon != null)
            {
                polygonBuilder.BuildPolygon(polygon);
                edgeManager.MergeEdgesToCompledEdges(completedEdges, tempPolygon.edges);
                
                DetectHiddenPolygons();

                CheckStepStatus(); 

                ResetTempPolygon();
            }
            else
            {
                Debug.Log("don't have same polygon");
                polygonBuilder.CancelPolygon(tempPolygon, true);
            }
        }

        private void HandleClickOnNewVertice(Vertice vertice)
        {
            tempPolygon.vertices.Add(vertice);

            var vertices = tempPolygon.vertices;
            int count = vertices.Count;

            Edge newEdge = null;

            if (count > 1)
            {
                newEdge = new Edge(vertices[count - 1], vertices[count - 2]);
                tempPolygon.AddEdgeIfNotExist(newEdge);
            }

            bool hasMainPolygon = false;

            // Check main polygon
            var samePoly = polygonChecker.GetSamePolygonByEdge(allPolygons, tempPolygon);
            if (samePoly != null)
            {
                polygonBuilder.BuildPolygon(samePoly);
                edgeManager.MergeEdgesToCompledEdges(completedEdges, tempPolygon.edges);
                hasMainPolygon = true;
                CheckStepStatus(); // Kiểm tra step
            }

            // Build temp edges for detect hint polygon
            List<Edge> tempCompleteEdges = new List<Edge>(completedEdges);
            if (newEdge != null)
            {               
                tempPolygon.AddEdgeIfNotExist(newEdge);
                if (edgeManager.CheckEdgeInteraction(completedEdges, tempPolygon.edges, newEdge))
                {
                    polygonBuilder.CancelPolygon(tempPolygon, true);
                    ResetTempPolygon();
                    return;
                }
            }

            // Detect hint polygon
            var detectedPolygons = polygonChecker.GetDetectedPolygons(allPolygons, tempCompleteEdges, tempPolygon.edges);
            if (detectedPolygons.Count > 0)
            {
                foreach (var p in detectedPolygons)
                {
                    polygonBuilder.BuildPolygon(p);
                    edgeManager.MergeEdgesToCompledEdges(completedEdges, tempPolygon.edges);
                    ResetTempPolygon();
                }
                CheckStepStatus(); // Kiểm tra step nếu có polygon ẩn được hoàn thành
            }
        
                //Check 
                if (!hasMainPolygon && detectedPolygons.Count == 0 && tempPolygon.vertices.Count > 0)
                {
                    if (tempPolygon.edges.Count >= 1)
                    {
                        Vertice startNode = tempPolygon.vertices[0];
                        if (pathFindingAlg.IsPathExist(vertice, startNode, completedEdges))
                        {
                            bool hasInvalidEdge = false;

                            //check edge in temp polygon
                            foreach (var edge in tempPolygon.edges)
                            {
                                bool existsInAllEdges = false;

                                foreach (var e in allEdges)
                                {
                                    if (e.EqualsIgnoreDirection(edge))
                                    {
                                        existsInAllEdges = true;
                                        break;
                                    }
                                }

                                if (!existsInAllEdges)
                                {
                                    hasInvalidEdge = true;
                                    break; // cạnh mới không hợp lệ
                                }
                            }
                            
                            if (!polygonChecker.DoAllTempEdgesBelongToSamePolygon(tempPolygon.edges, allPolygons))
                            {
                                polygonBuilder.CancelPolygon(tempPolygon, true);
                                ResetTempPolygon();
                                return;
                            }

                            if (hasInvalidEdge)
                            {
                                polygonBuilder.CancelPolygon(tempPolygon, true);
                                ResetTempPolygon();
                                return;
                            }
                        } 
                    }
                }
            
                    // Reset if don't have polygon
                    if (hasMainPolygon)
                {
                    ResetTempPolygon();
                    return;
            }

            LineManager.Instance.DrawLine(vertices);
        }

        #region Drag Connect Methods

        private void HandleDragConnect(Vertice vertice)
        {
            if (!vertice.IsCanConnect()) return;
            
            int count = tempPolygon.vertices.Count;
            if (count > 1)
            {
                if (vertice == tempPolygon.vertices[count - 2])
                {
                    tempPolygon.vertices[count - 1].ChangeVerticeSprite(false, false);
                    tempPolygon.vertices.RemoveAt(count - 1);
                    if (tempPolygon.edges.Count > 0)
                    {
                        tempPolygon.edges.RemoveAt(tempPolygon.edges.Count - 1);
                    }
                    
                    LineManager.Instance.DrawLine(tempPolygon.vertices);
                    return;
                }
            }

            // Cancel if connecting to an existing vertex in the path, unless it's the start vertex (for closing)
            if (tempPolygon.vertices.Contains(vertice))
            {
                if (vertice != tempPolygon.vertices[0])
                {
                    polygonBuilder.CancelPolygon(tempPolygon, true);
                    ResetTempPolygon();
                    isDragging = false;
                    return;
                }
            }

            // Logic kiểm tra cạnh trùng lặp
            if (count > 0)
            {
                Edge potentialEdge = new Edge(tempPolygon.vertices[count - 1], vertice);
                
                foreach (var edge in tempPolygon.edges)
                {
                    if (edge.EqualsIgnoreDirection(potentialEdge)) return;
                }
            }
            
            vertice.ChangeVerticeSprite();
            tempPolygon.vertices.Add(vertice);

            count = tempPolygon.vertices.Count;
            if (count > 1)
            {
                Edge newEdge = new Edge(tempPolygon.vertices[count - 2], vertice);
                tempPolygon.AddEdgeIfNotExist(newEdge);
            }
            
        }
        
        private void CheckReleasedPath()
        {
            if (tempPolygon.vertices.Count < 2) return;

            Vertice first = tempPolygon.vertices[0];
            Vertice last = tempPolygon.vertices[tempPolygon.vertices.Count - 1];
                
            //check main polygon
            if (last == first && tempPolygon.vertices.Count >= 4) 
            {
                var polygon = polygonChecker.GetSamePolygonByEdge(allPolygons, tempPolygon);
                if (polygon != null)
                {
                    polygonBuilder.BuildPolygon(polygon);
                    edgeManager.MergeEdgesToCompledEdges(completedEdges, tempPolygon.edges);
                    DetectHiddenPolygons();
                    CheckStepStatus();
                    ResetTempPolygon();
                    return;
                }
                else
                {
                    Debug.Log("don't have same polygon");
                    polygonBuilder.CancelPolygon(tempPolygon, true);
                    ResetTempPolygon();
                    return;
                }
            }

            //check edges
            bool isValid = true;
            
            foreach (var edge in tempPolygon.edges)
                {
                    if (edgeManager.CheckEdgeInteraction(completedEdges, tempPolygon.edges, edge))
                    {
                        isValid = false;
                        break;
                    }
                }

            if (!isValid)
            {
                Debug.Log("Invalid edges in temp polygon");
                polygonBuilder.CancelPolygon(tempPolygon, true);
                ResetTempPolygon();
                return;
            }

            // Check 3: Detect Hidden Polygon
            List<Edge> tempCompleteEdges = new List<Edge>(completedEdges);
            var detectedPolygons = polygonChecker.GetDetectedPolygons(allPolygons, tempCompleteEdges, tempPolygon.edges);
            if (detectedPolygons.Count > 0)
            {
                foreach (var p in detectedPolygons)
                {
                    polygonBuilder.BuildPolygon(p);
                    edgeManager.MergeEdgesToCompledEdges(completedEdges, tempPolygon.edges);
                    ResetTempPolygon();
                }
                CheckStepStatus();
            }
            
            if (pathFindingAlg.IsPathExist(first, last, completedEdges))
            {
                bool hasInvalidEdge = false;

                //check edge in temp polygon
                foreach (var edge in tempPolygon.edges)
                {
                    bool existsInAllEdges = false;

                    foreach (var e in allEdges)
                    {
                        if (e.EqualsIgnoreDirection(edge))
                        {
                            existsInAllEdges = true;
                            break;
                        }
                    }

                    if (!existsInAllEdges)
                    {
                        hasInvalidEdge = true;
                        break; // cạnh mới không hợp lệ
                    }
                }
                
                // if (!polygonChecker.DoAllTempEdgesBelongToSamePolygon(tempPolygon.edges, allPolygons))
                // {
                //     Debug.Log("bug1");
                //     polygonBuilder.CancelPolygon(tempPolygon, true);
                //     ResetTempPolygon();
                //     return;
                // }

                if (hasInvalidEdge)
                {
                    Debug.Log("bug2");
                    polygonBuilder.CancelPolygon(tempPolygon, true);
                    ResetTempPolygon();
                    return;
                }
            } 
            
        }

        #endregion
       

        private void ResetTempPolygon()
        {
            tempPolygon.ClearTemp();
        }
        #endregion

        #region Step System

        private void CheckStepStatus()
        {
            if (stepManager.IsStepComplete(steps[currentStepIndex]))
            {
                currentStepIndex++;
                EventManager.Instance.ChangeToNewStep?.Invoke();
                // Hiển thị step tiếp theo nếu còn
                if (currentStepIndex < steps.Count)
                {
                    polygonBuilder.UpdateConnectionsForStep(steps[currentStepIndex].polygons);
                    stepManager.ShowVerticesOfStep(steps[currentStepIndex], true);
                    EventManager.Instance.ShowLevelData?.Invoke(steps[currentStepIndex].polygons);
                    // Optional: Suggest line cho polygon đầu tiên của step mới
                    if (steps[currentStepIndex].polygons.Count > 0)
                    {
                        LineManager.Instance.ShowDrawSuggetLine(steps[currentStepIndex].polygons[0]);
                        GameManager.Instance.currentStepIndex++;
                        EventManager.Instance.OnStepComplete?.Invoke(steps[currentStepIndex].polygons[0]);
                    }
                        
                }
                else
                {
                    EventManager.Instance.OnAllPolygonsFilled?.Invoke();
                }
            }
        }

        #endregion

        #region Helper Methods
        
        private void DetectHiddenPolygons()
        {
            List<Edge> tempComplete = new List<Edge>(completedEdges);

            var detected = polygonChecker.GetDetectedPolygons(allPolygons, tempComplete, tempPolygon.edges);

            foreach (var p in detected)
            {
                polygonBuilder.BuildPolygon(p);
                edgeManager.MergeEdgesToCompledEdges(completedEdges, tempPolygon.edges);
                ResetTempPolygon();
            }
        }
        
        public void FillAllPolygons()
        {
            if (currentStepIndex > steps.Count-1) return;
            foreach (var polygon in steps[currentStepIndex].polygons)
            {
                if (polygon.IsComplete) continue;
                polygonBuilder.BuildPolygon(polygon);
            }
            CheckStepStatus();
        }
        #endregion
    }
}