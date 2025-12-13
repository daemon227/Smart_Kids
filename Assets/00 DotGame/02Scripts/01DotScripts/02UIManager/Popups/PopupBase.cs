using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Inwave.DongA.DotPuzzle.Manager;
using UnityEngine;

public class PopupBase : MonoBehaviour
{
    public GameObject popup;
    public void ShowPopup()
    {
        popup.SetActive(true);
        DOTween.PauseAll();
        popup.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBack).SetUpdate(true);
    }
    public void HidePopup()
    {
        popup.SetActive(false);
        DOTween.PlayAll();
    }
}
