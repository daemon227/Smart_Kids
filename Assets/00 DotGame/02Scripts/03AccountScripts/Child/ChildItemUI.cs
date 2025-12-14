using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DACN.Account
{   
    public class ChildItemUI : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text scoreText;
        public TMP_Text limitedTimePerDay;
        public Button editBtn;
        public Button editTimeBtn;

        public System.Action OnEditClick;
        public System.Action OnEditTimeClick;

        void Start()
        {
            editBtn.onClick.AddListener(() => OnEditClick?.Invoke());
            editTimeBtn.onClick.AddListener(() => OnEditTimeClick?.Invoke());
        }

        public void SetData(string name, int age,int score, float limitedTimePerDay)
        {
            nameText.text = name + "_Age: "+ age.ToString();
            scoreText.text = "Score: " +score.ToString();
            this.limitedTimePerDay.text =  $"Limited Time Per Day: {limitedTimePerDay} mins";
        }
    }
}
