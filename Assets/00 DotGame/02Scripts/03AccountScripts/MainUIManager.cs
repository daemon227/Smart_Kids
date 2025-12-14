using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
    
        void Start()
        {
            parentNameText.text = "" + LocalDataManager.Instance.currentUser.name;
            openChildListBtn.onClick.AddListener(OpenChildListUI);
        }

        void OpenChildListUI()
        {
            childListPanel.SetActive(true);
            childListPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
            mainPanel.gameObject.SetActive(false);
        }

    }
}

