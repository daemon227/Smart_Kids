using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DACN.Quiz
{
    public class QuizItemUI : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text levelText;
    public Button editBtn;
    public Button deleteBtn;

    public System.Action OnEditClick;
    public System.Action OnDeleteClick;

    void Start()
    {
        editBtn.onClick.AddListener(() => OnEditClick?.Invoke());
        deleteBtn.onClick.AddListener(() => OnDeleteClick?.Invoke());
    }

    public void SetData(string question, string level)
    {
        questionText.text = question;
        levelText.text = $"Level: {level}";
    }
}
}

