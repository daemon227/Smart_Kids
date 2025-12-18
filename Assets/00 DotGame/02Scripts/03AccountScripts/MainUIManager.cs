using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DACN.Account
{
    public class MainUIManager : MonoBehaviour
    {
        [Header("Main UI Elements")]
        public GameObject mainPanel;
        public Button openChildListBtn;
        public Button openLeaderboardBtn;
        public Button quizManagerBtn;
        public Button changeModeBtn;
        public TMP_Text parentNameText;

        [Header("Panel")]
        public GameObject childListPanel;
        public GameObject leaderboardPanel;
        public GameObject quizManagerPanel;

        private LeaderboardUIManager leaderboardUIManager;
    
        void Start()
        {
            parentNameText.text = "" + LocalDataManager.Instance.currentUser.name;
            openChildListBtn.onClick.AddListener(OpenChildListUI);
            openLeaderboardBtn.onClick.AddListener(OpenLeaderboardUI);
            quizManagerBtn.onClick.AddListener(OpenQuizManagerUI);
            changeModeBtn.onClick.AddListener(ChangeMode);

            // Get or create LeaderboardUIManager
            leaderboardUIManager = GetComponent<LeaderboardUIManager>();
        }

        private void OpenQuizManagerUI()
        {
            quizManagerPanel.SetActive(true);
            quizManagerPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
            mainPanel.gameObject.SetActive(false);
        }

        private void OpenLeaderboardUI()
        {
            mainPanel.gameObject.SetActive(false);
            leaderboardUIManager.OpenLeaderboard();
        }

        void OpenChildListUI()
        {
            childListPanel.SetActive(true);
            childListPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
            mainPanel.gameObject.SetActive(false);
        }

        void ChangeMode()
        {
            SceneManager.LoadScene("LoginScene");
        }
    }
}

