using System.Collections;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.Manager;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Inwave.DongA.DotPuzzle.InsectSystem;
using UnityEngine.UI;

namespace Inwave.DongA.DotPuzzle.UIManager
{
    public class InsectSystemUIManager : MonoBehaviour
    {
        [Header("Managers")]
        public InsectManager insectManager;
        [Header("UI Settings")]
        public GameObject warningUI;
        public float warningTime = 5f;
        
        public GameObject timeCountUI;
        public Image timeCountBar;
        public Image aura;
        public TextMeshProUGUI timeText;
        public GameObject warningEffect;

        private bool isCounting = false;
        
        private CanvasGroup warningCg;
        private void Start()
        {
            EventManager.Instance.OnInsectSpawned += ShowInsectUI;
            EventManager.Instance.OnEndTurn += EndCountDown;
            EventManager.Instance.OnLoseByBug += EndGame ;
            EventManager.Instance.OnAllPolygonsFilled += EndCountDown;
            GameManager.Instance.OnLevelLoadedEvent += ResetUI;
            
            warningCg = warningUI.GetComponent<CanvasGroup>();
        }

        private void OnDisable()
        {
            EventManager.Instance.OnInsectSpawned -= ShowInsectUI;
            EventManager.Instance.OnEndTurn -= EndCountDown;
            EventManager.Instance.OnLoseByBug -= EndGame ;
            EventManager.Instance.OnAllPolygonsFilled -= EndCountDown;
            EventManager.Instance.OnLoseByBug -= EndGame ;
            EventManager.Instance.OnAllPolygonsFilled -= EndCountDown;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelLoadedEvent -= ResetUI;
            }
        }

        private void ResetUI()
        {
            StopAllCoroutines();
                
            isCounting = false;

            if (warningUI != null)
            {
                if (warningCg != null)
                {
                    warningCg.DOKill();
                    warningCg.alpha = 1f;
                }
                warningUI.SetActive(false);
            }

            if (aura != null)
            {
                aura.transform.DOKill();
            }
            if(timeCountUI != null) timeCountUI.SetActive(false);
            
            if (aura != null)
            {
                aura.DOKill();
                var c = aura.color;
                c.a = 0;
                aura.color = c;
            }
        }

        private IEnumerator ShowWarningUI()
        {
            warningUI.SetActive(true);
           
            warningCg.DOKill();
            warningCg.alpha = 1f;

            warningCg.DOFade(0f, 0.7f)
                .SetLoops(-1, LoopType.Yoyo);
            
            float timer = 0;
            while (timer < warningTime)
            {
                 if (!GameManager.Instance.isPaused)
                 {
                     timer += Time.deltaTime;
                 }
                 yield return null;
            }
            
            warningCg.DOKill();
            warningCg.alpha = 1f;
            warningUI.SetActive(false);
        }
        
        private void ShowInsectUI()
        {
            //if (GameManager.Instance.isPaused) return;
            if (isCounting)
            {
                StopAllCoroutines();
                isCounting = false;
            }
            StartCoroutine(ShowWarningAndCountDown());
        }

        private IEnumerator ShowWarningAndCountDown()
        {
            //isCounting = true;
            isCounting = true;
            yield return StartCoroutine(ShowWarningUI());
            
            insectManager.StartTimer(); 
            
            StartCoroutine(CountDownCoroutine());
        }

        private IEnumerator CountDownCoroutine()
        {
            float timeToShowAura = insectManager.currentLifeTime * 0.35f;
            bool canShowAura = true;
            isCounting = true;
            timeCountUI.SetActive(true);
            timeCountUI.transform.DOScale(1f, 0.5f).From(0);
            
            bool wasPaused = false;

            while (insectManager.currentLifeTime > 0 && isCounting)
            {
                if(GameManager.Instance.isPaused) 
                {
                    if (!wasPaused)
                    {
                        if (!canShowAura)
                        {
                            aura.DOPause();
                            aura.transform.DOPause();
                        }
                        wasPaused = true;
                    }
                    yield return null;
                    continue;
                }
                    
                if (wasPaused)
                {
                    // Resume lại tween nếu aura đang hiện
                    if (!canShowAura)
                    {
                        aura.DOPlay();
                        aura.transform.DOPlay();
                    }
                    wasPaused = false;
                }

                timeCountBar.fillAmount = insectManager.currentLifeTime / insectManager.bugLifeTime;
                timeText.text = insectManager.insects.Count.ToString();

                if (insectManager.currentLifeTime <= timeToShowAura && canShowAura)
                {
                    ShowAura();
                    warningEffect.SetActive(true);
                    canShowAura = false;
                }
                yield return null; 
            }
        }

        private void EndGame(IInsect insect)
        {
            EndCountDown();
        }
        private void EndCountDown()
        {
            isCounting = false;
            timeCountUI.SetActive(false);
            warningEffect.SetActive(false);
            if (warningUI != null)
            {
                var cg = warningUI.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.DOKill();
                    cg.alpha = 1f;
                }
                warningUI.SetActive(false); 
            }

            DOTween.Kill(aura.transform);
            DOTween.Kill(aura);
            aura.gameObject.SetActive(false);
            
            StopAllCoroutines(); 
        }

        private void ShowAura()
        {
            aura.gameObject.SetActive(true);
            aura.DOKill();
            aura.transform.localScale = Vector3.one; 
            
            var c = aura.color;
            c.a = 1f;
            aura.color = c;
            aura.transform.DOScale(2f, 1.5f).SetLoops(-1, LoopType.Restart);
            aura.DOFade(0f, 1.5f).SetLoops(-1, LoopType.Restart);
        }
    }
}
