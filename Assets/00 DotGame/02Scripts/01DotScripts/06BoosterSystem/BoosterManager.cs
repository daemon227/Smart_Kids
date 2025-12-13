using System;
using DG.Tweening;
using Inwave.DongA.DotPuzzle.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterManager : MonoBehaviour
{
    
    [Header("Booster Buttons")]
    public Button hintButton;
    public Button sprayButton;
    
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI sprayText;
    
    [Header("Booster Count")]
    [SerializeField] private int hintCount = 2;
    [SerializeField] private int sprayCount = 1;
    
    int boosterType = -1;
    
    public int HintCount => hintCount;
    public int SprayCount => sprayCount;
    
    private void Start()
    {
        hintButton.onClick.AddListener(OnHintButtonClicked);
        sprayButton.onClick.AddListener(OnSprayButtonClicked);
        hintText.text = hintCount.ToString();
        sprayText.text = sprayCount.ToString();
        
        ButtonEventManager.Instance.onWatchVideoButtonClick += AddNewBooster;
    }

    private void OnEnable()
    {
        ButtonEventManager.Instance.onWatchVideoButtonClick -= AddNewBooster;
    }

    public void OnHintButtonClicked()
    {
        if (!GameManager.Instance.canInteract || GameManager.Instance.isGameOver) return;
        if (hintCount <= 0)
        {
            Debug.Log("No more hint, Need to buy more booster");
            boosterType = 0;
            PopupManager.Instance.OPenAdvertisePopup(boosterType);
            return;
        }

        if (LineManager.Instance.IsShowingSuggetLine) return;
        ButtonEventManager.Instance.onHintButtonClick?.Invoke();
        AddHint(-1);
    }

    public void OnSprayButtonClicked()
    {
        if (!GameManager.Instance.canInteract || GameManager.Instance.isGameOver) return;
        if (sprayCount <= 0)
        {
            Debug.Log("No more spray, Need to buy more booster");
            boosterType = 1;
            PopupManager.Instance.OPenAdvertisePopup(boosterType);
            return;
        }

        if (!GameManager.Instance.hasInsects)
        {
            Debug.Log("No insects to spray");
            return;
        }
        ButtonEventManager.Instance.onSprayButtonClick?.Invoke();
        AddSpray(-1);

    }
    
    public void AddNewBooster()
    {
        if (boosterType == 0)
        {
            AddHint(1);
        }
        else
        {
            AddSpray(1);
        }
        PopupManager.Instance.ClosePopup();
    }
    
    public void AddHint(int value)
    {
        hintCount += value;
        hintText.text = hintCount+"";
    }

    public void AddSpray(int value)
    {
        sprayCount += value;
        sprayText.text = sprayCount+"";
    }
    
}
