using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class MemoriesGame : MonoBehaviour
{
    // Tham chiếu đến UI và Prefab thẻ bài
    public GameObject cardGroupUI;    
    public GameObject cardPrefab;

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
    
    private List<GameObject> cards = new List<GameObject>();// Danh sách lưu trữ các thẻ bài

    public int SpecialCardCount { get => specialCardCount; set => specialCardCount = value; }

    public event Action<int> OnUpdateSpecialCardFound; // Sự kiện cập nhật số thẻ đặc biệt đã tìm thấy

    private void Start()
    {
        GenerateCard();
        selectedSpecialCardIndex = UnityEngine.Random.Range(0, specialCardSprites.Count);// Chọn ngẫu nhiên một thẻ đặc biệt từ danh sách
        SettupSpecialCard();
        MoveCardGroup();
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
        yield return new WaitForSeconds(3f);
        FilpAllCard();
    }
    public void FilpAllCard() // Lật tất cả thẻ về trạng thái bình thường
    {
        foreach (var card in cards)
        {
            card.transform.DORotate(new Vector3(0, 90, 0), 0.4f).OnComplete(() =>
            {
                MemorieCard memorieCard = card.GetComponent<MemorieCard>();
                card.GetComponent<Image>().sprite = memorieCard.normalSprite; // Hiển thị thẻ bình thường
                card.GetComponent<MemorieCard>().IsFlipped = false; // Đặt lại trạng thái lật
                card.transform.DORotate(new Vector3(0, 0, 0), 0.4f);
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
            EndGame();
        }
        else
        {
            Debug.Log("This is a normal card.");
            clickCount++;
        }
        if(clickCount >= maxClickCount)
        {
            FilpAllCard();
            clickCount = 0;
            specialCardFound = 0;
            OnUpdateSpecialCardFound?.Invoke(specialCardFound);
        }
    }

    public void EndGame()
    {
        if(specialCardFound >= SpecialCardCount)
        {
            Debug.Log("You win the game!");
            MiniGameManager.Instance.EndMiniGame(true);
        }
    }
}
