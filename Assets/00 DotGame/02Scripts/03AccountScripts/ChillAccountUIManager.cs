using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DACN.Account
{   
    public class ChildAccountUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject childListPanel;
    public GameObject addChildPanel;
    public GameObject editChildPanel;
    public GameObject limitedTimePanel;

    [Header("Child List UI")]
    public Transform childListContent;
    public GameObject childItemPrefab;
    public Button addNewChildBtn;

    [Header("Add Child UI")]
    public TMP_InputField addUsername;
    public TMP_InputField addPassword;
    public TMP_InputField addName;
    public Button addConfirmBtn;
    public Button addCancelBtn;
    public TMP_Text addMsg;

    [Header("Edit Child UI")]
    public TMP_InputField editUsername;
    public TMP_InputField editPassword;
    public TMP_InputField editName;
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

    void Start()
    {
        currentParent = LocalDataManager.Instance.currentUser;
        if (currentParent != null) Debug.Log("Current Parent: " + currentParent.username);
        else Debug.Log("No Current Parent");

        Debug.Log("ChildAccountUIManager Start");
        SetupButtons();
        RefreshChildList();
    }

    // ===== SETUP BUTTONS =====
    void SetupButtons()
    {
        Debug.Log("Setup Buttons");
        addNewChildBtn.onClick.AddListener(ShowAddPanel);
        addConfirmBtn.onClick.AddListener(() => OnAddChild());
        addCancelBtn.onClick.AddListener(() => HideAddPanel());
        editConfirmBtn.onClick.AddListener(() => OnEditChild());
        editCancelBtn.onClick.AddListener(() => HideEditPanel());
        editDeleteBtn.onClick.AddListener(() => OnDeleteChild());
        //editLimitedTimeBtn.onClick.AddListener(() => ShowLimitedTimePanel());
        limitedTimeConfirmBtn.onClick.AddListener(() => OnSetLimitedTime());
        limitedTimeCancelBtn.onClick.AddListener(() => HideLimitedTimePanel());
    }

    // ===== REFRESH LIST =====
    public void RefreshChildList()
    {
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
            itemUI.SetData(child.name, child.score, child.isLimitedTimeMode ? child.limitedTimePerDay / 60f : 0f);
            itemUI.OnEditClick = () => ShowEditPanel(child.username);
            itemUI.OnEditTimeClick = () => {
                selectedChildUsername = child.username;
                ShowLimitedTimePanel();
            };
        }
    }

    // ===== ADD CHILD =====
    void ShowAddPanel()
    {
        Debug.Log("Show Add Child Panel");
        addChildPanel.SetActive(true);
        childListPanel.SetActive(false);
        ClearAddFields();
    }

    void HideAddPanel()
    {
        addChildPanel.SetActive(false);
        childListPanel.SetActive(true);
        ClearAddFields();
    }

    void ClearAddFields()
    {
        addUsername.text = "";
        addPassword.text = "";
        addName.text = "";
        addMsg.text = "";
    }

    void OnAddChild()
    {
        if (addUsername.text == "" || addPassword.text == "" || addName.text == "")
        {
            addMsg.text = "Không được để trống";
            return;
        }

        if (currentParent == null)
        {
            addMsg.text = "Lỗi: Không tìm thấy tài khoản cha mẹ";
            return;
        }

        bool success = ChildAccountService.AddChild(
            currentParent,
            addUsername.text,
            addPassword.text,
            addName.text);

        if (success)
        {
            addMsg.text = "Thêm tài khoản con thành công";
            StartCoroutine(HideAddPanelDelayed());
            RefreshChildList();
        }
        else
        {
            addMsg.text = "Tên đăng nhập đã tồn tại";
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
        ChildAccount child = currentParent.childAccounts.Find(c => c.username == childUsername);

        if (child == null) return;

        editUsername.text = child.username;
        editPassword.text = child.password;
        editName.text = child.name;
        editMsg.text = "";

        editUsername.interactable = false; // Không cho sửa username

        editChildPanel.SetActive(true);
        childListPanel.SetActive(false);
    }

    void HideEditPanel()
    {
        editChildPanel.SetActive(false);
        childListPanel.SetActive(true);
        selectedChildUsername = "";
    }

    void OnEditChild()
    {
        if (editPassword.text == "" || editName.text == "")
        {
            editMsg.text = "Không được để trống";
            return;
        }

        bool success = ChildAccountService.EditChild(
            currentParent,
            selectedChildUsername,
            editPassword.text,
            editName.text);

        if (success)
        {
            editMsg.text = "Cập nhật thành công";
            StartCoroutine(HideEditPanelDelayed());
            RefreshChildList();
        }
        else
        {
            editMsg.text = "Cập nhật thất bại";
        }
    }

    void OnDeleteChild()
    {
        if (!ConfirmDelete(selectedChildUsername))
            return;

        bool success = ChildAccountService.DeleteChild(currentParent, selectedChildUsername);

        if (success)
        {
            editMsg.text = "Xóa tài khoản thành công";
            StartCoroutine(HideEditPanelDelayed());
            RefreshChildList();
        }
        else
        {
            editMsg.text = "Xóa tài khoản thất bại";
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

        ChildAccount child = currentParent.childAccounts.Find(c => c.username == selectedChildUsername);
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
                limitedTimeMsg.text = "Cập nhật thành công";
                StartCoroutine(HideLimitedTimePanelDelayed());
                RefreshChildList();
            }
            else
            {
                limitedTimeMsg.text = "Cập nhật thất bại";
            }
            return;
        }

        // Bật giới hạn thời gian
        if (!int.TryParse(limitedTimeHours.text, out int hours) || hours < 0)
        {
            limitedTimeMsg.text = "Giờ không hợp lệ";
            return;
        }

        if (!int.TryParse(limitedTimeMinutes.text, out int minutes) || minutes < 0 || minutes >= 60)
        {
            limitedTimeMsg.text = "Phút không hợp lệ (0-59)";
            return;
        }

        int totalSeconds = hours * 3600 + minutes * 60;

        if (totalSeconds == 0)
        {
            limitedTimeMsg.text = "Thời gian phải lớn hơn 0";
            return;
        }

        bool result = ChildAccountService.SetLimitedTime(
            currentParent,
            selectedChildUsername,
            true,
            totalSeconds);

        if (result)
        {
            limitedTimeMsg.text = "Cập nhật thành công";
            StartCoroutine(HideLimitedTimePanelDelayed());
            RefreshChildList();
        }
        else
        {
            limitedTimeMsg.text = "Cập nhật thất bại";
        }
    }

    IEnumerator HideLimitedTimePanelDelayed()
    {
        yield return new WaitForSeconds(1f);
        HideLimitedTimePanel();
    }

}
}
