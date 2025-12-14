using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.SocialPlatforms;
using DACN.Account;

namespace DACN.Quiz
{
    public class QuizUIManager : MonoBehaviour
    {
        [Header("Main Panels")]
        public GameObject mainPanel;
        public GameObject quizListPanel;
        public GameObject createQuizPanel;
        public GameObject editQuizPanel;
        public GameObject deleteConfirmPanel;

        [Header("Quiz List UI")]
        public Transform quizListContent;
        public GameObject quizItemPrefab;
        public Button addNewQuizBtn;
        public Button backBtn;

        [Header("Create Quiz UI")]
        public TMP_InputField createLevelInput;
        public TMP_InputField createQuestionInput;
        public TMP_InputField createWrongAnswer1Input;
        public TMP_InputField createWrongAnswer2Input;
        public TMP_InputField createWrongAnswer3Input;
        public TMP_InputField createCorrectAnswerInput;
        public Button createConfirmBtn;
        public Button createCancelBtn;
        public TMP_Text createMsg;

        [Header("Edit Quiz UI")]
        public TMP_InputField editLevelInput;
        public TMP_InputField editQuestionInput;
        public TMP_InputField editWrongAnswer1Input;
        public TMP_InputField editWrongAnswer2Input;
        public TMP_InputField editWrongAnswer3Input;
        public TMP_InputField editCorrectAnswerInput;
        public Button editConfirmBtn;
        public Button editCancelBtn;
        public TMP_Text editMsg;

        [Header("Delete Confirm UI")]
        public TMP_Text deleteConfirmMsg;
        public Button deleteConfirmYesBtn;
        public Button deleteConfirmNoBtn;

        private string selectedQuizId;

        void Start()
        {
            SetupButtons();
            RefreshQuizList();
        }

        // ===== SETUP BUTTONS =====
        void SetupButtons()
        {
            backBtn.onClick.AddListener(OpenMainPanel);
            addNewQuizBtn.onClick.AddListener(ShowCreatePanel);
            createConfirmBtn.onClick.AddListener(OnCreateQuiz);
            createCancelBtn.onClick.AddListener(HideCreatePanel);
            editConfirmBtn.onClick.AddListener(OnEditQuiz);
            editCancelBtn.onClick.AddListener(HideEditPanel);
            deleteConfirmYesBtn.onClick.AddListener(OnConfirmDelete);
            deleteConfirmNoBtn.onClick.AddListener(HideDeleteConfirmPanel);
        }

        private void OpenMainPanel()
        {
            mainPanel.SetActive(true);
            mainPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
            quizListPanel.SetActive(false);
        }

        // ===== REFRESH QUIZ LIST =====
        public void RefreshQuizList()
        {
            List<Quiz> quizzes = QuizManager.Instance.GetAllQuizzes();

            // Clear old items
            foreach (Transform child in quizListContent)
            {
                Destroy(child.gameObject);
            }

            // Add new items
            foreach (Quiz quiz in quizzes)
            {
                if(quiz.creatorUsername != LocalDataManager.Instance.currentUser.username)
                    continue;
                GameObject item = Instantiate(quizItemPrefab, quizListContent);
                QuizItemUI itemUI = item.GetComponent<QuizItemUI>();
                itemUI.SetData(quiz.question, quiz.levelIndex.ToString());
                itemUI.OnEditClick = () => ShowEditPanel(quiz.id);
                itemUI.OnDeleteClick = () => ShowDeleteConfirmPanel(quiz.id, quiz.levelIndex.ToString());
            }
        }

        // ===== CREATE QUIZ =====
        void ShowCreatePanel()
        {
            createQuizPanel.SetActive(true);
            createQuizPanel.transform.localScale = Vector3.zero;
            createQuizPanel.transform.DOScale(Vector3.one, 0.5f);
            quizListPanel.SetActive(false);
            ClearCreateFields();
        }

        void HideCreatePanel()
        {
            createQuizPanel.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
            {
                createQuizPanel.SetActive(false);
            });
            quizListPanel.SetActive(true);
            ClearCreateFields();
        }

        void ClearCreateFields()
        {
            createLevelInput.text = "";
            createQuestionInput.text = "";
            createWrongAnswer1Input.text = "";
            createWrongAnswer2Input.text = "";
            createWrongAnswer3Input.text = "";
            createCorrectAnswerInput.text = "";
            createMsg.text = "";
        }

        void OnCreateQuiz()
        {
            if (!ValidateCreateInput(out int level, out string question, out List<string> wrongAnswers, out string correctAnswer))
                return;
            
            var username = LocalDataManager.Instance.currentUser.username;
            if (string.IsNullOrEmpty(username))
            {
                createMsg.text = "User not logged in";
                return;
            }
            bool success = QuizManager.Instance.CreateQuiz(username,level, question, wrongAnswers, correctAnswer);

            if (success)
            {
                createMsg.text = "Quiz created successfully";
                StartCoroutine(HideCreatePanelDelayed());
                RefreshQuizList();
            }
            else
            {
                createMsg.text = "Failed to create quiz";
            }
        }

