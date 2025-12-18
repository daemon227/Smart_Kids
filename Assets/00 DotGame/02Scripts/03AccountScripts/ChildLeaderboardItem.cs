using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DACN.Account
{
    public class ChildLeaderboardItem : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_Text rankText;
        public TMP_Text nameText;
        public TMP_Text scoreText;
        public Button itemButton;

        private ChildAccount childData;
        private Action<ChildAccount> onClickCallback;

        private void Start()
        {
            itemButton.onClick.AddListener(OnItemClicked);
        }

        public void SetData(int rank, ChildAccount child, Action<ChildAccount> onClickCallback)
        {
            this.childData = child;
            this.onClickCallback = onClickCallback;

            rankText.text = rank.ToString();
            nameText.text = child.name;
            scoreText.text = child.score.ToString();
        }

        private void OnItemClicked()
        {
            onClickCallback?.Invoke(childData);
        }
    }
}
