using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Inwave.DongA.DotPuzzle.UIManager
{
    public class ToggleAnim : MonoBehaviour
    {
        public Image dotImage;
        public Image fillImage;
        public float duration = 0.5f;
        public Vector2 xPos = new Vector2(-45, 45);
        private bool isOn = true;
        
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(PlayAnim);
        }
        public void PlayAnim()
        {
            var dotImageRectTransform = dotImage.rectTransform;
            if (isOn)
            {
                dotImageRectTransform.DOAnchorPosX(xPos.x, duration);
                
            }
            else
            {
                dotImageRectTransform.DOAnchorPosX(xPos.y, duration);
            }
            int dir = isOn ? -1 : 1;
            fillImage.DOFillAmount(dir, duration).SetEase(Ease.OutCubic);
            isOn = !isOn;
        }
    }
}

