
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Inwave.DongA.DotPuzzle.UIManager
{
    public class LosePopup : PopupBase
    {
        public Button quitButton;
        public Button restartButton;
        public GameObject loseByBugImage;
        public GameObject loseByTimeImage;

        private SkeletonGraphic bugAnim;
        private SkeletonGraphic timeAnim;
        private void Start()
        {
            quitButton.onClick.AddListener(()=> ButtonEventManager.Instance.onQuitButtonClick?.Invoke() );
            restartButton.onClick.AddListener(()=> ButtonEventManager.Instance.onRestartButtonClick?.Invoke() );
            
            bugAnim = loseByBugImage.GetComponent<SkeletonGraphic>();
            timeAnim = loseByTimeImage.GetComponent<SkeletonGraphic>();
        }
        
        public void ShowLosePanel(bool isBug)
        {
            loseByBugImage.gameObject.SetActive(isBug);
            loseByTimeImage.gameObject.SetActive(!isBug);
            
            if (isBug) bugAnim.AnimationState.SetAnimation(0, "failed_2",true);
            else timeAnim.AnimationState.SetAnimation(0, "animation", false);
        }
        
    }
}