        bool ValidateCreateInput(out int level, out string question, out List<string> wrongAnswers, out string correctAnswer)
        {
            level = 0;
            question = "";
            wrongAnswers = new List<string>();
            correctAnswer = "";

            if (string.IsNullOrWhiteSpace(createLevelInput.text))
            {
                createMsg.text = "Level cannot be empty";
                return false;
            }

            if (!int.TryParse(createLevelInput.text, out level) || level < 1)
            {
                createMsg.text = "Invalid level number";
                return false;
            }

            question = createQuestionInput.text.Trim();
            if (string.IsNullOrWhiteSpace(question))
            {
                createMsg.text = "Question cannot be empty";
                return false;
            }

            string ans1 = createWrongAnswer1Input.text.Trim();
            string ans2 = createWrongAnswer2Input.text.Trim();
            string ans3 = createWrongAnswer3Input.text.Trim();

            if (string.IsNullOrWhiteSpace(ans1) || string.IsNullOrWhiteSpace(ans2) || string.IsNullOrWhiteSpace(ans3))
            {
                createMsg.text = "All wrong answers are required";
                return false;
            }

            correctAnswer = createCorrectAnswerInput.text.Trim();
            if (string.IsNullOrWhiteSpace(correctAnswer))
            {
                createMsg.text = "Correct answer cannot be empty";
                return false;
            }

            wrongAnswers = new List<string> { ans1, ans2, ans3 };
            return true;
        }

        IEnumerator HideCreatePanelDelayed()
        {
            yield return new WaitForSeconds(1f);
            HideCreatePanel();
        }

        // ===== EDIT QUIZ =====
        void ShowEditPanel(string quizId)
        {
            selectedQuizId = quizId;
            Quiz quiz = QuizManager.Instance.GetQuizById(quizId);

            if (quiz == null)
            {
                Debug.LogError("Quiz not found");
                return;
            }

            editLevelInput.text = quiz.levelIndex.ToString();
            editQuestionInput.text = quiz.question;
            editWrongAnswer1Input.text = quiz.wrongAnswers.Count > 0 ? quiz.wrongAnswers[0] : "";
            editWrongAnswer2Input.text = quiz.wrongAnswers.Count > 1 ? quiz.wrongAnswers[1] : "";
            editWrongAnswer3Input.text = quiz.wrongAnswers.Count > 2 ? quiz.wrongAnswers[2] : "";
            editCorrectAnswerInput.text = quiz.correctAnswer;
            editMsg.text = "";

            editQuizPanel.SetActive(true);
            editQuizPanel.transform.localScale = Vector3.zero;
            editQuizPanel.transform.DOScale(Vector3.one, 0.5f);
            quizListPanel.SetActive(false);
        }

        void HideEditPanel()
        {
            editQuizPanel.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
            {
                editQuizPanel.SetActive(false);
            });
            quizListPanel.SetActive(true);
            selectedQuizId = "";
        }

        void OnEditQuiz()
        {
            if (!ValidateEditInput(out int level, out string question, out List<string> wrongAnswers, out string correctAnswer))
                return;

            bool success = QuizManager.Instance.UpdateQuiz(selectedQuizId, level, question, wrongAnswers, correctAnswer);

            if (success)
            {
                editMsg.text = "Quiz updated successfully";
                StartCoroutine(HideEditPanelDelayed());
                RefreshQuizList();
            }
            else
            {
                editMsg.text = "Failed to update quiz";
            }
        }

        bool ValidateEditInput(out int level, out string question, out List<string> wrongAnswers, out string correctAnswer)
        {
            level = 0;
            question = "";
            wrongAnswers = new List<string>();
            correctAnswer = "";

            if (string.IsNullOrWhiteSpace(editLevelInput.text))
            {
                editMsg.text = "Level cannot be empty";
                return false;
            }

            if (!int.TryParse(editLevelInput.text, out level) || level < 1)
            {
                editMsg.text = "Invalid level number";
                return false;
            }

            question = editQuestionInput.text.Trim();
            if (string.IsNullOrWhiteSpace(question))
            {
                editMsg.text = "Question cannot be empty";
                return false;
            }

            string ans1 = editWrongAnswer1Input.text.Trim();
            string ans2 = editWrongAnswer2Input.text.Trim();
            string ans3 = editWrongAnswer3Input.text.Trim();

            if (string.IsNullOrWhiteSpace(ans1) || string.IsNullOrWhiteSpace(ans2) || string.IsNullOrWhiteSpace(ans3))
            {
                editMsg.text = "All wrong answers are required";
                return false;
            }

            correctAnswer = editCorrectAnswerInput.text.Trim();
            if (string.IsNullOrWhiteSpace(correctAnswer))
            {
                editMsg.text = "Correct answer cannot be empty";
                return false;
            }

            wrongAnswers = new List<string> { ans1, ans2, ans3 };
            return true;
        }

        IEnumerator HideEditPanelDelayed()
        {
            yield return new WaitForSeconds(1f);
            HideEditPanel();
        }

        // ===== DELETE QUIZ =====
        void ShowDeleteConfirmPanel(string quizId, string quizLevel)
        {
            selectedQuizId = quizId;
            deleteConfirmMsg.text = $"Are you sure you want to delete quiz level {quizLevel}?";
            deleteConfirmPanel.SetActive(true);
            deleteConfirmPanel.transform.localScale = Vector3.zero;
            deleteConfirmPanel.transform.DOScale(Vector3.one, 0.5f);
            quizListPanel.SetActive(false);
        }

        void HideDeleteConfirmPanel()
        {
            deleteConfirmPanel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                deleteConfirmPanel.SetActive(false);
            });
            quizListPanel.SetActive(true);
            selectedQuizId = "";
        }

        void OnConfirmDelete()
        {
            bool success = QuizManager.Instance.DeleteQuiz(selectedQuizId);

            if (success)
            {
                HideDeleteConfirmPanel();
                RefreshQuizList();
            }
            else
            {
                deleteConfirmMsg.text = "Failed to delete quiz";
            }
        }
    }
}

