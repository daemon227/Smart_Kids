using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor.SearchService;
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
        //public GameObject leaderboardPanel;
        public GameObject quizManagerPanel;
    
        void Start()
        {
            parentNameText.text = "" + LocalDataManager.Instance.currentUser.name;
            openChildListBtn.onClick.AddListener(OpenChildListUI);
            //openLeaderboardBtn.onClick.AddListener(OpenLeaderboardUI);
            quizManagerBtn.onClick.AddListener(OpenQuizManagerUI);

            changeModeBtn.onClick.AddListener(ChangeMode);
        }

        private void OpenQuizManagerUI()
        {
            quizManagerPanel.SetActive(true);
            quizManagerPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
            mainPanel.gameObject.SetActive(false);
        }

        private void OpenLeaderboardUI()
        {
            throw new NotImplementedException();
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

