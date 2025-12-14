using UnityEngine;
using TMPro;
using DACN.Account;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class AuthUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject chooseModePanel;
    public GameObject chooseChildProfilePanel;

    [Header("Login UI")]
    public TMP_InputField loginUser;
    public TMP_InputField loginPass;
    public Button loginPassShowHideBtn;
    public Toggle rememberMeToggle;
    public TMP_Text loginMsg;

    [Header("Register UI")]
    public TMP_InputField regUser;
    public TMP_InputField regPass;
    public TMP_InputField regName;
    public Button regPassShowHideBtn;
    public TMP_Text regMsg;
    public Button changeToLoginButton;

    [Header("Choose Mode UI")]
    public Button parentModeButton;
    public Button childModeButton;

    [Header("Buttons")]
    public Button loginButton;
    public Button registerButton;
    public Button switchToRegisterButton;

    private bool loginPassVisible = false;
    private bool regPassVisible = false;

    private LoadChildProfile loadChildProfile;

    void Start()
    {
        ShowUIPanel();
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

        // Setup choose mode buttons
        parentModeButton.onClick.AddListener(OnParentModeSelected);
        childModeButton.onClick.AddListener(OnChildModeSelected);

        loadChildProfile = GetComponent<LoadChildProfile>();
        
        // Load Remember Me data
        LoadRememberMeData();
    }

    // ===== PANEL =====
    public void ShowUIPanel()
    {
        if (LocalDataManager.Instance.currentUser != null)
        {
            OnShowChooseMode();
        }
        else
        {
            ShowLogin();
        }
    }
    public void ShowLogin()
    {
        loginMsg.gameObject.SetActive(false);
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        
        // Clear current session when showing login
        LocalDataManager.Instance.Logout();
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
            
            // Save Remember Me data
            SaveRememberMeData(loginUser.text, loginPass.text);
            
            // TODO: Load Parent Panel
            loginUser.text = "";
            loginPass.text = "";
            OnShowChooseMode();
            //SceneManager.LoadScene("ParentScene");
        }
        else
        {
            loginMsg.gameObject.SetActive(true);
            loginMsg.text = "Sai tài khoản hoặc mật khẩu";
        }
    }

    public void OnShowChooseMode()
    {
        chooseModePanel.SetActive(true);
        chooseModePanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        loginPanel.SetActive(false);
    }

    public void OnShowChooseChildProfile()
    {
        loadChildProfile.RefreshChildList();
        chooseChildProfilePanel.SetActive(true);
        chooseChildProfilePanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        chooseModePanel.SetActive(false);
    }

    public void OnParentModeSelected()
    {
        SceneManager.LoadScene("ParentScene");
    }

    public void OnChildModeSelected()
    {
        OnShowChooseChildProfile();
    }

    // ===== REMEMBER ME =====
    void LoadRememberMeData()
    {
        if (PlayerPrefs.HasKey("RememberMe") && PlayerPrefs.GetInt("RememberMe") == 1)
        {
            string savedUsername = PlayerPrefs.GetString("SavedUsername", "");
            string savedPassword = PlayerPrefs.GetString("SavedPassword", "");
            
            loginUser.text = savedUsername;
            loginPass.text = savedPassword;
            rememberMeToggle.isOn = true;
            
            Debug.Log("Loaded saved login: " + savedUsername);
        }
        else
        {
            rememberMeToggle.isOn = false;
        }
    }

    void SaveRememberMeData(string username, string password)
    {
        if (rememberMeToggle.isOn)
        {
            PlayerPrefs.SetInt("RememberMe", 1);
            PlayerPrefs.SetString("SavedUsername", username);
            PlayerPrefs.SetString("SavedPassword", password);
            PlayerPrefs.Save();
            Debug.Log("Saved login credentials");
        }
        else
        {
            PlayerPrefs.SetInt("RememberMe", 0);
            PlayerPrefs.DeleteKey("SavedUsername");
            PlayerPrefs.DeleteKey("SavedPassword");
            PlayerPrefs.Save();
            Debug.Log("Cleared saved credentials");
        }
    }
}
