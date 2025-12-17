# Memories Level Generator - Documentation

## Overview
Tự động sinh levels cho game Memory Card từ dễ đến khó với các thông số tính toán khoa học.

## Thuật Toán Level Generation

### Quy Tắc Số Thẻ
```
Levels có số thẻ hợp lệ:
- Dưới 6: 2, 3, 4, 5 (tối thiểu 2 thẻ)
- Từ 6 trở lên: bội của 6 (6, 12, 18, 24, 30, 36, 42)
- Tối đa: 45 thẻ → nhưng do điều kiện bội 6, thực tế max là 42
```

### Các Level Được Sinh Ra

| Level | Cards | Grid | Special | Difficulty |
|-------|-------|------|---------|-----------|
| 1 | 2 | 2×1 | 1 | 0.09 |
| 2 | 3 | 3×1 | 1 | 0.18 |
| 3 | 4 | 2×2 | 1 | 0.27 |
| 4 | 5 | 5×1 | 1 | 0.36 |
| 5 | 6 | 3×2 | 2 | 0.45 |
| 6 | 12 | 4×3 | 2 | 0.55 |
| 7 | 18 | 6×3 | 3 | 0.64 |
| 8 | 24 | 6×4 | 4 | 0.73 |
| 9 | 30 | 6×5 | 5 | 0.82 |
| 10 | 36 | 6×6 | 6 | 0.91 |
| 11 | 42 | 7×6 | 7 | 1.00 |

**Tổng: 11 levels**

## Key Features

### 1. **Valid Card Counts**
Chỉ cho phép số thẻ thỏa mãn điều kiện:
- 2, 3, 4, 5 (nhỏ hơn 6)
- 6, 12, 18, 24, 30, 36, 42 (bội của 6)

```csharp
List<int> validCounts = MemoriesLevelGenerator.GetValidCardCounts();
// Returns: [2, 3, 4, 5, 6, 12, 18, 24, 30, 36, 42]
```

### 2. **Grid Size Calculation**
Tính toán kích thước lưới (width × height) gần như hình vuông:
- **2 cards**: 2×1 (ngang)
- **3 cards**: 3×1 (ngang)
- **4 cards**: 2×2 (vuông)
- **5 cards**: 5×1 (ngang)
- **6+ cards**: Tìm yếu số gần nhất để tạo grid cân bằng

```
Ví dụ: 24 cards
√24 ≈ 4.9 → 6×4 (gần nhất)
```

### 3. **Special Card Count**
Số lượng thẻ đặc biệt tăng dần theo độ khó:
- Levels 1-4: 1 thẻ đặc biệt
- Levels 5-6: 2 thẻ đặc biệt
- Levels 7-8: 3-4 thẻ đặc biệt
- Levels 9-11: 5-7 thẻ đặc biệt

### 4. **Difficulty Score**
Giá trị từ 0.0 (dễ) đến 1.0 (khó):
```
difficulty = (levelIndex) / totalLevels
```

## Usage Examples

### Example 1: Get All Levels
```csharp
MemoriesLevelGenerator generator = GetComponent<MemoriesLevelGenerator>();

List<MemoriesLevelData> allLevels = generator.GenerateAllLevels();

foreach (var level in allLevels)
{
    Debug.Log(level); 
    // Output: Level 1: 2 cards (2x1), 1 special, Difficulty: 0.09
}
```

### Example 2: Get Specific Level
```csharp
MemoriesLevelData level5 = generator.GetLevelData(5);

Debug.Log($"Level {level5.levelIndex}:");
Debug.Log($"  Total Cards: {level5.totalCards}");
Debug.Log($"  Grid: {level5.width}x{level5.height}");
Debug.Log($"  Special Cards: {level5.specialCardCount}");
Debug.Log($"  Difficulty: {level5.difficulty:F2}");

// Output:
// Level 5:
//   Total Cards: 6
//   Grid: 3x2
//   Special Cards: 2
//   Difficulty: 0.45
```

