using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace DACN.Memories
{
    public class MemoriesResultPopup : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject winPanel;
        public GameObject losePanel;

        [SerializeField] private Button nextLevelButton;

        [Header("Settings")]
        [SerializeField] private float animationDuration = 0.5f;

        public System.Action OnNextLevelButtonClicked;

        private GameObject currentActivePanel;

        private void Start()
        {
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            }
        }

        public void ShowWinPopup(int currentLevel)
        {
            winPanel.SetActive(true);
            winPanel.transform.DOScale(Vector3.one, animationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            currentActivePanel = winPanel;
            losePanel.SetActive(false);
        }

        public void ShowLosePopup()
        {
            losePanel.SetActive(true);
            losePanel.transform.DOScale(Vector3.one, animationDuration).From(Vector3.zero).SetEase(Ease.OutBack);
            currentActivePanel = losePanel;
            winPanel.SetActive(false);
        }
        private void OnNextLevelClicked()
        {
            Hide();
            OnNextLevelButtonClicked?.Invoke();
        }

        private void Hide()
        {
            if (currentActivePanel == null) return;
            currentActivePanel.transform.DOScale(Vector3.zero, animationDuration * 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                currentActivePanel.SetActive(false);
            });
        }
    }
}
