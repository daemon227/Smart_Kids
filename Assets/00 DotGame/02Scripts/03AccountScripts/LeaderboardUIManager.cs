using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace DACN.Account
{
    public class LeaderboardUIManager : MonoBehaviour
    {
        [Header("Leaderboard Panel")]
        public GameObject mainPanel;
        public GameObject leaderboardPanel;
        public Transform childListContent;
        public Button closeLeaderboardBtn;
        public GameObject childLeaderboardItemPrefab;

        [Header("Score Breakdown Panel")]
        public GameObject scoreBreakdownPanel;
        public TMP_Text selectedChildNameText;
        public Image dotGamePieImage;
        public Image memoryGamePieImage;
        public Image quizGamePieImage;
        public TMP_Text dotGameScoreText;
        public TMP_Text memoryGameScoreText;
        public TMP_Text quizGameScoreText;
        public Button closeBreakdownBtn;

        private const int POINTS_PER_LEVEL = 10;
        private List<ChildLeaderboardItem> leaderboardItems = new List<ChildLeaderboardItem>();

        private void Start()
        {
            closeLeaderboardBtn.onClick.AddListener(CloseLeaderboard);
            closeBreakdownBtn.onClick.AddListener(CloseBreakdown);
        }

        public void OpenLeaderboard()
        {
            if(leaderboardPanel == null) {
                Debug.LogError("[LeaderboardUIManager] Leaderboard Panel is not assigned.");
                return;
            }
            leaderboardPanel.transform.gameObject.SetActive(true);
            leaderboardPanel.transform.localScale = Vector3.zero;
            leaderboardPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

            LoadLeaderboardData();
        }

        private void LoadLeaderboardData()
        {
            // Clear previous items
            foreach (Transform child in childListContent)
            {
                Destroy(child.gameObject);
            }
            leaderboardItems.Clear();

            // Get all child accounts from current parent
            var childAccounts = LocalDataManager.Instance.currentUser.childAccounts;

            // Sort by score descending
            var sortedChildren = childAccounts.OrderByDescending(c => c.score).ToList();

            // Create leaderboard items
            int rank = 1;
            foreach (var child in sortedChildren)
            {
                var itemGO = Instantiate(childLeaderboardItemPrefab.gameObject, childListContent);
                var itemComponent = itemGO.GetComponent<ChildLeaderboardItem>();
                itemComponent.SetData(rank, child, OnChildItemClicked);
                leaderboardItems.Add(itemComponent);
                rank++;
            }
        }

        private void OnChildItemClicked(ChildAccount child)
        {
            ShowScoreBreakdown(child);
        }

        private void ShowScoreBreakdown(ChildAccount child)
        {
            selectedChildNameText.text = child.name;

            // Calculate score breakdown (Level 1 = 0 points, Level 2 = 10 points, etc.)
            int dotGameScore = (child.dotLevel - 1) * POINTS_PER_LEVEL;
            int memoryGameScore = (child.memoryLevel - 1) * POINTS_PER_LEVEL;
            int quizGameScore = (child.quizLevel - 1) * POINTS_PER_LEVEL;

            // Calculate effective levels (Level 1 = 0, Level 2 = 1, etc.)
            int totalEffectiveLevels = (child.dotLevel - 1) + (child.memoryLevel - 1) + (child.quizLevel - 1);

            // Calculate percentages based on effective levels
            float dotPercentage = totalEffectiveLevels > 0 ? (float)(child.dotLevel - 1) / totalEffectiveLevels * 100f : 0f;
            float memoryPercentage = totalEffectiveLevels > 0 ? (float)(child.memoryLevel - 1) / totalEffectiveLevels * 100f : 0f;
            float quizPercentage = totalEffectiveLevels > 0 ? (float)(child.quizLevel - 1) / totalEffectiveLevels * 100f : 0f;

            // Define colors for each game
            Color dotGameColor = new Color(1f, 0.5f, 0f); // Orange
            Color memoryGameColor = new Color(0f, 0.7f, 1f); // Light Blue
            Color quizGameColor = new Color(1f, 0f, 0.7f); // Pink

            // Update text displays with colors
            dotGameScoreText.text = $"Dot Game: {dotGameScore} points";
            dotGameScoreText.color = dotGameColor;

            memoryGameScoreText.text = $"Memory Game: {memoryGameScore} points";
            memoryGameScoreText.color = memoryGameColor;

            quizGameScoreText.text = $"Quiz Game: {quizGameScore} points";
            quizGameScoreText.color = quizGameColor;

            // Update pie chart
            UpdatePieChart(dotPercentage, memoryPercentage, quizPercentage);

            // Show breakdown panel
            scoreBreakdownPanel.SetActive(true);
            scoreBreakdownPanel.transform.localScale = Vector3.zero;
            scoreBreakdownPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        private void UpdatePieChart(float dotPercentage, float memoryPercentage, float quizPercentage)
        {
            // Create game data with percentages and colors
            var gameData = new List<GamePieData>
            {
                new GamePieData { image = dotGamePieImage, percentage = dotPercentage, color = new Color(1f, 0.5f, 0f) }, // Orange
                new GamePieData { image = memoryGamePieImage, percentage = memoryPercentage, color = new Color(0f, 0.7f, 1f) }, // Light Blue
                new GamePieData { image = quizGamePieImage, percentage = quizPercentage, color = new Color(1f, 0f, 0.7f) } // Pink
            };

            // Sort by percentage descending (largest first)
            gameData = gameData.OrderByDescending(g => g.percentage).ToList();

            // Set fill amounts cumulatively (from largest to smallest)
            float cumulativeFill = 0f;
            for (int i = 0; i < gameData.Count; i++)
            {
                if (gameData[i].image != null)
                {
                    // Convert percentage (0-100) to fill (0-1)
                    float currentFill = gameData[i].percentage / 100f;
                    cumulativeFill += currentFill;

                    // Set fill amount and color
                    gameData[i].image.fillAmount = cumulativeFill;
                    gameData[i].image.color = gameData[i].color;

                    // Set sibling index: largest on top, smallest on bottom
                    // This ensures smaller segments are not hidden
                    gameData[i].image.transform.SetSiblingIndex(gameData.Count - 1 - i);

                    Debug.Log($"[LeaderboardUIManager] Game {i}: Percentage={gameData[i].percentage:F1}%, FillAmount={cumulativeFill:F2}, Color={gameData[i].color}");
                }
            }
        }

        private class GamePieData
        {
            public Image image;
            public float percentage;
            public Color color;
        }

        private void CloseLeaderboard()
        {
            mainPanel.SetActive(true);
            leaderboardPanel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                leaderboardPanel.SetActive(false);
            });
        }

        private void CloseBreakdown()
        {
            scoreBreakdownPanel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                scoreBreakdownPanel.SetActive(false);      
            });
        }
    }

    public class Prefab : MonoBehaviour
    {
        // This class is just a placeholder for the prefab reference
    }
}
