
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Inwave.DongA.DotPuzzle.Manager
{
    public class HintManager : MonoBehaviour
    {
        public Image hintImage;
        
        private RectTransform rectTransform;
        
        private void Start()
        {
            ButtonEventManager.Instance.onHintButtonClick += ShowHint;
            rectTransform = hintImage.GetComponent<RectTransform>();
            ResetHint();
        }

        void OnDisable()
        {
            ButtonEventManager.Instance.onHintButtonClick -= ShowHint;
        }
        
        public void ShowHint()
        {
            GameManager.Instance.canInteract = false;
            hintImage.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOAnchorPos(new Vector2(0f, 0f), 0.5f));
            sequence.Append(hintImage.transform.DOScale(10, 1f));
            sequence.Join(hintImage.DOFade(0, 1f));
            
            sequence.OnComplete(() =>
            {
                var polygons = GameManager.Instance.levelData.allStep[GameManager.Instance.currentStepIndex].polygons;
                if (polygons.Count == 0) return;
                foreach (var polygon in polygons)
                {
                    if (!polygon.IsComplete && polygon.IsCentroidVisible())
                    {
                        var center = polygon.GetCentroid();
                        Camera.main.transform.DOMove(new Vector3(center.x, center.y, -10), 1);
                        LineManager.Instance.ShowDrawSuggetLine(polygon);
                        break;
                    }
                }

                GameManager.Instance.canInteract = true;
                ResetHint();
            });
            
        }

        private void ResetHint()
        {
            hintImage.gameObject.SetActive(false);
            hintImage.transform.localScale = Vector3.one;
            hintImage.DOFade(1, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, -1300f);
        }
        
    }
}
