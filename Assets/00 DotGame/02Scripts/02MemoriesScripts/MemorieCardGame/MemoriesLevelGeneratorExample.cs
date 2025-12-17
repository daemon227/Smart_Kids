using System.Collections.Generic;
using UnityEngine;

namespace DACN.Memories
{
    /// <summary>
    /// Example usage of MemoriesLevelGenerator
    /// Attach this script to test and see how levels are generated
    /// </summary>
    public class MemoriesLevelGeneratorExample : MonoBehaviour
    {
        private MemoriesLevelGenerator levelGenerator;

        private void Start()
        {
            // Get or create level generator
            levelGenerator = GetComponent<MemoriesLevelGenerator>();
            if (levelGenerator == null)
            {
                levelGenerator = gameObject.AddComponent<MemoriesLevelGenerator>();
            }

            // Example 1: Print all levels
            Debug.Log("\n=== Example 1: Print All Levels ===");
            levelGenerator.PrintAllLevels();

            // Example 2: Get tier card counts
            Debug.Log("\n=== Example 2: Tier Card Counts ===");
            int[] tierCardCounts = MemoriesLevelGenerator.GetTierCardCounts();
            Debug.Log($"Tier card counts: {string.Join(", ", tierCardCounts)}");
            Debug.Log($"Total tiers: {tierCardCounts.Length}");

            // Example 3: Get specific level data
            Debug.Log("\n=== Example 3: Get Specific Level Data ===");
            for (int i = 1; i <= 11; i++)
            {
                MemoriesLevelData levelData = levelGenerator.GetLevelData(i);
                if (levelData != null)
                {
                    Debug.Log($"Level {i}: {levelData.totalCards} cards, {levelData.width}x{levelData.height} grid, " +
                        $"{levelData.specialCardCount} special cards, Difficulty: {levelData.difficulty:F2}");
                }
            }

            // Example 4: Get all levels
            Debug.Log("\n=== Example 4: Generate All Levels ===");
            List<MemoriesLevelData> allLevels = levelGenerator.GenerateAllLevels();
            Debug.Log($"Generated {allLevels.Count} levels:");
            foreach (var level in allLevels)
            {
                Debug.Log($"  {level}");
            }

            // Example 5: How to use in game
            Debug.Log("\n=== Example 5: Usage in Game ===");
            LoadGameLevel(5);
        }

        /// <summary>
        /// Example of how to use level generator in actual game
        /// </summary>
        private void LoadGameLevel(int levelIndex)
        {
            Debug.Log($"\n--- Loading Level {levelIndex} ---");

            MemoriesLevelData levelData = levelGenerator.GetLevelData(levelIndex);

            if (levelData != null)
            {
                Debug.Log($"Level Configuration:");
                Debug.Log($"  Level Index: {levelData.levelIndex}");
                Debug.Log($"  Total Cards: {levelData.totalCards}");
                Debug.Log($"  Grid Size: {levelData.width} x {levelData.height}");
                Debug.Log($"  Special Card Count: {levelData.specialCardCount}");
                Debug.Log($"  Difficulty: {levelData.difficulty:F2}");

                // Apply to MemoriesGame
                // MemoriesGame game = GetComponent<MemoriesGame>();
                // if (game != null)
                // {
                //     game.widthSize = levelData.width;
                //     game.heightSize = levelData.height;
                //     game.specialCardCount = levelData.specialCardCount;
                //     game.GenerateCard();
                // }
            }
        }

        /// <summary>
        /// Test difficulty curve
        /// </summary>
        public void TestDifficultyCurve()
        {
            Debug.Log("\n=== Difficulty Curve ===");
            List<MemoriesLevelData> allLevels = levelGenerator.GenerateAllLevels();

            for (int i = 0; i < allLevels.Count; i++)
            {
                float difficultyPercent = allLevels[i].difficulty * 100;
                string difficultyBar = new string('█', (int)(difficultyPercent / 5));
                Debug.Log($"Level {i + 1:D2}: {difficultyBar,20} {difficultyPercent:F0}%");
            }
        }

        /// <summary>
        /// Test grid size calculations
        /// </summary>
        public void TestGridSizes()
        {
            Debug.Log("\n=== Grid Size Calculations ===");
            int[] tierCardCounts = MemoriesLevelGenerator.GetTierCardCounts();

            for (int tierIndex = 0; tierIndex < tierCardCounts.Length; tierIndex++)
            {
                int count = tierCardCounts[tierIndex];
                MemoriesLevelData data = levelGenerator.GenerateLevelData(tierIndex + 1, tierIndex, count, 1);
                int gridArea = data.width * data.height;
                Debug.Log($"{count,2} cards -> {data.width}x{data.height} (area: {gridArea})");
            }
        }

        /// <summary>
        /// Test special card distribution
        /// </summary>
        public void TestSpecialCardDistribution()
        {
            Debug.Log("\n=== Special Card Distribution ===");
            List<MemoriesLevelData> allLevels = levelGenerator.GenerateAllLevels();

            foreach (var level in allLevels)
            {
                float percentage = (float)level.specialCardCount / level.totalCards * 100;
                Debug.Log($"Level {level.levelIndex}: {level.specialCardCount}/{level.totalCards} special cards ({percentage:F1}%)");
            }
        }
    }
}
