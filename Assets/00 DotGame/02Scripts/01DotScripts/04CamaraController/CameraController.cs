using System.Collections;
using DG.Tweening;
using Inwave.DongA.DotPuzzle.Entity;
using Inwave.DongA.DotPuzzle.Event;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Inwave.DongA.DotPuzzle.Manager
{
    public class CameraController : MonoBehaviour
    {
        [Header("First Camera Settings")]
        public float firstZoom = 20f;
        public float firstZoomDuration = 1f;
        [Header("Camera Settings")]
        public float cameraSpeed = 1.5f;
        public float dragRange = 10f;
        public float zoomSpeed = 1f;
        public float minZoom = 2.5f;
        public float defaultZoom = 4f;
        public float maxZoom = 6f;
        
        public float zoomDuration = 1f;
        public float cameraMoveDuration = 1f;
        
        [Header("Camera UI Elements")]
        public Button cameraResetButton;
        [SerializeField]private Vector2 screenBoundsX = new Vector2(100, 100);
        [SerializeField]private Vector2 screenBoundsY = new Vector2(450, 300);

        private Vector3 lastMousePosition;
        private bool isDragging = false;
        
        private Camera cam;

        private Vector2 xRange = new Vector2(float.MaxValue, float.MinValue);
        private Vector2 yRange = new Vector2(float.MaxValue, float.MinValue);
        private PolygonManager polygonManager;
        private Polygon currentPolygon;

        
        IEnumerator Start()
        {
            cam = Camera.main;
            GameManager.Instance.OnLevelLoadedEvent += InitializeCameraForNewLevel;
            yield return new WaitUntil(() => GameManager.Instance.isLevelLoaded);
            EventManager.Instance.OnStepComplete += MoveToNewStep;
            EventManager.Instance.FilledOnePolygon += SetNewPolygon;

            cameraResetButton.onClick.AddListener(OnCameraResetButtonClick);
        }
        void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelLoadedEvent -= InitializeCameraForNewLevel;
            }
            EventManager.Instance.OnStepComplete -= MoveToNewStep;
            EventManager.Instance.FilledOnePolygon -= SetNewPolygon;
        }
        void Update()
        {
            if (GameManager.Instance.isGameOver || !GameManager.Instance.canInteract)
            {
                isDragging = false;
                return;
            }
            CheckPolygonPosition();
            
            if (Input.touchCount == 2)
            {
                isDragging = false; 
                ZoomCamera();
            }
            else
            {
                DragCamera();
                ZoomCamera();
            }
        }
        private void InitializeCameraForNewLevel()
        {
            polygonManager = new PolygonManager();
            (xRange, yRange) = polygonManager.GetGlobalPolygonRange();
            FirstSetup(GameManager.Instance.levelData.GetAllPolygons()[0]);
            currentPolygon = null;
            cameraResetButton.gameObject.SetActive(false);
        
        }
        

        private void FirstSetup(Polygon polygon)
        {
            if (Camera.main == null) return;
            Camera.main.orthographicSize = firstZoom;
            GameManager.Instance.canInteract = false;
            var centroid = polygon.GetCentroid();
            Camera.main.transform.position = new Vector3(centroid.x, centroid.y, -10);
            Camera.main.DOOrthoSize(defaultZoom,firstZoomDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                GameManager.Instance.canInteract = true;
            });
        }
        
        private void DragCamera()
        {
            if (Camera.main == null) return;
            if (Input.GetMouseButtonDown(0) && !isDragging)
            {
                lastMousePosition = Input.mousePosition;
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapCircle(worldPoint, 0.3f);
                if (hit != null)
                {
                    hit = null;
                }
                else
                {
                    isDragging = true;
                }
                return;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                return;
            }
            if (Input.GetMouseButton(0) && isDragging)
            {
                if (Camera.main == null) return;
                Vector3 pos1 = Camera.main.ScreenToWorldPoint(lastMousePosition);
                Vector3 pos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 difference = pos1 - pos2;
                difference.z = 0;
                transform.position += difference * cameraSpeed;
                lastMousePosition = Input.mousePosition;
                ResetCamera();
            }
        }

        private void ZoomCamera()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                cam.orthographicSize -= scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
                
                ResetCamera();
            }

            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
                
                if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
                {
                    return;
                }

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;


                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                
                cam.orthographicSize += deltaMagnitudeDiff * zoomSpeed * 0.01f;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

            }
        }
        private void ResetCamera()
        {
            float clampedX = Mathf.Clamp(transform.position.x, xRange.x, xRange.y + dragRange);
            float clampedY = Mathf.Clamp(transform.position.y, yRange.x - dragRange, yRange.y + dragRange);

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }

        
        private void MoveToNewStep(Polygon polygon)
        {
            GameManager.Instance.canInteract = false;
            Camera.main.DOOrthoSize(8,zoomDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                var centroid = polygon.GetCentroid();
                Camera.main.transform.DOMove(new Vector3(centroid.x,centroid.y,-10), cameraMoveDuration).OnComplete(() =>
                {
                    Camera.main.DOOrthoSize(defaultZoom,zoomDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        GameManager.Instance.canInteract = true;
                    });
                });
            });
            (xRange, yRange) = polygonManager.GetGlobalPolygonRange();
            
        }

#region Camera UI Methods
        private void MoveToNewPolygon(Polygon polygon)
        {
            GameManager.Instance.canInteract = false;
            var centroid = polygon.GetCentroid();
            Camera.main.transform.DOMove(new Vector3(centroid.x,centroid.y,-10), cameraMoveDuration).OnComplete(() =>
            {
                GameManager.Instance.canInteract = true;
            });
        }

        private void SetNewPolygon(Polygon polygon)
        {
            currentPolygon = polygon;
        }

        private void CheckPolygonPosition()
        {
            if (currentPolygon == null) return;
            Vector3 camPos = Camera.main.transform.position;
            Vector3 polyCentroid = currentPolygon.GetCentroid();
            float distance = Vector3.Distance(new Vector3(camPos.x, camPos.y, 0), new Vector3(polyCentroid.x, polyCentroid.y, 0));
            if (distance > 5f)
            {
                cameraResetButton.gameObject.SetActive(true);
                //UpdateResetButtonPosition(polyCentroid);
            }
            else
            {
                cameraResetButton.gameObject.SetActive(false);
            }
        }

        private void UpdateResetButtonPosition(Vector3 targetPos)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(targetPos);
            Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Vector3 dir = screenPos - center;

            float minX = screenBoundsX.x;
            float maxX = Screen.width - screenBoundsX.y;
            float minY = screenBoundsY.x;
            float maxY = Screen.height - screenBoundsY.y;

            float t = float.MaxValue;

            if (dir.x != 0)
            {
                float tx = (dir.x > 0) ? (maxX - center.x) / dir.x : (minX - center.x) / dir.x;
                if (tx > 0) t = Mathf.Min(t, tx);
            }

            if (dir.y != 0)
            {
                float ty = (dir.y > 0) ? (maxY - center.y) / dir.y : (minY - center.y) / dir.y;
                if (ty > 0) t = Mathf.Min(t, ty);
            }

            if (t < float.MaxValue)
            {
                Vector2 pos = center + dir * t;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(cameraResetButton.transform.parent as RectTransform, pos, cam, out Vector2 localPoint))
                {
                    cameraResetButton.transform.localPosition = localPoint;
                }
            }
        }

        public void OnCameraResetButtonClick()
        {
            if (currentPolygon == null) return;
            MoveToNewPolygon(currentPolygon);
            cameraResetButton.gameObject.SetActive(false);
        }

        
#endregion

    }
}