### Example 3: Use in MemoriesGame
```csharp
public class MemoriesGame : MonoBehaviour
{
    private MemoriesLevelGenerator levelGenerator;
    private int currentLevel = 1;

    private void Start()
    {
        levelGenerator = GetComponent<MemoriesLevelGenerator>();
        LoadLevel(currentLevel);
    }

    private void LoadLevel(int levelIndex)
    {
        MemoriesLevelData levelData = levelGenerator.GetLevelData(levelIndex);
        
        if (levelData != null)
        {
            widthSize = levelData.width;
            heightSize = levelData.height;
            specialCardCount = levelData.specialCardCount;
            
            Debug.Log($"Loading {levelData}");
            GenerateCard();
        }
    }
}
```

### Example 4: Debug All Levels
```csharp
MemoriesLevelGenerator generator = GetComponent<MemoriesLevelGenerator>();
generator.PrintAllLevels();

// Output in Console:
// === Memory Card Game Levels ===
// Total Levels: 11
// ================================
// Level 1: 2 cards (2x1), 1 special, Difficulty: 0.09
// Level 2: 3 cards (3x1), 1 special, Difficulty: 0.18
// ...
// Level 11: 42 cards (7x6), 7 special, Difficulty: 1.00
// ================================
```

## Integration with MemoriesGame

### Option 1: Add Component to MemoriesGame
```csharp
// In MemoriesGame.cs Start()
if (levelGenerator == null)
    levelGenerator = GetComponent<MemoriesLevelGenerator>();

MemoriesLevelData currentLevelData = levelGenerator.GetLevelData(currentLevel);
widthSize = currentLevelData.width;
heightSize = currentLevelData.height;
specialCardCount = currentLevelData.specialCardCount;
```

### Option 2: Standalone Service
```csharp
// Create empty GameObject with MemoriesLevelGenerator
GameObject levelGenObject = new GameObject("LevelGenerator");
MemoriesLevelGenerator levelGenerator = levelGenObject.AddComponent<MemoriesLevelGenerator>();

// Get level data anywhere
MemoriesLevelData level = levelGenerator.GetLevelData(3);
```

## API Reference

### MemoriesLevelGenerator

```csharp
// Get all valid card counts
public static List<int> GetValidCardCounts()

// Generate all levels
public List<MemoriesLevelData> GenerateAllLevels()

// Generate specific level
public MemoriesLevelData GenerateLevelData(int levelIndex, int totalCards)

// Get level by index
public MemoriesLevelData GetLevelData(int levelIndex)

// Get total number of levels
public int GetTotalLevels()

// Print all levels to console
public void PrintAllLevels()
```

### MemoriesLevelData

```csharp
public int levelIndex;           // Level number (1, 2, 3, ...)
public int totalCards;           // Total number of cards
public int width;                // Grid width
public int height;               // Grid height
public int specialCardCount;     // Number of special cards
public float difficulty;         // 0.0 (easy) to 1.0 (hard)
```

## Design Rationale

### Why These Card Counts?
- **2-5 cards**: Introduce game gradually
- **6, 12, 18, ...: Multiples of 6**: Ensure balanced grids and pair-friendly counts

### Why These Special Card Counts?
- Start low (1 special card) so players learn mechanics
- Gradually increase to add complexity
- Max 7 special cards at 42 total (highest level)

### Why This Difficulty Curve?
- Linear difficulty increase per level
- Each level is noticeably harder than previous
- Not too steep to avoid frustration

## Performance Notes
- Level generation is **O(n)** where n = number of valid card counts
- Only 11 levels total, so generation is instant
- No loops or heavy calculations per level

## Future Enhancements
- Add time limits per level
- Add move limits
- Add multiplier bonuses for high scores
- Add leaderboard per difficulty level
- Add power-ups/boosters per level
