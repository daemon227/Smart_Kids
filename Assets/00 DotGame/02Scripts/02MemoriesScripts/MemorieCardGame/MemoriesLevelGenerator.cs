using System.Collections.Generic;
using UnityEngine;

namespace DACN.Memories
{
    /// <summary>
    /// Data structure for a memories game level
    /// </summary>
    [System.Serializable]
    public class MemoriesLevelData
    {
        public int levelIndex;
        public int totalCards;
        public int width;
        public int height;
        public int specialCardCount;
        public float difficulty; // 0.0 - 1.0
        public int tierIndex; // Which tier this level belongs to

        public override string ToString()
        {
            return $"Level {levelIndex}: {totalCards} cards ({width}x{height}), {specialCardCount} special, Difficulty: {difficulty:F2}";
        }
    }

    /// <summary>
    /// Generates memory card game levels with tier-based difficulty system
    /// 
    /// System: Tiers of increasing card counts, each level increases difficulty
    /// - Special cards increase linearly from level 1 to 57
    /// - Special cards never decrease (smooth difficulty curve)
    /// - Grid size stays consistent per tier
    /// 
    /// Tiers:
    /// - Tier 1: 6 cards
    /// - Tier 2: 12 cards
    /// - Tier 3: 18 cards
    /// - Tier 4: 24 cards
    /// - Tier 5: 30 cards
    /// - Tier 6: 36 cards
    /// - Tier 7: 42 cards
    /// - Tier 8: 48 cards
    /// - Tier 9: 54 cards
    /// 
    /// Within each tier: 6-7 levels with increasing special cards
    /// Special cards start low in each tier but never below previous tier's max
    /// Total: 57 levels
    /// Max cards: 54
    /// Max special: 27 (50% of max cards)
    /// </summary>
    public class MemoriesLevelGenerator : MonoBehaviour
    {
        private static readonly int[] TIER_CARD_COUNTS = { 6, 12, 18, 24, 30, 36, 42, 48, 54 };
        private static readonly int MAX_TOTAL_CARDS = 54;

        /// <summary>
        /// Get all valid tier card counts
        /// </summary>
        public static int[] GetTierCardCounts()
        {
            return TIER_CARD_COUNTS;
        }

        /// <summary>
        /// Generate all levels from easy to hard (57 total levels)
        /// Special cards increase linearly from level 1 to 57
        /// </summary>
        public List<MemoriesLevelData> GenerateAllLevels()
        {
            List<MemoriesLevelData> levels = new List<MemoriesLevelData>();
            int levelIndex = 1;

            // Generate levels for each tier
            for (int tierIndex = 0; tierIndex < TIER_CARD_COUNTS.Length; tierIndex++)
            {
                int totalCards = TIER_CARD_COUNTS[tierIndex];
                int levelsInTier = GetLevelsInTier(tierIndex);

                // Each tier has multiple difficulty levels
                for (int tierLevel = 0; tierLevel < levelsInTier; tierLevel++)
                {
                    // Calculate special cards: increase linearly across all 57 levels
                    // Max special cards is 27 (50% of 54 cards)
                    int specialCount = CalculateSpecialCardCount(levelIndex);
                    
                    MemoriesLevelData levelData = GenerateLevelData(levelIndex, tierIndex, totalCards, specialCount);
                    levels.Add(levelData);
                    levelIndex++;
                }
            }

            return levels;
        }

        /// <summary>
        /// Get number of levels in a tier
        /// </summary>
        private int GetLevelsInTier(int tierIndex)
        {
            // First 6 tiers have 6 levels, last 3 tiers have 7 levels
            if (tierIndex < 6)
                return 6;
            else
                return 7;
        }

        /// <summary>
        /// Calculate special card count for a level (increases linearly from 1 to 27)
        /// Ensures smooth difficulty curve
        /// </summary>
        private int CalculateSpecialCardCount(int levelIndex)
        {
            // Linear increase from level 1 (1 special) to level 57 (27 special)
            // Formula: specialCount = 1 + (levelIndex - 1) * 26 / 56
            int maxSpecialCards = 27; // 50% of max 54 cards
            
            // Linear interpolation
            int specialCount = 1 + (levelIndex - 1) * (maxSpecialCards - 1) / 56;
            
            return Mathf.Clamp(specialCount, 1, maxSpecialCards);
        }

