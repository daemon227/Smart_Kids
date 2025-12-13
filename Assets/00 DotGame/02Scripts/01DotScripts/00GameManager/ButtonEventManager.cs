using System;
using UnityEngine;

public class ButtonEventManager : MonoBehaviour
{
    public static ButtonEventManager Instance;
    // Instruction Buttons Events
    public Action onRestartButtonClick;
    public Action onNextLevelButtonClick;
    public Action onQuitButtonClick;
    public Action onContinueButtonClick;
    public Action onClosePopupButtonClick;
    
    public Action onWatchVideoButtonClick;
    
    // Booster Buttons Events
    public Action onHintButtonClick;
    public Action onSprayButtonClick;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
}
