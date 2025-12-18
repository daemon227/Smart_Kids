using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DACN.Account;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace DACN.Quiz
{
    public class QuizGameManager : MonoBehaviour
    {
        [SerializeField] private QuizPlayUIManager quizPlayUIManager;
        [SerializeField] private QuizResultPopup resultPopup;

        [Header("Settings")]
        [SerializeField] private int pointsPerCorrectAnswer = 10;
        [SerializeField] private float delayBeforeNextQuiz = 0.5f;

        public GameObject pausePanel;
        public Button settingButton;
        public Button closeButton;
        public Button homeButton;

        public TMPro.TextMeshProUGUI scoreText;

        private Quiz currentQuiz;
        private int currentQuizLevel;
        private List<Quiz> availableQuizzes;

        private void Start()
        {
            settingButton.onClick.AddListener(OnSettingButtonClicked);
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            homeButton.onClick.AddListener(OnHomeButtonClicked);
            // Initialize with current child's quiz level
            var currentChild = LocalDataManager.Instance.currentChild;
            if (currentChild != null)
            {
                scoreText.text = "Score: " + currentChild.score.ToString();
                currentQuizLevel = currentChild.quizLevel;
                if (currentQuizLevel < 1)
                {
                    currentQuizLevel = 1;
                }
                Debug.Log($"[QuizGameManager] Child: {currentChild.name}, QuizLevel: {currentQuizLevel}");
            }
            else
            {
                Debug.LogError("[QuizGameManager] CurrentChild is null! Make sure to set it before loading this scene.");
                return;
            }

            // Setup UI callbacks
            if (quizPlayUIManager != null)
            {
                quizPlayUIManager.OnAnswerSelected += HandleAnswerSelected;
            }

            if (resultPopup != null)
            {
                resultPopup.OnNextButtonClicked += HandleNextQuiz;
                resultPopup.OnReplayButtonClicked += HandleReplay;
            }

            // Load and show first quiz
            LoadNextQuiz();
        }

        private void OnDestroy()
        {
            if (quizPlayUIManager != null)
            {
                quizPlayUIManager.OnAnswerSelected -= HandleAnswerSelected;
            }

            if (resultPopup != null)
            {
                resultPopup.OnNextButtonClicked -= HandleNextQuiz;
                resultPopup.OnReplayButtonClicked -= HandleReplay;
            }
        }

        private void LoadNextQuiz()
        {
            //Debug.Log("[QuizGameManager] LoadNextQuiz called");

            // Get current parent username
            var currentUser = LocalDataManager.Instance.currentUser;
            if (currentUser == null)
            {
                Debug.LogError("[QuizGameManager] No parent account logged in");
                return;
            }

            // Get quizzes for current level and current parent creator
            Debug.Log($"[QuizGameManager] Loading quizzes for level {currentQuizLevel} from creator {currentUser.username}");
            availableQuizzes = QuizManager.Instance.GetQuizzesByLevelAndCreator(currentQuizLevel, currentUser.username);

            if (availableQuizzes == null || availableQuizzes.Count == 0)
            {
                Debug.LogWarning($"[QuizGameManager] No quizzes found for level {currentQuizLevel}. Resetting to level 0.");
                ResetQuizLevel();
                return;
            }

            Debug.Log($"[QuizGameManager] Found {availableQuizzes.Count} quizzes for level {currentQuizLevel}");

            // Select random quiz from available quizzes
            currentQuiz = availableQuizzes[Random.Range(0, availableQuizzes.Count)];
            Debug.Log($"[QuizGameManager] Selected quiz: {currentQuiz.question}");

            // Display quiz
            if (quizPlayUIManager != null)
            {
                quizPlayUIManager.ResetUI();
                quizPlayUIManager.ShowQuiz(currentQuiz);
            }
            else
            {
                Debug.LogError("[QuizGameManager] QuizPlayUIManager is not assigned!");
            }
        }

        private void HandleAnswerSelected(bool isCorrect)
        {
            Debug.Log($"[QuizGameManager] Answer selected - Correct: {isCorrect}");
            if (isCorrect)
            {
                HandleCorrectAnswer();
            }
            else
            {
                HandleIncorrectAnswer();
            }
        }

        private void HandleCorrectAnswer()
        {
            // Add points to child
            AddPointsToChild(pointsPerCorrectAnswer);

            // Show win popup
            if (resultPopup != null)
            {
                resultPopup.ShowWinPopup(pointsPerCorrectAnswer);
            }
        }

        private void HandleIncorrectAnswer()
        {
            // Show lose popup
            if (resultPopup != null)
            {
                resultPopup.ShowLosePopup();
            }
        }

        private void HandleNextQuiz()
        {
            // Increment quiz level
            IncrementQuizLevel();

            // Load next quiz
            StartCoroutine(LoadNextQuizDelayed());
        }

        private void HandleReplay()
        {
            // Load current quiz again (same level, same quiz)
            StartCoroutine(LoadNextQuizDelayed());
        }

        private IEnumerator LoadNextQuizDelayed()
        {
            yield return new WaitForSeconds(delayBeforeNextQuiz);
            LoadNextQuiz();
        }

        private void AddPointsToChild(int points)
        {
            var currentChild = LocalDataManager.Instance.currentChild;
            var currentUser = LocalDataManager.Instance.currentUser;
            if (currentUser == null || currentChild == null)
            {
                Debug.LogError("[QuizGameManager] Cannot add points: currentUser or currentChild is null");
                return;
            }

            currentChild.score += points;
            scoreText.text = "Score: " + currentChild.score.ToString();
            LocalDataManager.Instance.Save();
            Debug.Log($"[QuizGameManager] Added {points} points to child {currentChild.name}. Total score: {currentChild.score}");
        }

        private void IncrementQuizLevel()
        {
            var currentChild = LocalDataManager.Instance.currentChild;
            if (currentChild != null)
            {
                currentChild.quizLevel++;
                currentQuizLevel = currentChild.quizLevel;
                LocalDataManager.Instance.Save();
                Debug.Log($"[QuizGameManager] Incremented quiz level to {currentChild.quizLevel}");
            }
        }

        private void ResetQuizLevel()
        {
            var currentChild = LocalDataManager.Instance.currentChild;
            if (currentChild != null)
            {
                currentChild.quizLevel = 0;
                currentQuizLevel = 0;
                LocalDataManager.Instance.Save();
                Debug.Log("[QuizGameManager] Reset quiz level to 0");
                // Reload quizzes
                StartCoroutine(LoadNextQuizDelayed());
            }
        }

        void OnSettingButtonClicked()
    {
        pausePanel.SetActive(true);    
        pausePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    void OnCloseButtonClicked()
    {
        pausePanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            pausePanel.SetActive(false);
        });
    }

    void OnHomeButtonClicked()
    {
        SceneManager.LoadScene("ChildScene");
    }
    }
}