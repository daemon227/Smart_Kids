
using System.Collections;
using DG.Tweening;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.Manager;
using UnityEngine;
using UnityEngine.UI;

public class GameEndingManager: MonoBehaviour
{
    [Header("Camera Move Setting")]
    public float cameraMoveDuration = 1.3f;
    public float cameraOrthoSize = 2f;
    public float cameraOrthoSizeDuration = 0.8f;
    public float delayBeforeGameOver = 2f;
    public float delayBeforeWin = 1f;
    [Range(1f, 2f)]
    public float winZoomPadding = 1.2f; // Hệ số padding, 1.2 nghĩa là chừa lề 20%
    
    [Header("Lose Setting")]
    public Image background;
    public Image hole;
    
    [Header("Win Setting")]
    public GameObject winParticle;
    private PolygonManager polygonManager;
    private void Start()
    {
        polygonManager = new PolygonManager();
        
        EventManager.Instance.OnLoseByBug+= LoseByBugHandle;
        EventManager.Instance.OnAllPolygonsFilled += WinGameHandle;
        
        GameManager.Instance.OnLevelLoadedEvent += ResetGameEndingState;
    }

    void OnDisable()
    {
        EventManager.Instance.OnLoseByBug -= LoseByBugHandle;
        EventManager.Instance.OnAllPolygonsFilled -= WinGameHandle;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelLoadedEvent -= ResetGameEndingState;
        }
    }
    
    private void ResetGameEndingState()
    {
        if (winParticle != null)
        {
            winParticle.SetActive(false); 
        }
    }

    private void LoseByBugHandle(IInsect insect)
    {
        GameManager.Instance.isGameOver = true;
        
        var insectPos = insect.GetPosition();
        float z = Camera.main.transform.position.z;
        
        background.gameObject.SetActive(true);
        hole.gameObject.SetActive(true);
        hole.transform.localScale = new Vector3(5, 5, 0);
        
        Sequence seq = DOTween.Sequence();

        seq.Append(Camera.main.transform.DOMove(new Vector3(insectPos.x, insectPos.y, z), cameraMoveDuration));
        seq.Append(Camera.main.DOOrthoSize(cameraOrthoSize,cameraOrthoSizeDuration).SetEase(Ease.OutQuad));
        seq.Append(hole.transform.DOScale(1.5f, 1f).SetEase(Ease.OutBack));
        seq.OnComplete(() =>
        {
            EventManager.Instance.OnFillWiltedFlower?.Invoke(insect.targetPolygon);
            insect.ShowEmote(true);
            StartCoroutine(LoseByBugCoroutine());
        });
        
    }

    private IEnumerator LoseByBugCoroutine()
    {
        GameManager.Instance.loseByBug = true;
        yield return new WaitForSeconds(delayBeforeGameOver);
        
        background.gameObject.SetActive(false);
        hole.gameObject.SetActive(false);
        
        EventManager.Instance.OnGameOver?.Invoke(false);
    }

    private void WinGameHandle()
    {
        GameManager.Instance.isGameOver = true;
        
        var (xRange, yRange) = polygonManager.GetGlobalPolygonRange();

        float contentWidth = xRange.y - xRange.x;
        float contentHeight = yRange.y - yRange.x;


        float centerX = (xRange.x + xRange.y) / 2f;
        float centerY = (yRange.x + yRange.y) / 2f;
        
        float screenRatio = Camera.main.aspect;
        float targetRatio = contentWidth / contentHeight;
        float targetSize = 0f;

        if (screenRatio >= targetRatio)
        {
            targetSize = contentHeight / 2f;
        }
        else
        {
            targetSize = contentWidth / (2f * screenRatio);
        }
        
        targetSize *= winZoomPadding;

        Camera.main.transform.DOMove(new Vector3(centerX, centerY, -10), cameraMoveDuration);
        Camera.main.DOOrthoSize(targetSize, cameraOrthoSizeDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            winParticle.SetActive(true);
            StartCoroutine(EndGameCoroutine());
        });
    }

    private IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeWin);
        EventManager.Instance.OnGameOver?.Invoke(true);
    }
    
}
