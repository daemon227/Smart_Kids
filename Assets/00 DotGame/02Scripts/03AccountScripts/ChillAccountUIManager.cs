using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace DACN.Account
{   
    public class ChildAccountUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject parentPanel;
    public GameObject childListPanel;
    public GameObject addChildPanel;
    public GameObject editChildPanel;
    public GameObject limitedTimePanel;

    [Header("Child List UI")]
    public Transform childListContent;
    public GameObject childItemPrefab;
    public Button addNewChildBtn;
    public Button backToParentBtn;
    public TMP_Text parentNameText;

    [Header("Add Child UI")]
    public TMP_InputField addPassword;
    public TMP_InputField addName;
    public TMP_InputField addAge;
    //public Button addPasswordToggleBtn;
    public Button addConfirmBtn;
    public Button addCancelBtn;
    public TMP_Text addMsg;

    [Header("Edit Child UI")]
    public TMP_InputField editPassword;
    public TMP_InputField editName;
    public TMP_InputField editAge;
    //public Button editPasswordToggleBtn;
    public Button editConfirmBtn;
    public Button editCancelBtn;
    public Button editDeleteBtn;
    //public Button editLimitedTimeBtn;
    public TMP_Text editMsg;

    [Header("Limited Time UI")]
    public Toggle limitedTimeToggle;
    public TMP_InputField limitedTimeHours;
    public TMP_InputField limitedTimeMinutes;
    public Button limitedTimeConfirmBtn;
    public Button limitedTimeCancelBtn;
    public TMP_Text limitedTimeMsg;

    private UserAccount currentParent;
    private string selectedChildUsername;
    private bool addPasswordVisible = false;
    //private bool editPasswordVisible = false;

    void Start()
    {
        currentParent = LocalDataManager.Instance.currentUser;
        if (currentParent != null) Debug.Log("Current Parent: " + currentParent.username);
        else Debug.Log("No Current Parent");

        Debug.Log("ChildAccountUIManager Start");
        SetupButtons();
        RefreshChildList();
        
        // Set initial password content type
        // addPassword.contentType = TMP_InputField.ContentType.Password;
        // editPassword.contentType = TMP_InputField.ContentType.Password;
    }

    // ===== SETUP BUTTONS =====
    void SetupButtons()
    {
        Debug.Log("Setup Buttons");
        backToParentBtn.onClick.AddListener(() => {
            parentPanel.SetActive(true);
            childListPanel.SetActive(false);
        });
        addNewChildBtn.onClick.AddListener(ShowAddPanel);
        addConfirmBtn.onClick.AddListener(() => OnAddChild());
        addCancelBtn.onClick.AddListener(() => HideAddPanel());
        //addPasswordToggleBtn.onClick.AddListener(ToggleAddPassword);
        editConfirmBtn.onClick.AddListener(() => OnEditChild());
        editCancelBtn.onClick.AddListener(() => HideEditPanel());
        editDeleteBtn.onClick.AddListener(() => OnDeleteChild());
        //editPasswordToggleBtn.onClick.AddListener(ToggleEditPassword);
        //editLimitedTimeBtn.onClick.AddListener(() => ShowLimitedTimePanel());
        limitedTimeConfirmBtn.onClick.AddListener(() => OnSetLimitedTime());
        limitedTimeCancelBtn.onClick.AddListener(() => HideLimitedTimePanel());
    }

    // ===== REFRESH LIST =====
    public void RefreshChildList()
    {
        parentNameText.text = currentParent.name;
        currentParent = LocalDataManager.Instance.currentUser;
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

            GameObject item = Instantiate(childItemPrefab, childListContent);
            ChildItemUI itemUI = item.GetComponent<ChildItemUI>();
            itemUI.SetData(child.name, child.age,child.score, child.isLimitedTimeMode ? child.limitedTimePerDay / 60f : 0f);
            itemUI.OnEditClick = () => ShowEditPanel(child.childId);
            itemUI.OnEditTimeClick = () => {
                selectedChildUsername = child.childId;
                ShowLimitedTimePanel();
            };
        }
    }

    // ===== PASSWORD TOGGLE =====
    // void ToggleAddPassword()
    // {
    //     addPasswordVisible = !addPasswordVisible;
    //     if (addPasswordVisible)
    //     {
    //         addPassword.contentType = TMP_InputField.ContentType.Standard;
    //         //addPasswordToggleBtn.GetComponentInChildren<TMP_Text>().text = "Hide";
    //     }
    //     else
    //     {
    //         addPassword.contentType = TMP_InputField.ContentType.Password;
    //         //addPasswordToggleBtn.GetComponentInChildren<TMP_Text>().text = "Show";
    //     }
    //     addPassword.ForceLabelUpdate();
    // }

    // void ToggleEditPassword()
    // {
    //     editPasswordVisible = !editPasswordVisible;
    //     if (editPasswordVisible)
    //     {
    //         editPassword.contentType = TMP_InputField.ContentType.Standard;
    //         editPasswordToggleBtn.GetComponentInChildren<TMP_Text>().text = "Hide";
    //     }
    //     else
    //     {
    //         editPassword.contentType = TMP_InputField.ContentType.Password;
    //         editPasswordToggleBtn.GetComponentInChildren<TMP_Text>().text = "Show";
    //     }
    //     editPassword.ForceLabelUpdate();
    // }

    // ===== ADD CHILD =====
    void ShowAddPanel()
    {
        Debug.Log("Show Add Child Panel");
        addChildPanel.SetActive(true);
        addChildPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        childListPanel.SetActive(false);
        ClearAddFields();
    }

    void HideAddPanel()
    {
        addChildPanel.SetActive(false);
        childListPanel.SetActive(true);
        childListPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        ClearAddFields();
    }

    void ClearAddFields()
    {
        addPassword.text = "";
        addName.text = "";
        addAge.text = "";
        addMsg.text = "";
        addPasswordVisible = false;
        //addPassword.contentType = TMP_InputField.ContentType.Password;
        //addPasswordToggleBtn.GetComponentInChildren<TMP_Text>().text = "Show";
    }

    void OnAddChild()
    {
        if (addPassword.text == "" || addName.text == "" || addAge.text == "")
        {
            addMsg.text = "Fields cannot be empty";
            return;
        }

        if (!int.TryParse(addAge.text, out int age) || age < 0 || age > 150)
        {
            addMsg.text = "Invalid age";
            return;
        }

        if (currentParent == null)
        {
            addMsg.text = "Error: Parent account not found";
            return;
        }

        bool success = ChildAccountService.AddChild(
            currentParent,
            addPassword.text,
            addName.text,
            age);

        if (success)
        {
            addMsg.text = "Child account created successfully";
            StartCoroutine(HideAddPanelDelayed());
            RefreshChildList();
        }
        else
        {
            addMsg.text = "Failed to create child account";
        }
    }

    IEnumerator HideAddPanelDelayed()
    {
        yield return new WaitForSeconds(1f);
        HideAddPanel();
    }

    // ===== EDIT CHILD =====
    void ShowEditPanel(string childUsername)
    {
        selectedChildUsername = childUsername;
        ChildAccount child = currentParent.childAccounts.Find(c => c.childId == childUsername);

        if (child == null) return;

        editPassword.text = child.password;
        editName.text = child.name;
        editAge.text = child.age.ToString();
        editMsg.text = "";
        
        // editPasswordVisible = false;
        // editPassword.contentType = TMP_InputField.ContentType.Password;
        //editPasswordToggleBtn.GetComponentInChildren<TMP_Text>().text = "Show";

        editChildPanel.SetActive(true);
        editChildPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        childListPanel.SetActive(false);
    }

    void HideEditPanel()
    {
        editChildPanel.SetActive(false);
        childListPanel.SetActive(true);
        childListPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        selectedChildUsername = "";
    }

    void OnEditChild()
    {
        if (editPassword.text == "" || editName.text == "" || editAge.text == "")
        {
            editMsg.text = "Fields cannot be empty";
            return;
        }

        if (!int.TryParse(editAge.text, out int age) || age < 0 || age > 150)
        {
            editMsg.text = "Invalid age";
            return;
        }

        bool success = ChildAccountService.EditChild(
            currentParent,
            selectedChildUsername,
            editPassword.text,
            editName.text,
            age);

        if (success)
        {
            editMsg.text = "Updated successfully";
            StartCoroutine(HideEditPanelDelayed());
            RefreshChildList();
        }
        else
        {
            editMsg.text = "Update failed";
        }
    }

    void OnDeleteChild()
    {
        if (!ConfirmDelete(selectedChildUsername))
            return;

        bool success = ChildAccountService.DeleteChild(currentParent, selectedChildUsername);

        if (success)
        {
            editMsg.text = "Account deleted successfully";
            StartCoroutine(HideEditPanelDelayed());
            RefreshChildList();
        }
        else
        {
            editMsg.text = "Failed to delete account";
        }
    }

    bool ConfirmDelete(string childUsername)
    {
        // Có thể thay bằng popup xác nhận thực tế
        return UnityEngine.EventSystems.EventSystem.current != null;
    }

    IEnumerator HideEditPanelDelayed()
    {
        yield return new WaitForSeconds(1f);
        HideEditPanel();
    }

    // ===== LIMITED TIME SETTING =====
    void ShowLimitedTimePanel()
    {
        if (string.IsNullOrEmpty(selectedChildUsername)) return;

        ChildAccount child = currentParent.childAccounts.Find(c => c.childId == selectedChildUsername);
        if (child == null) return;

        limitedTimeToggle.isOn = child.isLimitedTimeMode;
        int seconds = child.limitedTimePerDay;
        limitedTimeHours.text = (seconds / 3600).ToString();
        limitedTimeMinutes.text = ((seconds % 3600) / 60).ToString();
        limitedTimeMsg.text = "";

        limitedTimePanel.SetActive(true);
        childListPanel.SetActive(false);
    }

    void HideLimitedTimePanel()
    {
        limitedTimePanel.SetActive(false);
        childListPanel.SetActive(true);
        childListPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
    }

    void OnSetLimitedTime()
    {
        if (!limitedTimeToggle.isOn)
        {
            // Tắt giới hạn thời gian
            bool success = ChildAccountService.SetLimitedTime(
                currentParent,
                selectedChildUsername,
                false,
                0);

            if (success)
            {
                limitedTimeMsg.text = "Updated successfully";
                StartCoroutine(HideLimitedTimePanelDelayed());
                RefreshChildList();
            }
            else
            {
                limitedTimeMsg.text = "Update failed";
            }
            return;
        }

        // Bật giới hạn thời gian
        if (!int.TryParse(limitedTimeHours.text, out int hours) || hours < 0)
        {
            limitedTimeMsg.text = "Invalid hours";
            return;
        }

        if (!int.TryParse(limitedTimeMinutes.text, out int minutes) || minutes < 0 || minutes >= 60)
        {
            limitedTimeMsg.text = "Invalid minutes (0-59)";
            return;
        }

        int totalSeconds = hours *60 + minutes;

        if (totalSeconds == 0)
        {
            limitedTimeMsg.text = "Time must be greater than 0";
            return;
        }

        bool result = ChildAccountService.SetLimitedTime(
            currentParent,
            selectedChildUsername,
            true,
            totalSeconds);

        if (result)
        {
            limitedTimeMsg.text = "Updated successfully";
            StartCoroutine(HideLimitedTimePanelDelayed());
            RefreshChildList();
        }
        else
        {
            limitedTimeMsg.text = "Update failed";
        }
    }

    IEnumerator HideLimitedTimePanelDelayed()
    {
        yield return new WaitForSeconds(1f);
        HideLimitedTimePanel();
    }

}
}
