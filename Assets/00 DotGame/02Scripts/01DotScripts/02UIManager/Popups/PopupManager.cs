
using System.Collections.Generic;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.Manager;
using Inwave.DongA.DotPuzzle.UIManager;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;
    
    public WinPopup winPopup;
    public LosePopup losePopup;
    public SettingPopup settingPopup;
    public AdvertisePopup advertisePopup;
    
    public Button pauseButton;
    
    public Stack<PopupBase> popupStack = new Stack<PopupBase>();
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ButtonEventManager.Instance.onClosePopupButtonClick += ClosePopup;
        EventManager.Instance.OnGameOver += ShowResultPopup;
        pauseButton.onClick.AddListener(OpenSettingPopup);
    }
    private void OnDisable()
    {
        ButtonEventManager.Instance.onClosePopupButtonClick -= ClosePopup;
        EventManager.Instance.OnGameOver -= ShowResultPopup;
    }

    public void OpenPopup(PopupBase popup)
    {
        GameManager.Instance.isPaused  = true;
        GameManager.Instance.canInteract = false;
        popupStack.Push(popup);
        popup.ShowPopup();
    }
    public void ClosePopup()
    {
        if (popupStack.Count > 0)
        {
            GameManager.Instance.isPaused = false;
            GameManager.Instance.canInteract = true;
            popupStack.Pop().HidePopup();
        }
    }
    
    public void ShowResultPopup(bool isWin)
    {
        if (isWin)
        {
            OpenPopup(winPopup);
        }
        else
        {
            Debug.Log("lose");
            OpenPopup(losePopup);
            losePopup.ShowLosePanel(GameManager.Instance.loseByBug);
        }
    }
    
    public void OPenAdvertisePopup(int boosterType)
    {
        OpenPopup(advertisePopup);
        advertisePopup.ShowAdvertisementPanel(boosterType);
    }
    
    public void OpenSettingPopup()
    {
        if (!GameManager.Instance.canInteract || GameManager.Instance.isGameOver) return;
        OpenPopup(settingPopup);
    }
    
}
