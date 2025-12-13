using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inwave.DongA.DotPuzzle.UIManager
{
    public class WinPopup : PopupBase
    {
        public Button nextLevelButton;
        public Button restartButton;

        private void Start()
        {
            nextLevelButton.onClick.AddListener(()=> ButtonEventManager.Instance.onNextLevelButtonClick?.Invoke() );
            restartButton.onClick.AddListener(()=> ButtonEventManager.Instance.onRestartButtonClick?.Invoke() );
        }
    }
}

