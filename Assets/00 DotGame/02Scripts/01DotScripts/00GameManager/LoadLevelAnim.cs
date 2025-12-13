
using DG.Tweening;
using Inwave.DongA.DotPuzzle.Manager;
using UnityEngine;
using UnityEngine.UI;

public class LoadLevelAnim : MonoBehaviour
{
    [Header("Elements")]
    public CanvasGroup loadingPanel;
    public Image loadingBar;
    
    [Header("Settings")]
    public float fillDuration = 1.5f;
    public float fadeDuration = 1.5f;      

    void Start()
    {
        GameManager.Instance.OnStartLoadLevelEvent += StartTransition;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnStartLoadLevelEvent -= StartTransition;
    }
    
    private void StartTransition()
    {
        loadingBar.fillAmount = 0;
        loadingPanel.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(loadingPanel.DOFade(1, 0f));
        
        sequence.Append(loadingBar.DOFillAmount(0.7f, fillDuration * 0.5f)
            .SetEase(Ease.OutQuad));
        
        sequence.Append(loadingBar.DOFillAmount(0.8f, fillDuration * 1.5f)
            .SetEase(Ease.Linear));
        
        sequence.Append(loadingBar.DOFillAmount(1f, fillDuration * 0.25f)
            .SetEase(Ease.OutQuad)); 

        // Fade out
        sequence.Append(loadingPanel.DOFade(0, fadeDuration));

        sequence.AppendCallback(() =>
        {
            
            loadingPanel.gameObject.SetActive(false);
        });
    }

    
}
