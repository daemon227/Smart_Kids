using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inwave.DongA.DotPuzzle.UIManager
{
    public class AdvertisePopup : PopupBase
    {
        public Button watchedButton;
        public Button closeButton;
        public GameObject buyMoreHintImage;
        public GameObject buyMoreSprayImage;
        
        private void Start()
        {
            watchedButton.onClick.AddListener(()=> ButtonEventManager.Instance.onWatchVideoButtonClick?.Invoke());
            closeButton.onClick.AddListener(()=> ButtonEventManager.Instance.onClosePopupButtonClick?.Invoke());
        }
        public void ShowAdvertisementPanel(int boosterType)
        {
            if (boosterType == 0)
            {
                buyMoreHintImage.gameObject.SetActive(true);
                buyMoreSprayImage.gameObject.SetActive(false);
            }
            else
            {
                buyMoreHintImage.gameObject.SetActive(false);
                buyMoreSprayImage.gameObject.SetActive(true);
            }
        }
    }
    
}

