# Quiz Game System - Implementation Details

## Tệp Được Tạo

### 1. **QuizPlayUIManager.cs**
- **Location**: `Assets/00 DotGame/02Scripts/QuizScripts/QuizPlayUIManager.cs`
- **Responsibility**: Quản lý UI hiển thị quiz

**Key Features:**
```csharp
ShowQuiz(Quiz quiz)           // Hiển thị quiz và xáo trộn đáp án
ShuffleAnswers()              // Fisher-Yates shuffle algorithm
OnAnswerSelected              // Event khi chọn đáp án (true=đúng, false=sai)
```

**Properties:**
- `questionText`: Hiển thị câu hỏi
- `answerButtons[4]`: 4 button chứa đáp án
- `answerTexts[4]`: Text của từng button
- `correctColor/incorrectColor`: Màu sắc khi đúng/sai
- `colorDisplayDuration`: Thời gian hiển thị màu (default: 1.5s)

---

### 2. **QuizResultPopup.cs**
- **Location**: `Assets/00 DotGame/02Scripts/QuizScripts/QuizResultPopup.cs`
- **Responsibility**: Hiển thị popup kết quả

**Key Methods:**
```csharp
ShowWinPopup(int pointsGained = 0)    // Hiển thị popup thắng
ShowLosePopup()                         // Hiển thị popup thua
```

**Animations:**
- Fade in/out animation (0.5s)
- Scale animation (từ 0 lên 1)

---

### 3. **QuizGameManager.cs**
- **Location**: `Assets/00 DotGame/02Scripts/QuizScripts/QuizGameManager.cs`
- **Responsibility**: Quản lý logic chính của game

**Workflow:**
```
Start()
  ├─ Lấy child account hiện tại
  ├─ Subscribe vào events
  └─ LoadNextQuiz()

LoadNextQuiz()
  ├─ Lấy quizzes ở currentQuizLevel
  ├─ Random chọn 1 quiz
  ├─ Hiển thị quiz lên UI
  └─ Nếu không có quiz → ResetQuizLevel()

OnAnswerSelected(bool isCorrect)
  ├─ Nếu đúng → HandleCorrectAnswer()
  │   ├─ AddPointsToChild()
  │   └─ ShowWinPopup()
  └─ Nếu sai → HandleIncorrectAnswer()
      └─ ShowLosePopup()

OnNextButtonClicked
  ├─ IncrementQuizLevel()
  └─ LoadNextQuiz()
```

**Key Methods:**
- `LoadNextQuiz()`: Load quiz mới ở level hiện tại
- `AddPointsToChild(int points)`: Cộng điểm và save
- `IncrementQuizLevel()`: Tăng quiz level và save
- `ResetQuizLevel()`: Reset về level 0

---

## Sequence Diagram

```
User                QuizPlayUIManager     QuizGameManager     QuizResultPopup
 |                       |                      |                    |
 |----Start()------------>|                      |                    |
 |                       |                      |                    |
 |                       |<--ShowQuiz()---------|                    |
 |<--Display Question----|                      |                    |
 |                       |                      |                    |
 |----Click Answer------>|                      |                    |
 |                       |----OnAnswerSelected->|                    |
 |<--Change Color--------|                      |                    |
 |                       |                      |----ShowWinPopup()-->|
 |                       |                      |                  [Wait]
 |<-----Display Popup----|--------------------->|<-----Click Next----|
 |                       |                      |----IncrementLevel-->|
 |                       |                      |----LoadNextQuiz()--->|
 |                       |<-----ShowQuiz()------|                    |
 |<--Display Next Q------|                      |                    |
```

---

## Data Flow

### Quiz Structure (từ QuizData.cs)
```csharp
Quiz {
    string id;
    string creatorUsername;
    int levelIndex;
    string question;
    List<string> wrongAnswers;  // 3 đáp án sai
    string correctAnswer;        // 1 đáp án đúng
    DateTime createdAt;
    DateTime updatedAt;
}
```

### Child Account (từ Appdata.cs)
```csharp
ChildAccount {
    string childId;
    string name;
    int age;
    int score;              // Điểm được cộng khi trả lời đúng
    int quizLevel;          // Level quiz hiện tại (tự động tăng)
    int dotLevel;
    int memoryLevel;
    bool acountStatus;
}
```

---

## Configuration Guide

### Colors
Mở Inspector của QuizPlayUIManager:
- **Correct Color**: `RGB(0, 255, 0)` - Xanh lá
- **Incorrect Color**: `RGB(255, 0, 0)` - Đỏ
- **Normal Color**: `RGB(255, 255, 255)` - Trắng

### Points & Timing
Mở Inspector của QuizGameManager:
- **Points Per Correct Answer**: 10 (điều chỉnh tuỳ ý)
- **Delay Before Next Quiz**: 0.5 (thời gian delay sau khi ấn nút Next)

### Popup Animation
Mở Inspector của QuizResultPopup:
- **Animation Duration**: 0.5 (thời gian fade in/out)

---

## Troubleshooting

### Popup không hiển thị?
✓ Kiểm tra `QuizResultPopup` component có được assign chưa
✓ Kiểm tra CanvasGroup tồn tại

### Màu sắc không thay đổi?
✓ Kiểm tra button Image component tồn tại
✓ Kiểm tra colors được assign trong Inspector

### Điểm không được cộng?
✓ Kiểm tra `currentUser` và `childAccounts` không rỗng
✓ Kiểm tra `LocalDataManager.Instance.Save()` được gọi

### Quiz không load?
✓ Kiểm tra QuizManager đã được initialize
✓ Kiểm tra có quiz ở level 0 trong database
✓ Kiểm tra child account `quizLevel` được set đúng

---

## Extensions (Optional)

### Thêm Sound Effects
```csharp
// Trong QuizPlayUIManager.OnAnswerButtonClicked()
if (isCorrect)
    audioSource.PlayOneShot(correctAnswerSound);
else
    audioSource.PlayOneShot(incorrectAnswerSound);
```

### Thêm Difficulty Levels
```csharp
// Trong QuizGameManager.LoadNextQuiz()
if (currentQuizLevel >= 5) {
    // Reduce time limit
    answerTimer.duration = 10f;
}
```

### Thêm Combo System
```csharp
private int correctStreak = 0;

public void HandleCorrectAnswer() {
    correctStreak++;
    if (correctStreak % 5 == 0)
        AddPointsToChild(pointsPerCorrectAnswer * 2); // 2x bonus
}
```

---

## Files Summary

| File | Purpose | Component |
|------|---------|-----------|
| QuizPlayUIManager.cs | UI display & interaction | Monobehaviour |
| QuizResultPopup.cs | Result notification | Monobehaviour |
| QuizGameManager.cs | Game logic | Monobehaviour |
| QuizSetupExample.cs | Setup guide reference | Helper |
| QUIZ_SETUP_GUIDE.md | User guide | Documentation |

