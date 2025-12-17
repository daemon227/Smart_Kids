using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace DACN.Memories
{
public class MemorieCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool isSpecial = false;
    [SerializeField] private bool isFlipped = false;
    public Sprite normalSprite = null;
    public Sprite specialSprite = null;
    public bool IsFlipped { get => isFlipped; set => isFlipped = value; }
    public bool IsSpecial { get => isSpecial; set => isSpecial = value; }   

    public event Action<MemorieCard> OnCardFlipped;
    private bool isAnimating = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(MiniGameManager.Instance.isFinishedMinigame) return;
        FlipCard();
        OnCardFlipped?.Invoke(this);
    }
    public void FlipCard()
    {
        if (isFlipped) return; // Prevent flipping if already flipped
        transform.DORotate(new Vector3(0, 90, 0), 0.6f).OnComplete(() =>
        {
            // Here y
            // ou would typically change the sprite or material to show the card's face
            if (IsSpecial)
            {  
                this.GetComponent<Image>().sprite = specialSprite; // Example for special card
                isFlipped = true;
            }
            transform.DORotate(new Vector3(0, 0, 0), 0.6f);
        });
    }

}
}
