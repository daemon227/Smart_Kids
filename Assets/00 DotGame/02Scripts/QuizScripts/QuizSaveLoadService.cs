using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DACN.Quiz
{
    public class QuizSaveLoadService : MonoBehaviour
    {
        private static string filePath;

        void Awake()
        {
            filePath = Path.Combine(Application.persistentDataPath, "quiz_data.json");
        }

        // ===== LOAD =====
        public static QuizDatabase LoadQuizzes()
        {
            if (!File.Exists(filePath))
            {
                return new QuizDatabase();
            }

            string json = File.ReadAllText(filePath);
            QuizDatabase database = JsonUtility.FromJson<QuizDatabase>(json);
            return database ?? new QuizDatabase();
        }

        // ===== SAVE =====
        public static void SaveQuizzes(QuizDatabase database)
        {
            string json = JsonUtility.ToJson(database, true);
            File.WriteAllText(filePath, json);
        }

        // ===== GET FILE PATH =====
        public static string GetFilePath()
        {
            return filePath;
        }
    }

}
