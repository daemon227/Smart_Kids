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
    public Button loginPassShowHideBtn;
    public TMP_Text loginMsg;

    [Header("Register UI")]
    public TMP_InputField regUser;
    public TMP_InputField regPass;
    public TMP_InputField regName;
    public Button regPassShowHideBtn;
    public TMP_Text regMsg;
    public Button changeToLoginButton;

    public Button loginButton;
    public Button registerButton;
    public Button switchToRegisterButton;

    private bool loginPassVisible = false;
    private bool regPassVisible = false;

    void Start()
    {
        ShowLogin();
        loginButton.onClick.AddListener(OnLogin);
        registerButton.onClick.AddListener(OnRegister);
        switchToRegisterButton.onClick.AddListener(ShowRegister);
        changeToLoginButton.onClick.AddListener(ShowLogin);
        // Setup password show/hide buttons
        loginPassShowHideBtn.onClick.AddListener(ToggleLoginPassword);
        regPassShowHideBtn.onClick.AddListener(ToggleRegPassword);
        
        // Set initial password content type
        loginPass.contentType = TMP_InputField.ContentType.Password;
        regPass.contentType = TMP_InputField.ContentType.Password;
    }

    // ===== PANEL =====
    public void ShowLogin()
    {
        loginMsg.gameObject.SetActive(false);
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ShowRegister()
    {
        regMsg.gameObject.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    // ===== PASSWORD TOGGLE =====
    void ToggleLoginPassword()
    {
        loginPassVisible = !loginPassVisible;
        if (loginPassVisible)
        {
            loginPass.contentType = TMP_InputField.ContentType.Standard;
            //loginPassShowHideBtn.GetComponentInChildren<TMP_Text>().text = "Hide";
        }
        else
        {
            loginPass.contentType = TMP_InputField.ContentType.Password;
            //loginPassShowHideBtn.GetComponentInChildren<TMP_Text>().text = "Show";
        }
        loginPass.ForceLabelUpdate();
    }

    void ToggleRegPassword()
    {
        regPassVisible = !regPassVisible;
        if (regPassVisible)
        {
            regPass.contentType = TMP_InputField.ContentType.Standard;
            //regPassShowHideBtn.GetComponentInChildren<TMP_Text>().text = "Hide";
        }
        else
        {
            regPass.contentType = TMP_InputField.ContentType.Password;
            //regPassShowHideBtn.GetComponentInChildren<TMP_Text>().text = "Show";
        }
        regPass.ForceLabelUpdate();
    }

    // ===== REGISTER =====
    public void OnRegister()
    {
        if (regUser.text == "" || regPass.text == "")
        {
            regMsg.gameObject.SetActive(true);
            regMsg.text = "Không được để trống";
            return;
        }

        bool success = AuthLocalService.Register(regUser.text, regPass.text, regName.text);

        if (success)
        {
            regMsg.gameObject.SetActive(true);
            regMsg.text = "Đăng ký thành công";
            ShowLogin();
            regUser.text = "";
            regPass.text = "";
            regName.text = "";
        }
        else
        {
            regMsg.gameObject.SetActive(true);
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
            loginMsg.gameObject.SetActive(true);
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
            loginMsg.gameObject.SetActive(true);
            loginMsg.text = "Sai tài khoản hoặc mật khẩu";
        }
    }
}
