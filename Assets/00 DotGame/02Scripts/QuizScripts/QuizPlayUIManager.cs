using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

namespace DACN.Quiz
{
    public class QuizPlayUIManager : MonoBehaviour
    {
        [Header("Question UI")]
        [SerializeField] private TMP_Text questionText;
        
        [Header("Answer Buttons")]
        [SerializeField] private Button[] answerButtons = new Button[4];
        [SerializeField] private TMP_Text[] answerTexts = new TMP_Text[4];
        
        [Header("Booster")]
        [SerializeField] private Button fiftyFiftyButton;
        
        [Header("Colors")]
        [SerializeField] private Color correctColor = Color.green;
        [SerializeField] private Color incorrectColor = Color.red;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color disabledColor = Color.gray;

        [Header("Settings")]
        [SerializeField] private float colorDisplayDuration = 1.5f;

        private Quiz currentQuiz;
        private bool isAnswered = false;
        private List<string> shuffledAnswers = new List<string>();
        private bool fiftyFiftyUsed = false;
        private List<int> hiddenAnswerIndices = new List<int>();

        public System.Action<bool> OnAnswerSelected; // true = correct, false = incorrect

        private void OnEnable()
        {
            SetupButtons();
            SetupBoosters();
        }

        private void SetupBoosters()
        {
            if (fiftyFiftyButton != null)
            {
                fiftyFiftyButton.onClick.AddListener(OnFiftyFiftyClicked);
            }
        }

        private void SetupButtons()
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                int index = i;
                answerButtons[i].onClick.AddListener(() => OnAnswerButtonClicked(index));
            }
        }

        public void ShowQuiz(Quiz quiz)
        {
            currentQuiz = quiz;
            isAnswered = false;
            fiftyFiftyUsed = false;
            hiddenAnswerIndices.Clear();

            // Display question
            questionText.text = quiz.question;

            // Create list of all answers and shuffle
            shuffledAnswers = new List<string>(quiz.wrongAnswers)
            {
                quiz.correctAnswer
            };
            ShuffleAnswers();

            // Display answers on buttons
            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerTexts[i].text = shuffledAnswers[i];
                
                // Reset button colors
                Image buttonImage = answerButtons[i].GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = normalColor;
                }
                
                answerButtons[i].interactable = true;
            }

            // Enable booster button
            if (fiftyFiftyButton != null)
            {
                fiftyFiftyButton.interactable = true;
                Image boosterImage = fiftyFiftyButton.GetComponent<Image>();
                if (boosterImage != null)
                {
                    boosterImage.color = normalColor;
                }
            }
        }

        private void ShuffleAnswers()
        {
            // Fisher-Yates shuffle
            for (int i = shuffledAnswers.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                
                // Swap
                string temp = shuffledAnswers[i];
                shuffledAnswers[i] = shuffledAnswers[randomIndex];
                shuffledAnswers[randomIndex] = temp;
            }
        }

        private void OnAnswerButtonClicked(int buttonIndex)
        {
            if (isAnswered) return;
            if (hiddenAnswerIndices.Contains(buttonIndex)) return; // Cannot click hidden buttons
            
            isAnswered = true;
            string selectedAnswer = shuffledAnswers[buttonIndex];
            bool isCorrect = selectedAnswer == currentQuiz.correctAnswer;

            // Disable all buttons
            foreach (Button btn in answerButtons)
            {
                btn.interactable = false;
            }

            // Show result colors
            StartCoroutine(ShowAnswerResult(buttonIndex, isCorrect));
        }

        private void OnFiftyFiftyClicked()
        {
            if (fiftyFiftyUsed || isAnswered) return;
            
            UseFiftyFiftyBooster();
        }

        private void UseFiftyFiftyBooster()
        {
            fiftyFiftyUsed = true;
            
            // Find correct answer index
            int correctIndex = -1;
            for (int i = 0; i < shuffledAnswers.Count; i++)
            {
                if (shuffledAnswers[i] == currentQuiz.correctAnswer)
                {
                    correctIndex = i;
                    break;
                }
            }

            // Get all wrong answer indices
            List<int> wrongIndices = new List<int>();
            for (int i = 0; i < shuffledAnswers.Count; i++)
            {
                if (i != correctIndex)
                {
                    wrongIndices.Add(i);
                }
            }

            // Randomly hide 2 wrong answers, keep 1 wrong and the correct one
            if (wrongIndices.Count >= 2)
            {
                // Shuffle wrong indices
                for (int i = wrongIndices.Count - 1; i > 0; i--)
                {
                    int randomIdx = Random.Range(0, i + 1);
                    int temp = wrongIndices[i];
                    wrongIndices[i] = wrongIndices[randomIdx];
                    wrongIndices[randomIdx] = temp;
                }

                // Hide first 2 wrong answers
                hiddenAnswerIndices.Add(wrongIndices[0]);
                hiddenAnswerIndices.Add(wrongIndices[1]);
            }

            // Hide the buttons
            foreach (int hiddenIndex in hiddenAnswerIndices)
            {
                answerButtons[hiddenIndex].gameObject.SetActive(false);
            }

            // Disable booster button
            fiftyFiftyButton.interactable = false;
            Image boosterImage = fiftyFiftyButton.GetComponent<Image>();
            if (boosterImage != null)
            {
                boosterImage.color = disabledColor;
            }

            Debug.Log("[QuizPlayUIManager] Used 50-50 booster. Hidden indices: " + string.Join(", ", hiddenAnswerIndices));
        }

        private IEnumerator ShowAnswerResult(int selectedIndex, bool isCorrect)
        {
            // Only show color of the selected button
            Image selectedButtonImage = answerButtons[selectedIndex].GetComponent<Image>();
            if (selectedButtonImage != null)
            {
                selectedButtonImage.color = isCorrect ? correctColor : incorrectColor;
            }

            yield return new WaitForSeconds(colorDisplayDuration);

            // Invoke callback
            OnAnswerSelected?.Invoke(isCorrect);
        }

        public void ResetUI()
        {
            isAnswered = false;
            fiftyFiftyUsed = false;
            hiddenAnswerIndices.Clear();

            foreach (Button btn in answerButtons)
            {
                btn.interactable = true;
                btn.gameObject.SetActive(true);
                Image buttonImage = btn.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = normalColor;
                }
            }

            // Reset booster button
            if (fiftyFiftyButton != null)
            {
                fiftyFiftyButton.interactable = true;
                Image boosterImage = fiftyFiftyButton.GetComponent<Image>();
                if (boosterImage != null)
                {
                    boosterImage.color = normalColor;
                }
            }
        }
    }
}
