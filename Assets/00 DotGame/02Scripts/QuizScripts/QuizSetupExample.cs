using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DACN.Quiz
{
    /// <summary>
    /// Example setup for Quiz Game UI
    /// This script shows how to properly configure the Quiz system in Unity Editor
    /// </summary>
    public class QuizSetupExample : MonoBehaviour
    {
        /* 
         * SETUP INSTRUCTIONS:
         * 
         * 1. CREATE HIERARCHY:
         *    - Create a Canvas
         *      ├── QuizUIContainer (Panel with VerticalLayoutGroup)
         *      │   ├── QuestionPanel
         *      │   │   └── QuestionText (TextMeshProUGUI)
         *      │   ├── AnswerButtonsPanel (GridLayoutGroup, 2x2)
         *      │   │   ├── Button1 (Image: normal color)
         *      │   │   │   └── Text (TextMeshProUGUI)
         *      │   │   ├── Button2 (Image: normal color)
         *      │   │   │   └── Text (TextMeshProUGUI)
         *      │   │   ├── Button3 (Image: normal color)
         *      │   │   │   └── Text (TextMeshProUGUI)
         *      │   │   └── Button4 (Image: normal color)
         *      │   │       └── Text (TextMeshProUGUI)
         * 
         * 2. CREATE POPUP:
         *    - Create another Panel for ResultPopup
         *      ├── ResultPanel (CanvasGroup)
         *      │   ├── ResultText (TextMeshProUGUI) - "🎉 Chính xác! 🎉"
         *      │   ├── MessageText (TextMeshProUGUI) - "Bạn được cộng 10 điểm!"
         *      │   └── NextButton (Button)
         * 
         * 3. ADD COMPONENTS:
         *    - Add QuizPlayUIManager to QuizUIContainer
         *      - Assign QuestionText
         *      - Assign 4 Answer Buttons
         *      - Assign 4 Answer TextMeshProUGUI components
         * 
         *    - Add QuizResultPopup to ResultPanel
         *      - Assign ResultText, MessageText, NextButton, CanvasGroup
         * 
         *    - Create a new GameObject "QuizGameLogic"
         *      - Add QuizGameManager component
         *      - Assign QuizPlayUIManager and QuizResultPopup references
         * 
         * 4. COLORS:
         *    - Set Button Images to use Color block:
         *      - Normal Color: White
         *      - Highlighted Color: Light Gray
         *      - Pressed Color: Dark Gray
         * 
         * 5. FONTS & SIZES:
         *    - Question Text: Size 40, Bold
         *    - Answer Text: Size 28
         *    - Result Text: Size 50, Bold
         *    - Message Text: Size 24
         * 
         * 6. CONTENT SIZE FITTERS:
         *    - Add ContentSizeFitter to button text to auto-fit
         * 
         * 7. TEST DATA:
         *    - Create some Quiz data in QuizManager before running
         */

        public void PrintSetupGuide()
        {
            Debug.Log("Quiz Setup Guide printed to console. Check comments in QuizSetupExample.cs");
        }
    }
}
