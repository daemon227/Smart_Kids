using DACN.Account;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace DACN.Memories
{
public class MemoriesGame : MonoBehaviour
{
    // Tham chiếu đến UI và Prefab thẻ bài
    public GameObject cardGroupUI;    
    public GameObject cardPrefab;

    // Level generator và Result Popup
    private MemoriesLevelGenerator levelGenerator;
    private MemoriesResultPopup resultPopup;
    [SerializeField] private int currentMemoryLevel = 1;

    // Cấu hình kích thước lưới và số thẻ đặc biệt
    [SerializeField] private int widthSize = 5;
    [SerializeField] private int heightSize = 5;
    [SerializeField] private int specialCardCount = 7;
    [SerializeField] private int specialCardFound = 0;

    // Sprites cho thẻ bài
    public Sprite normalCardSprite; 
    public List<Sprite> specialCardSprites; 
    private int selectedSpecialCardIndex = -1;

    // Biến đếm số lần click
    [SerializeField] private int clickCount = 0; 
    [SerializeField] private int maxClickCount = 3;
    
    // Animation control
    //private bool canClick = false;  // Cho phép click sau khi animation xong
    private int animatingCardsCount = 0;  // Số thẻ đang flip
    
    private List<GameObject> cards = new List<GameObject>();// Danh sách lưu trữ các thẻ bài

    public int SpecialCardCount { get => specialCardCount; set => specialCardCount = value; }

    public event Action<int> OnUpdateSpecialCardFound; // Sự kiện cập nhật số thẻ đặc biệt đã tìm thấy

    private void Start()
    {
        // Initialize level generator
        levelGenerator = GetComponent<MemoriesLevelGenerator>();
        if (levelGenerator == null)
        {
            levelGenerator = gameObject.AddComponent<MemoriesLevelGenerator>();
        }

        // Initialize result popup
        resultPopup = GetComponent<MemoriesResultPopup>();
        if (resultPopup == null)
        {
            resultPopup = gameObject.AddComponent<MemoriesResultPopup>();
        }

        // Subscribe to popup event
        if (resultPopup != null)
        {
            resultPopup.OnNextLevelButtonClicked += HandleNextLevel;
        }

        // Load current memory level
        GetCurrentLevel();
    }
    public void GetCurrentLevel(){
        currentMemoryLevel = LocalDataManager.Instance.currentChild.memoryLevel;
        LoadMemoryLevel(currentMemoryLevel);
    }

    private void OnDestroy()
    {
        if (resultPopup != null)
        {
            resultPopup.OnNextLevelButtonClicked -= HandleNextLevel;
        }
    }

    /// <summary>
    /// Load and apply level configuration
    /// </summary>
    private void LoadMemoryLevel(int levelIndex)
    {
        MemoriesLevelData levelData = levelGenerator.GetLevelData(levelIndex);
        
        if (levelData != null)
        {
            currentMemoryLevel = levelIndex;
            widthSize = levelData.width;
            heightSize = levelData.height;
            specialCardCount = levelData.specialCardCount;
            specialCardFound = 0;
            
            // Calculate maxClickCount: player can click at most 50% of total cards before losing
            // This ensures there are always normal cards to find
            int totalCards = widthSize * heightSize;
            maxClickCount = Mathf.Max(3, totalCards / 2);
            
            Debug.Log($"[MemoriesGame] Loading {levelData}");
            Debug.Log($"[MemoriesGame] MaxClickCount set to {maxClickCount} (50% of {totalCards} cards)");
            
            // Clear existing cards
            foreach (Transform child in cardGroupUI.transform)
            {
                Destroy(child.gameObject);
            }
            cards.Clear();
            
            GenerateCard();
            selectedSpecialCardIndex = UnityEngine.Random.Range(0, specialCardSprites.Count);
            SettupSpecialCard();
            MoveCardGroup();
        }
        else
        {
            Debug.LogError($"[MemoriesGame] Failed to load level {levelIndex}");
        }
    }

    public void GenerateCard() // Tạo các thẻ bài
    {
        for (int i = 0; i < widthSize; i++)
        {
            for (int j = 0; j < heightSize; j++)
            {
                GameObject card = Instantiate(cardPrefab, cardGroupUI.transform);
                card.GetComponent<Image>().sprite = normalCardSprite; 
                card.GetComponent<MemorieCard>().normalSprite = normalCardSprite;
                cards.Add(card);
                card.GetComponent<MemorieCard>().OnCardFlipped += OnCardFlipped; // Đăng ký sự kiện lật thẻ
            }
        }
    }
    public void MoveCardGroup()// Di chuyển thẻ đến vị trí trong lưới sau khi tạo
    {
        RectTransform rect = cardGroupUI.GetComponent<RectTransform>();
        rect.DOAnchorPos(Vector3.zero, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            StartCoroutine(ShowSpecialCard());
        });

    }
    public void SettupSpecialCard() // Thiết lập các thẻ đặc biệt
    {
        for (int i = 0; i < SpecialCardCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, cards.Count);

            MemorieCard memorieCard = cards[randomIndex].GetComponent<MemorieCard>();
            if(memorieCard == null)
            {
                Debug.LogError("MemorieCard component not found on the card prefab.");
                return;
            }
            if (memorieCard.IsSpecial) // Nếu thẻ đã là thẻ đặc biệt thì chọn lại
            {
                i--;
                continue;
            }
            // Thiết lập thẻ thành thẻ đặc biệt
            memorieCard.IsSpecial = true;      
            memorieCard.specialSprite = specialCardSprites[selectedSpecialCardIndex];
            cards[randomIndex].GetComponent<Image>().sprite = specialCardSprites[selectedSpecialCardIndex]; // Gán sprite thẻ special;
        }   
    }

    public IEnumerator ShowSpecialCard() // Hiển thị thẻ đặc biệt trong 2 giây
    {
        // Disable click during initial flip animation
        animatingCardsCount = cards.Count;  // All cards are animating
        
        yield return new WaitForSeconds(3f);
        
        FilpAllCard();
    }
    public void FilpAllCard() // Lật tất cả thẻ về trạng thái bình thường
    {
        animatingCardsCount = cards.Count;  // Track animation
        
        foreach (var card in cards)
        {
            card.transform.DORotate(new Vector3(0, 90, 0), 0.4f).OnComplete(() =>
            {
                MemorieCard memorieCard = card.GetComponent<MemorieCard>();
                card.GetComponent<Image>().sprite = memorieCard.normalSprite; // Hiển thị thẻ bình thường
                card.GetComponent<MemorieCard>().IsFlipped = false; // Đặt lại trạng thái lật
                card.transform.DORotate(new Vector3(0, 0, 0), 0.4f).OnComplete(() =>
                {
                    // Decrease animation counter
                    animatingCardsCount--;
                    
                    // Enable click when all animations done
                    if (animatingCardsCount <= 0)
                    {
                        Debug.Log("[MemoriesGame] All cards ready - Click enabled");
                    }
                });
            });
        }
    }

    public void OnCardFlipped(MemorieCard flippedCard) // Xử lý khi thẻ được lật
    {
        if (flippedCard.IsSpecial)
        {
            Debug.Log("You found a special card!");
            clickCount = 0;
            specialCardFound++;
            OnUpdateSpecialCardFound?.Invoke(specialCardFound);
            
            // Disable click during flip animation
            DisableClick();
            
            // Check win after flip animation completes (0.8s total for both rotations)
            StartCoroutine(CheckWinAfterAnimation());
        }
        else
        {
            Debug.Log("This is a normal card.");
            clickCount++;
        }
        
        if(clickCount >= maxClickCount)
        {
            Debug.Log($"Game Over! Clicked {clickCount} times (max: {maxClickCount})");
            // Disable click
            DisableClick();
            
            // Show lose popup after animation
            StartCoroutine(ShowLosePopupAfterAnimation());
        }
    }

    private void DisableClick()
    {
        //canClick = false;
        animatingCardsCount = 1;  // 1 card is animating
    }

    private IEnumerator CheckWinAfterAnimation()
    {
        // Wait for flip animation to complete (0.4s rotate + 0.4s rotate back = 0.8s)
        yield return new WaitForSeconds(0.8f);
        
        CheckWin();
    }

    private IEnumerator ShowLosePopupAfterAnimation()
    {
        // Wait for any ongoing animations
        yield return new WaitForSeconds(0.8f);
        
        // Show lose popup
        if (resultPopup != null)
        {
            resultPopup.ShowLosePopup();
        }
    }

    private void CheckWin()
    {
        if(specialCardFound >= SpecialCardCount)
        {
            Debug.Log($"You win the game! Level {currentMemoryLevel} completed!");
            
            // Disable click before showing popup
            //canClick = false;
            
            // Show win popup
            if (resultPopup != null)
            {
                resultPopup.ShowWinPopup(currentMemoryLevel);
            }
        }
        else
        {
            // Re-enable click if didn't win yet
            //canClick = true;
        }
    }

    private void HandleNextLevel()
    {
        // Increment level for next play
        if (currentMemoryLevel < levelGenerator.GetTotalLevels())
        {
            currentMemoryLevel++;
            LocalDataManager.Instance.currentChild.memoryLevel = currentMemoryLevel;
            LocalDataManager.Instance.Save();
            LoadMemoryLevel(currentMemoryLevel);
        }
        else
        {
            // All levels completed!
            Debug.Log("All levels completed! Resetting to level 1...");
            currentMemoryLevel = 1;
            LoadMemoryLevel(currentMemoryLevel);
        }
    }

    public void EndGame()
    {
        // Deprecated - use CheckWin() and HandleNextLevel() instead
        if(specialCardFound >= SpecialCardCount)
        {
            Debug.Log($"You win the game! Level {currentMemoryLevel} completed!");
            MiniGameManager.Instance.EndMiniGame(true);
        }
    }
}
}
