using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

namespace DACN.Account
{
    public class LoadChildProfile : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject childListPanel;
        public GameObject chooseModePanel;

        [Header("UI")]
        public Transform childListContent;
        public GameObject childProfileItemPrefab;

        public Button backBtn;

        [Header("Confirmation")]
        public GameObject confirmationPanel;
        public TMP_Text confirmationText;
        public TMP_InputField passwordInputField;
        public Button confirmBtn;
        public Button cancelBtn;
        public TMP_Text confirmMsg;

        private ChildAccount selectedChildAccount;

        void Start()
        {
            backBtn.onClick.AddListener(OpenPanel);
            confirmBtn.onClick.AddListener(OnConfirmPassword);
            cancelBtn.onClick.AddListener(OnCancelPassword);
            RefreshChildList();
        }

        // ===== SETUP BUTTONS =====
        void OpenPanel()
        {
            chooseModePanel.SetActive(true);
            chooseModePanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
            childListPanel.SetActive(false);
        }

        // ===== LOAD AND DISPLAY CHILDREN =====
        public void RefreshChildList()
        {
            var currentParent = LocalDataManager.Instance.currentUser;
            if (currentParent == null) return;

            // Xóa item cũ
            foreach (Transform child in childListContent)
            {
                Destroy(child.gameObject);
            }

            // Thêm item mới
            foreach (ChildAccount child in currentParent.childAccounts)
            {
                if (!child.acountStatus) continue; // Bỏ qua tài khoản bị xóa

                GameObject item = Instantiate(childProfileItemPrefab, childListContent);
                var itemUI = item.GetComponent<ChildProfileItemUI>();
                itemUI.SetChildProfileItem(
                    AvatarService.Instance.GetAvatarSpriteById(child.avatarId),
                    child.name,
                    child.age,
                    () => OnChildSelected(child)
                );
            }
        }

        // ===== ON CHILD SELECTED =====
        void OnChildSelected(ChildAccount childAccount)
        {
            selectedChildAccount = childAccount;
            
            // Show confirmation panel
            confirmationPanel.SetActive(true);
            confirmationPanel.transform.localScale = Vector3.zero;
            confirmationPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            
            confirmationText.text = $"Enter password for {childAccount.name}";
            passwordInputField.text = "";
            confirmMsg.text = "";
            confirmMsg.gameObject.SetActive(false);
            Debug.Log($"Showing password confirmation for: {childAccount.name}");
        }

        // ===== PASSWORD CONFIRMATION =====
        void OnConfirmPassword()
        {
            if (selectedChildAccount == null) return;
            
            string enteredPassword = passwordInputField.text;
            
            // Validate password
            if (enteredPassword == selectedChildAccount.password)
            {
                // Password correct - save child and load scene
                LocalDataManager.Instance.currentChild = selectedChildAccount;
                Debug.Log($"Password correct for child: {selectedChildAccount.name}");
                
                CloseConfirmationPanel();
                SceneManager.LoadScene("ChildScene");
            }
            else
            {
                // Password wrong - show error message
                confirmMsg.gameObject.SetActive(true);
                confirmMsg.text = "Wrong password!";
                passwordInputField.text = "";
                Debug.Log($"Wrong password for child: {selectedChildAccount.name}");
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
            selectedChildAccount = null;
        }
    }
}