using System.Collections;
using System.Collections.Generic;
using DACN.Account;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace DACN.ChildScene
{
    public class ChildSceneUIManager : MonoBehaviour
    {
        [Header("Main UI Elements")]
        public GameObject mainPanel;
        public GameObject profilePanel;
        //public GameObject settingsPanel;
        
        [Header("Main UI Elements")]
        public Button playDotGameButton;
        public Button playMemoryGameButton;
        public Button playQuizGameButton;
        public Button changeModeButton;

        [Header("Text Elements")]
        public TMP_Text childNameText;
        public TMP_Text scoreText;
        public Image avatarImage;

        [Header("Parent Password Confirmation")]
        public GameObject confirmationPanel;
        public TMP_InputField passwordInputField;
        public Button confirmBtn;
        public Button cancelBtn;
        public TMP_Text confirmMsg;

        void Start()
        {
            // Setup button listeners
            playDotGameButton.onClick.AddListener(OnPlayDotGame);
            playMemoryGameButton.onClick.AddListener(OnPlayMemoryGame);
            playQuizGameButton.onClick.AddListener(OnPlayQuizGame);
            changeModeButton.onClick.AddListener(OnChangeModeClicked);
            confirmBtn.onClick.AddListener(OnConfirmParentPassword);
            cancelBtn.onClick.AddListener(OnCancelPassword);

            LoadChildProfile();
        }
        void OnPlayDotGame()
        {
            Debug.Log("Play Dot Game button clicked");
            // Load Dot Game Scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("DotGameScene");
        }
        void OnPlayMemoryGame()
        {
            Debug.Log("Play Memory Game button clicked");
            // Load Memory Game Scene
            SceneManager.LoadScene("MemoryGameScene");
        }
        void OnPlayQuizGame()
        {
            Debug.Log("Play Quiz Game button clicked");
            // Load Quiz Game Scene
            // Sau đó load scene Quiz
            SceneManager.LoadScene("QuizGameScene");
        }

        void LoadChildProfile()
        {
            var childProfile = LocalDataManager.Instance.currentChild;
            if (childProfile != null)
            {
                childNameText.text = "Welcome, " + childProfile.name + "!";
                scoreText.text = "Score: " + childProfile.score.ToString();
                avatarImage.sprite = AvatarService.Instance.GetAvatarSpriteById(childProfile.avatarId);
            }
            confirmMsg.gameObject.SetActive(false); 
        }

        // ===== CHANGE MODE (PARENT PASSWORD CONFIRMATION) =====
        void OnChangeModeClicked()
        {
            ShowParentPasswordConfirmation();
        }

        void ShowParentPasswordConfirmation()
        {
            confirmationPanel.SetActive(true);
            confirmationPanel.transform.localScale = Vector3.zero;
            confirmationPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            
            //confirmMsg.gameObject.SetActive(true);
            //confirmMsg.text = "Enter parent password to continue";
            passwordInputField.text = "";
            
            Debug.Log("Showing parent password confirmation");
        }

        void OnConfirmParentPassword()
        {
            var parentAccount = LocalDataManager.Instance.currentUser;
            if (parentAccount == null)
            {
                confirmMsg.gameObject.SetActive(true);
                confirmMsg.text = "Parent account not found!";
                return;
            }

            string enteredPassword = passwordInputField.text;

            // Validate parent password
            if (enteredPassword == parentAccount.password)
            {
                // Password correct - load parent scene
                Debug.Log($"Parent password correct. Loading ParentScene");
                CloseConfirmationPanel();
                //LocalDataManager.Instance.Logout();
                SceneManager.LoadScene("LoginScene");
            }
            else
            {
                confirmMsg.gameObject.SetActive(true);
                // Password wrong - show error message
                confirmMsg.text = "Wrong password!";
                passwordInputField.text = "";
                Debug.Log("Wrong parent password");
            }
        }

        void OnCancelPassword()
        {
            CloseConfirmationPanel();
        }

        void CloseConfirmationPanel()
        {
            confirmationPanel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                confirmationPanel.SetActive(false);
            });
            passwordInputField.text = "";
        }
    }
}