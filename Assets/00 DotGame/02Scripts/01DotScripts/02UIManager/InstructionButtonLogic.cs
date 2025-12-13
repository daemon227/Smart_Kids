
using Inwave.DongA.DotPuzzle.Manager;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.UIManager
{
    public class InstructionButtonLogic : MonoBehaviour
    {
        private void Start()
        {
            ButtonEventManager.Instance.onNextLevelButtonClick += NextLevelHandle;
            ButtonEventManager.Instance.onRestartButtonClick += ReplayHandle;
            ButtonEventManager.Instance.onQuitButtonClick += QuitHandle;
            ButtonEventManager.Instance.onContinueButtonClick += ContinueGame;
        }

        void OnDisable()
        {
            ButtonEventManager.Instance.onNextLevelButtonClick -= NextLevelHandle;
            ButtonEventManager.Instance.onRestartButtonClick -= ReplayHandle;
            ButtonEventManager.Instance.onQuitButtonClick -= QuitHandle;
            ButtonEventManager.Instance.onContinueButtonClick -= ContinueGame;
        }

        void NextLevelHandle()
        {
            GameManager.Instance.NextLevel();
            ButtonEventManager.Instance.onClosePopupButtonClick?.Invoke();
        }
    
        void ReplayHandle()
        {
            GameManager.Instance.ReplayLevel();
            ButtonEventManager.Instance.onClosePopupButtonClick?.Invoke();
        }

        void QuitHandle()
        {
            Application.Quit();
        }

        public void ContinueGame()
        {
            Debug.Log("Continue game");
            //GameManager.Instance.isPaused = false;
            ButtonEventManager.Instance.onClosePopupButtonClick?.Invoke();
        }
        
    }
}

