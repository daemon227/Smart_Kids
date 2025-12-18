
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.LevelPlay;
using Inwave.DongA.DotPuzzle.Manager;

namespace Inwave.DongA.DotPuzzle.UIManager
{
    public class AdvertisePopup : PopupBase
    {
        public Button watchedButton;
        public Button closeButton;
        public GameObject buyMoreHintImage;
        public GameObject buyMoreSprayImage;
        
        private LevelPlayRewardedAd rewardedVideoAd;
        private bool isWatchingAd = false;
        
        private void Start()
        {
            watchedButton.onClick.AddListener(OnWatchedButtonClick);
            closeButton.onClick.AddListener(()=> ButtonEventManager.Instance.onClosePopupButtonClick?.Invoke());
            
            // Initialize rewarded ad if not already done
            if (rewardedVideoAd == null)
            {
                InitializeRewardedAd();
            }
        }
        
        private void InitializeRewardedAd()
        {
            // Create Rewarded Video object
            rewardedVideoAd = new LevelPlayRewardedAd(AdConfig.RewardedVideoAdUnitId);
            
            // Register to Rewarded Video events
            rewardedVideoAd.OnAdLoaded += OnRewardedVideoLoaded;
            rewardedVideoAd.OnAdLoadFailed += OnRewardedVideoLoadFailed;
            rewardedVideoAd.OnAdDisplayed += OnRewardedVideoDisplayed;
            rewardedVideoAd.OnAdDisplayFailed += OnRewardedVideoDisplayFailed;
            rewardedVideoAd.OnAdRewarded += OnRewardedVideoRewarded;
            rewardedVideoAd.OnAdClosed += OnRewardedVideoClosed;
        }
        
        private void OnWatchedButtonClick()
        {
            if (!isWatchingAd)
            {
                Debug.Log("[AdvertisePopup] Loading Rewarded Video");
                rewardedVideoAd.LoadAd();
            }
        }
        
        private void OnRewardedVideoLoaded(LevelPlayAdInfo adInfo)
        {
            Debug.Log($"[AdvertisePopup] Rewarded Video Loaded: {adInfo}");
            if (rewardedVideoAd.IsAdReady())
            {
                Debug.Log("[AdvertisePopup] Showing Rewarded Video Ad");
                isWatchingAd = true;
                // Pause game
                GameManager.Instance.canInteract = false;
                rewardedVideoAd.ShowAd();
            }
        }
        
        private void OnRewardedVideoLoadFailed(LevelPlayAdError error)
        {
            Debug.Log($"[AdvertisePopup] Rewarded Video Load Failed: {error}");
            ButtonEventManager.Instance.onWatchVideoButtonClick?.Invoke();
        }
        
        private void OnRewardedVideoDisplayed(LevelPlayAdInfo adInfo)
        {
            Debug.Log($"[AdvertisePopup] Rewarded Video Displayed: {adInfo}");
        }
        
        private void OnRewardedVideoDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
        {
            Debug.Log($"[AdvertisePopup] Rewarded Video Display Failed: {adInfo}, Error: {error}");
            isWatchingAd = false;
            // Resume game
            GameManager.Instance.canInteract = true;
        }
        
        private void OnRewardedVideoRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            Debug.Log($"[AdvertisePopup] Rewarded Video Rewarded: {adInfo}, Reward: {reward}");
        }
        
        private void OnRewardedVideoClosed(LevelPlayAdInfo adInfo)
        {
            Debug.Log($"[AdvertisePopup] Rewarded Video Closed: {adInfo}");
            isWatchingAd = false;
            // Resume game
            GameManager.Instance.canInteract = true;
            // Trigger button click event
            ButtonEventManager.Instance.onWatchVideoButtonClick?.Invoke();
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
        
        private void OnDisable()
        {
            // Clean up event subscriptions
            if (rewardedVideoAd != null)
            {
                rewardedVideoAd.OnAdLoaded -= OnRewardedVideoLoaded;
                rewardedVideoAd.OnAdLoadFailed -= OnRewardedVideoLoadFailed;
                rewardedVideoAd.OnAdDisplayed -= OnRewardedVideoDisplayed;
                rewardedVideoAd.OnAdDisplayFailed -= OnRewardedVideoDisplayFailed;
                rewardedVideoAd.OnAdRewarded -= OnRewardedVideoRewarded;
                rewardedVideoAd.OnAdClosed -= OnRewardedVideoClosed;
                rewardedVideoAd.DestroyAd();
            }
        }
    }
}

