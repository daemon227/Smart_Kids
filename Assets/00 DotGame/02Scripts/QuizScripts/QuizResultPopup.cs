using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace DACN.Quiz
{
    public class QuizResultPopup : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject winPanel;
        public GameObject losePanel;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button replayButton2;
        //[SerializeField] private CanvasGroup canvasGroup;
        private GameObject currentActivePanel;

        [Header("Settings")]
        [SerializeField] private float animationDuration = 0.5f;

        public System.Action OnNextButtonClicked;
        public System.Action OnReplayButtonClicked;

        private void Start()
        {
            nextButton.onClick.AddListener(OnNextClicked);
            replayButton.onClick.AddListener(OnReplayClicked);
            replayButton2.onClick.AddListener(OnReplayClicked);
        }

        public void ShowWinPopup(int pointsGained = 0)
        {
            winPanel.SetActive(true);
            winPanel.transform.DOScale(Vector3.one, animationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            currentActivePanel = winPanel;
            // Show Next button, hide Replay button
            nextButton.gameObject.SetActive(true);
            replayButton.gameObject.SetActive(false);

        }

        public void ShowLosePopup()
        {
            losePanel.SetActive(true);
            losePanel.transform.DOScale(Vector3.one, animationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            currentActivePanel = losePanel;
            // Show Replay button, hide Next button
            replayButton2.gameObject.SetActive(true);

        }

        private void OnNextClicked()
        {
            Hide();
            OnNextButtonClicked?.Invoke();
        }

        private void OnReplayClicked()
        {
            Hide();
            OnReplayButtonClicked?.Invoke();
        }

        private void Hide()
        {
            currentActivePanel.transform.DOScale(Vector3.zero, animationDuration * 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                currentActivePanel.SetActive(false);
            });
        }
    }
}
