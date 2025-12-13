
using UnityEngine.UI;

namespace Inwave.DongA.DotPuzzle.UIManager
{
    public class SettingPopup : PopupBase
    {
        public Button replayButton;
        public Button continueButton;
        public Button closeButton;

        private void Start()
        {
            replayButton.onClick.AddListener(()=> ButtonEventManager.Instance.onRestartButtonClick?.Invoke() );
            continueButton.onClick.AddListener(()=> ButtonEventManager.Instance.onContinueButtonClick?.Invoke() );
            closeButton.onClick.AddListener(()=> ButtonEventManager.Instance.onClosePopupButtonClick?.Invoke() );
        }
    }
}

