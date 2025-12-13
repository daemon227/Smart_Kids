
using Inwave.DongA.DotPuzzle.Manager;
using UnityEngine;

public class InsectWarningManager: MonoBehaviour
{
    public Transform parentObject;
    [SerializeField]private GameObject warningIconPrefab;
    [SerializeField]private Vector2 screenBoundsX = new Vector2(100, 100);
    [SerializeField]private Vector2 screenBoundsY = new Vector2(450, 300);
    
    private bool isWarningActive = true;
    
    private RectTransform warningIconPos;
    private RectTransform parentRect;
    private Canvas parentCanvas;

    private void Start()
    {
        var warningIcon = Instantiate(warningIconPrefab, parentObject);
        warningIconPos = warningIcon.GetComponent<RectTransform>();

        // Lấy RectTransform và Canvas của parent để dùng cho việc tính toán tọa độ
        if (parentObject != null)
        {
            parentRect = parentObject.GetComponent<RectTransform>();
            parentCanvas = parentObject.GetComponentInParent<Canvas>();
        }
    }

    private void Update()
    {
        if (!isWarningActive) return;
        
        // Don't show warning if game is over
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            warningIconPos.gameObject.SetActive(false);
            return;
        }
        
        CheckWarning();
    }

    bool IsVisibleByCamera(Camera cam, Renderer renderer)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    private void CheckWarning()
    {
        if (IsVisibleByCamera(Camera.main,this.GetComponent<Renderer>()))
        {
            warningIconPos.gameObject.SetActive(false);
            return;
        }

        warningIconPos.gameObject.SetActive(true);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
        
        if (screenPos.z < 0)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }
        
        screenPos.x = Mathf.Clamp(screenPos.x, screenBoundsX.x, Screen.width - screenBoundsX.y);
        screenPos.y = Mathf.Clamp(screenPos.y, screenBoundsY.x, Screen.height - screenBoundsY.y);
        
        if (parentRect != null && parentCanvas != null)
        {
            Camera uiCam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
                
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, uiCam, out localPoint))
            {
                warningIconPos.localPosition = localPoint;
            }
        }

        Vector3 dir = (transform.position - transform.transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        warningIconPos.rotation = Quaternion.Euler(0, 0, -angle);
    }
    
    public void DestroyWarning()
    {
        isWarningActive = false;
        Destroy(warningIconPos.gameObject);
    }

}
