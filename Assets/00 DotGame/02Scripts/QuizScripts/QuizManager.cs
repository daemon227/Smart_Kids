using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DACN.Quiz
{
    public class QuizManager : MonoBehaviour
    {
        public static QuizManager Instance { get; private set; }
        private QuizDatabase quizDatabase;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize - load from file
            quizDatabase = QuizSaveLoadService.LoadQuizzes();
        }

        // ===== CREATE QUIZ =====
        public bool CreateQuiz(string username,int levelIndex, string question, List<string> wrongAnswers, string correctAnswer)
        {
            if (string.IsNullOrWhiteSpace(question) || 
                wrongAnswers == null || wrongAnswers.Count != 3 || 
                string.IsNullOrWhiteSpace(correctAnswer))
            {
                Debug.LogError("Invalid quiz data");
                return false;
            }

            Quiz newQuiz = new Quiz(username,levelIndex,question, wrongAnswers, correctAnswer);
            quizDatabase.quizzes.Add(newQuiz);
            SaveQuizzes();
            Debug.Log($"Quiz created with ID: {newQuiz.id}");
            return true;
        }

        // ===== GET QUIZ BY ID =====
        public Quiz GetQuizById(string quizId)
        {
            return quizDatabase.quizzes.FirstOrDefault(q => q.id == quizId);
        }

        // ===== GET QUIZZES BY LEVEL =====
        public List<Quiz> GetQuizzesByLevel(int levelIndex)
        {
            return quizDatabase.quizzes
                .Where(q => q.levelIndex == levelIndex)
                .ToList();
        }

        // ===== GET QUIZZES BY LEVEL AND CREATOR =====
        public List<Quiz> GetQuizzesByLevelAndCreator(int levelIndex, string creatorUsername)
        {
            return quizDatabase.quizzes
                .Where(q => q.levelIndex == levelIndex && q.creatorUsername == creatorUsername)
                .ToList();
        }

        // ===== GET ALL QUIZZES =====
        public List<Quiz> GetAllQuizzes()
        {
            return new List<Quiz>(quizDatabase.quizzes);
        }

        // ===== UPDATE QUIZ =====
        public bool UpdateQuiz(string quizId, int levelIndex, string question, List<string> wrongAnswers, string correctAnswer)
        {
            Quiz quiz = GetQuizById(quizId);
            if (quiz == null)
            {
                Debug.LogError($"Quiz with ID {quizId} not found");
                return false;
            }

            if (string.IsNullOrWhiteSpace(question) || 
                wrongAnswers == null || wrongAnswers.Count != 3 || 
                string.IsNullOrWhiteSpace(correctAnswer))
            {
                Debug.LogError("Invalid quiz data");
                return false;
            }

            quiz.levelIndex = levelIndex;
            quiz.question = question;
            quiz.wrongAnswers = wrongAnswers;
            quiz.correctAnswer = correctAnswer;

            SaveQuizzes();
            Debug.Log($"Quiz {quizId} updated");
            return true;
        }

        // ===== DELETE QUIZ =====
        public bool DeleteQuiz(string quizId)
        {
            Quiz quiz = GetQuizById(quizId);
            if (quiz == null)
            {
                Debug.LogError($"Quiz with ID {quizId} not found");
                return false;
            }

            quizDatabase.quizzes.Remove(quiz);
            SaveQuizzes();
            Debug.Log($"Quiz {quizId} deleted");
            return true;
        }

        // ===== DELETE ALL QUIZZES BY LEVEL =====
        public int DeleteQuizzesByLevel(int levelIndex)
        {
            int count = quizDatabase.quizzes.RemoveAll(q => q.levelIndex == levelIndex);
            SaveQuizzes();
            Debug.Log($"Deleted {count} quizzes from level {levelIndex}");
            return count;
        }

        // ===== COUNT QUIZZES =====
        public int GetTotalQuizCount()
        {
            return quizDatabase.quizzes.Count;
        }

        public int GetQuizCountByLevel(int levelIndex)
        {
            return quizDatabase.quizzes.Count(q => q.levelIndex == levelIndex);
        }

        // ===== SAVE TO FILE =====
        private void SaveQuizzes()
        {
            QuizSaveLoadService.SaveQuizzes(quizDatabase);
        }

        // ===== RELOAD FROM FILE =====
        public void ReloadQuizzes()
        {
            quizDatabase = QuizSaveLoadService.LoadQuizzes();
            Debug.Log("Quizzes reloaded from file");
        }

        // ===== CLEAR ALL =====
        public void ClearAllQuizzes()
        {
            quizDatabase.quizzes.Clear();
            SaveQuizzes();
            Debug.Log("All quizzes cleared");
        }

        // ===== GET DATABASE INFO =====
        public void PrintQuizzesInfo()
        {
            Debug.Log($"Total quizzes: {quizDatabase.quizzes.Count}");
            foreach (var quiz in quizDatabase.quizzes)
            {
                Debug.Log($"ID: {quiz.id}, Level: {quiz.levelIndex}, Question: {quiz.question}");
            }
        }
    }
}