        /// <summary>
        /// Get max special card count for a tier (based on total cards)
        /// Special cards should never exceed 50% of total cards (so player can play normally)
        /// </summary>
        private int GetMaxSpecialCardsForTier(int tierIndex)
        {
            int totalCards = TIER_CARD_COUNTS[tierIndex];
            return totalCards / 2; // Max 50% of total cards can be special
        }

        /// <summary>
        /// Generate data for a specific level
        /// </summary>
        public MemoriesLevelData GenerateLevelData(int levelIndex, int tierIndex, int totalCards, int specialCardCount)
        {
            MemoriesLevelData levelData = new MemoriesLevelData();
            levelData.levelIndex = levelIndex;
            levelData.tierIndex = tierIndex;
            levelData.totalCards = totalCards;
            levelData.specialCardCount = specialCardCount;

            // Calculate grid size (width x height)
            CalculateGridSize(totalCards, out int width, out int height);
            levelData.width = width;
            levelData.height = height;

            // Calculate difficulty (0.0 to 1.0)
            // Total levels = 57 (6*6 + 3*7)
            levelData.difficulty = (float)levelIndex / 57f;

            return levelData;
        }

        /// <summary>
        /// Calculate grid dimensions (width x height) for a given total card count
        /// Tries to create a relatively square grid
        /// </summary>
        private void CalculateGridSize(int totalCards, out int width, out int height)
        {
            // Find the closest factor pair to make it square
            width = 1;
            height = totalCards;

            for (int i = (int)Mathf.Sqrt(totalCards); i > 0; i--)
            {
                if (totalCards % i == 0)
                {
                    width = i;
                    height = totalCards / i;
                    break;
                }
            }

            // Ensure width <= height for better UI layout
            if (width > height)
            {
                int temp = width;
                width = height;
                height = temp;
            }
        }

        /// <summary>
        /// Get level data by level index
        /// </summary>
        public MemoriesLevelData GetLevelData(int levelIndex)
        {
            List<MemoriesLevelData> allLevels = GenerateAllLevels();
            
            if (levelIndex >= 1 && levelIndex <= allLevels.Count)
            {
                return allLevels[levelIndex - 1];
            }

            Debug.LogError($"Level {levelIndex} not found! Max level: {allLevels.Count}");
            return null;
        }

        /// <summary>
        /// Get total number of levels
        /// </summary>
        public int GetTotalLevels()
        {
            // 6 levels per tier for first 6 tiers, 7 levels for last 3 tiers
            // 6*6 + 3*7 = 36 + 21 = 57
            return 57;
        }

        /// <summary>
        /// Print all levels to console for debugging
        /// </summary>
        public void PrintAllLevels()
        {
            List<MemoriesLevelData> levels = GenerateAllLevels();
            Debug.Log("\n=== Memory Card Game Levels (Tier-Based System) ===");
            Debug.Log($"Total Levels: {levels.Count}");
            Debug.Log($"Max Cards: {MAX_TOTAL_CARDS}");
            Debug.Log("====================================================");
            
            int currentTier = -1;
            foreach (var level in levels)
            {
                if (level.tierIndex != currentTier)
                {
                    currentTier = level.tierIndex;
                    Debug.Log($"\n--- Tier {currentTier + 1}: {level.totalCards} cards ---");
                }
                Debug.Log(level);
            }
            Debug.Log("\n====================================================");
        }

        /// <summary>
        /// Print tier summary
        /// </summary>
        public void PrintTierSummary()
        {
            Debug.Log("\n=== Tier Summary ===");
            int levelIndex = 1;
            
            for (int tierIndex = 0; tierIndex < TIER_CARD_COUNTS.Length; tierIndex++)
            {
                int totalCards = TIER_CARD_COUNTS[tierIndex];
                int maxSpecial = GetMaxSpecialCardsForTier(tierIndex);
                int tierSize = maxSpecial;
                
                Debug.Log($"Tier {tierIndex + 1}: Cards={totalCards}, Levels={tierSize}, Range=[Level {levelIndex}-{levelIndex + tierSize - 1}]");
                levelIndex += tierSize;
            }
            Debug.Log("====================\n");
        }
    }
}
