# Hướng Dẫn Sử Dụng Quiz Game System

## Tổng Quan
Hệ thống Quiz này cho phép:
- ✅ Hiển thị câu hỏi quiz
- ✅ Sắp xếp 4 đáp án theo thứ tự ngẫu nhiên
- ✅ Kiểm tra đáp án (xanh nếu đúng, đỏ nếu sai)
- ✅ Hiển thị popup Win/Lose
- ✅ Cộng điểm cho child khi trả lời đúng
- ✅ Tự động tăng quiz level
- ✅ Load quiz level tiếp theo hoặc reset về level 0

## Các Class Chính

### 1. **QuizPlayUIManager**
Quản lý giao diện hiển thị quiz

**Chức năng:**
- Hiển thị câu hỏi (Question)
- Hiển thị 4 button đáp án đã được xáo trộn
- Đổi màu button khi chọn đáp án (xanh = đúng, đỏ = sai)
- Phát sự kiện `OnAnswerSelected` khi người chơi chọn đáp án

**Cách Setup trong Unity:**
1. Tạo một GameObject để chứa Quiz UI
2. Thêm component `QuizPlayUIManager` vào GameObject
3. Assign các UI elements:
   - **Question Text**: TextMeshProUGUI hiển thị câu hỏi
   - **Answer Buttons**: 4 Button UI elements (đặt lần lượt vào mảng answerButtons)
   - **Answer Texts**: 4 TextMeshProUGUI tương ứng với mỗi button (đặt lần lượt vào mảng answerTexts)
4. Tuỳ chỉnh:
   - **Correct Color**: Màu khi đáp án đúng (mặc định: xanh)
   - **Incorrect Color**: Màu khi đáp án sai (mặc định: đỏ)
   - **Normal Color**: Màu bình thường (mặc định: trắng)
   - **Color Display Duration**: Thời gian hiển thị màu (mặc định: 1.5 giây)

### 2. **QuizResultPopup**
Hiển thị popup kết quả (Win/Lose)

**Chức năng:**
- Hiển thị tin nhắn thắng/thua
- Hiển thị số điểm được cộng
- Phát sự kiện `OnNextButtonClicked` khi click nút Next

**Cách Setup trong Unity:**
1. Tạo một Canvas chứa popup
2. Thêm component `QuizResultPopup`
3. Assign các UI elements:
   - **Result Text**: Tiêu đề (thắng/thua)
   - **Message Text**: Tin nhắn chi tiết
   - **Next Button**: Nút để chuyển sang câu hỏi tiếp theo
   - **Canvas Group**: Component CanvasGroup (để animate opacity)
4. Tuỳ chỉnh:
   - **Animation Duration**: Thời gian hiệu ứng (mặc định: 0.5 giây)

### 3. **QuizGameManager**
Quản lý logic chính của game

**Chức năng:**
- Load quiz theo level hiện tại của child
- Xáo trộn đáp án
- Xử lý logic khi chọn đáp án đúng/sai
- Cộng điểm cho child
- Tăng quiz level
- Load quiz tiếp theo hoặc reset về level 0

**Cách Setup trong Unity:**
1. Tạo một GameObject để chứa game logic
2. Thêm component `QuizGameManager`
3. Assign các UI manager:
   - **Quiz Play UI Manager**: Reference tới QuizPlayUIManager
   - **Result Popup**: Reference tới QuizResultPopup
4. Tuỳ chỉnh:
   - **Points Per Correct Answer**: Điểm cộng khi trả lời đúng (mặc định: 10)
   - **Delay Before Next Quiz**: Thời gian delay trước khi load quiz tiếp theo (mặc định: 0.5 giây)

## Cách Hoạt Động

```
Start()
  ↓
Load quiz level của child hiện tại
  ↓
LoadNextQuiz()
  - Lấy danh sách quiz ở level hiện tại
  - Chọn random 1 quiz
  - Hiển thị quiz lên QuizPlayUIManager
  ↓
QuizPlayUIManager.ShowQuiz()
  - Hiển thị câu hỏi
  - Xáo trộn 4 đáp án
  - Hiển thị lên 4 button
  ↓
Người chơi click vào button (OnAnswerButtonClicked)
  ↓
Kiểm tra đáp án có đúng không?
  ↓
  ├─ Đúng → HandleCorrectAnswer()
  │   - Cộng điểm cho child
  │   - Hiển thị popup Win
  │
  └─ Sai → HandleIncorrectAnswer()
      - Hiển thị popup Lose
  ↓
Người chơi click Next button
  ↓
HandleNextQuiz()
  - Tăng quiz level lên 1
  - Kiểm tra có quiz ở level mới không
  ├─ Có → Load quiz mới
  └─ Không → Reset level về 0 và reload
```

## Dependencies
- **DoTween**: Cho hiệu ứng animation
- **TextMesh Pro**: Cho hiển thị text
- **DACN.Account**: Cho quản lý account/child
- **DACN.Quiz**: Cho quản lý quiz data

## Lưu Ý Quan Trọng
1. **Child Account**: Hệ thống sử dụng child account đầu tiên từ `LocalDataManager.Instance.currentUser.childAccounts[0]`
   - Nếu cần thay đổi, sửa hàm `GetCurrentChild()`
   
2. **Quiz Database**: Cần phải có quiz được tạo trước đó trong hệ thống
   - Nếu không có quiz ở level nào, hệ thống sẽ reset về level 0
   
3. **LocalDataManager**: Dữ liệu được lưu tự động khi:
   - Cộng điểm (AddPointsToChild)
   - Tăng quiz level (IncrementQuizLevel)
   - Reset quiz level (ResetQuizLevel)

4. **Color Animation**: Hiển thị màu sắc trong 1.5 giây rồi mới hiển thị popup
   - Người chơi có thể thấy rõ câu trả lời đúng/sai

## Testing
Để test:
1. Tạo một số quiz ở các level khác nhau
2. Assign child account cho player
3. Chạy scene
4. Hệ thống sẽ tự động load quiz từ level 0
5. Kiểm tra:
   - ✅ Đáp án được xáo trộn
   - ✅ Màu sắc hiển thị đúng
   - ✅ Popup hiển thị đúng
   - ✅ Điểm được cộng
   - ✅ Level tự động tăng
   - ✅ Reset khi hết quiz
