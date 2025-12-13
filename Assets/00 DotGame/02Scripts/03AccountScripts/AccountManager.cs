using UnityEngine;
using TMPro;
using DACN.Account;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Login UI")]
    public TMP_InputField loginUser;
    public TMP_InputField loginPass;
    public TMP_Text loginMsg;

    [Header("Register UI")]
    public TMP_InputField regUser;
    public TMP_InputField regPass;
    public TMP_InputField regName;
    public TMP_Text regMsg;

    public Button loginButton;
    public Button registerButton;
    public Button switchToRegisterButton;

    void Start()
    {
        ShowLogin();
        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);
        switchToRegisterButton.onClick.AddListener(ShowRegister);
    }

    // ===== PANEL =====
    public void ShowLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ShowRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    // ===== REGISTER =====
    public void OnRegister()
    {
        if (regUser.text == "" || regPass.text == "")
        {
            regMsg.text = "Không được để trống";
            return;
        }

        bool success = AuthLocalService.Register(regUser.text, regPass.text, regName.text);

        if (success)
        {
            regMsg.text = "Đăng ký thành công";
            ShowLogin();
            regUser.text = "";
            regPass.text = "";
            regName.text = "";
        }
        else
        {
            regMsg.text = "Tài khoản đã tồn tại";
        }
    }

    // ===== LOGIN =====
    public void OnLogin()
    {
        if (loginUser.text == "" || loginPass.text == "")
        {
            loginMsg.text = "Không được để trống";
            return;
        }

        bool success = AuthLocalService.Login(loginUser.text, loginPass.text);

        if (success)
        {
            loginMsg.text = "Đăng nhập thành công";
            LocalDataManager.Instance.currentUser = AuthLocalService.GetCurrentUser(loginUser.text);
            Debug.Log("Login successful: " + loginUser.text);
            // TODO: Load Parent Panel
            loginUser.text = "";
            loginPass.text = "";
            SceneManager.LoadScene("MenuScene");
        }
        else
        {
            loginMsg.text = "Sai tài khoản hoặc mật khẩu";
        }
    }
}
