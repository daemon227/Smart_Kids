using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DACN.Quiz
{
    [Serializable]
    public class Quiz
    {
        public string id;
        public string creatorUsername; // Username of parent who created this quiz
        public int levelIndex;
        public string question;
        public List<string> wrongAnswers; // 3 wrong answers
        public string correctAnswer;
        public DateTime createdAt;
        public DateTime updatedAt;

        public Quiz()
        {
            this.id = System.Guid.NewGuid().ToString();
            this.creatorUsername = "";
            this.levelIndex = 1;
            this.question = "";
            this.wrongAnswers = new List<string> { "", "", "" };
            this.correctAnswer = "";
            this.createdAt = DateTime.Now;
            this.updatedAt = DateTime.Now;
        }

        public Quiz(string creatorUsername, int levelIndex, string question, List<string> wrongAnswers, string correctAnswer)
        {
            this.id = System.Guid.NewGuid().ToString();
            this.creatorUsername = creatorUsername;
            this.levelIndex = levelIndex;
            this.question = question;
            this.wrongAnswers = wrongAnswers ?? new List<string> { "", "", "" };
            this.correctAnswer = correctAnswer;
            this.createdAt = DateTime.Now;
            this.updatedAt = DateTime.Now;
        }
    }

    [Serializable]
    public class QuizDatabase
    {
        public List<Quiz> quizzes = new List<Quiz>();
    }
}


