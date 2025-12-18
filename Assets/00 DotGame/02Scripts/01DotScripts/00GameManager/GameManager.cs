
using UnityEngine;
using Inwave.DongA.DotPuzzle.LeveData;
using System.Linq; 
using System;
using DG.Tweening;
using Inwave.DongA.DotPuzzle.Data;
using DACN.Account;
using TMPro;

namespace Inwave.DongA.DotPuzzle.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [Header("Game status")]
        public bool isGameOver;
        public bool loseByBug = false;
        public bool isPaused = false;
        public bool canInteract = true;
        public bool hasInsects = false;

        [Header("Level Data")]
        public int currentLevel = 1;
        public Transform parentContainer;
        public LevelData levelData;
        public AllLevelDataSO allLevelDataSO; 
        public int currentStepIndex;
        public bool isLevelLoaded;

        public TextMeshProUGUI scoreText;
        
        public event Action OnLevelLoadedEvent;
        public event Action OnStartLoadLevelEvent;
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return; 
            }
            Instance = this;
            DOTween.SetTweensCapacity(2000, 200);
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
        }

        private void Start()
        {
            currentLevel = LocalDataManager.Instance.currentChild.dotLevel;
            scoreText.text = "Score: " + LocalDataManager.Instance.currentChild.score.ToString();
            LoadLevelData();
        }

        public void NextLevel()
        {
            currentLevel++;
            LocalDataManager.Instance.currentChild.dotLevel = currentLevel;
            LocalDataManager.Instance.currentChild.score += 10;
            scoreText.text = "Score: " + LocalDataManager.Instance.currentChild.score.ToString();
            LocalDataManager.Instance.Save();
            
            if(currentLevel > allLevelDataSO.allLevelData.Length) currentLevel = 1; 
            
            LoadLevelData();
        }

        public void ReplayLevel()
        {
            LoadLevelData();
        }
        public void LoadLevelData()
        {
            isGameOver = false;
            loseByBug = false;
            canInteract = true;
            currentStepIndex = 0;
            isLevelLoaded = false;
            hasInsects = false;
            
            if (levelData != null)
            {
                DOTween.KillAll();
                Destroy(levelData.gameObject);
            }
            OnStartLoadLevelEvent?.Invoke();

#if UNITY_EDITOR
            bool isTestMode = PlayerPrefs.GetInt("TEST", 0) ==1;
            if (isTestMode)
            {
                PlayerPrefs.DeleteKey("TEST");
                var levelText = PlayerPrefs.GetString("TEST_LEVEL", String.Empty);
                if (!string.IsNullOrEmpty(levelText))
                {
                    LoadLevelFromText(levelText);
                }
            }
            else
            {
                LoadLevel();
            }
#else
            LoadLevel();
#endif

        }

        private void LoadLevel()
        {
            var levelRecord = allLevelDataSO.allLevelData.FirstOrDefault(x => x.LevelId == currentLevel);

            if (levelRecord != null && levelRecord.LevelData != null)
            {
                string jsonContent = levelRecord.LevelData.text;

                LoadLevelFromText(jsonContent);
            }
            else
            {
                Debug.LogError($"Cannot find data for level {currentLevel} in AllLevelDataSO");
            }
        }

        private void LoadLevelFromText(string levelText)
        {
            SaveLoadData.Instance.LoadLevelFromContent(levelText, parentContainer, loadedData =>
            {
                if (loadedData == null)
                {
                    Debug.LogError("Load level failed");
                    return;
                }
                this.levelData = loadedData;
                isLevelLoaded = true;
                Debug.Log("Load level success: " + loadedData.levelId);

                OnLevelLoadedEvent?.Invoke();
            });
        }
    }
}
