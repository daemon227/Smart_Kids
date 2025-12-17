using DG.Tweening;
using UnityEngine;

namespace DACN.Memories
{
public class WinLoseUIManager : MonoBehaviour
{
    public GameObject winpanel;
    public GameObject losepanel;

    public void ShowWinPanel()
    {
        winpanel.SetActive(true);
        winpanel.transform.DOScale(Vector3.one, 0.8f).From(Vector3.zero).SetEase(Ease.OutBack);
    }
    public void ShowLosePanel()
    {
        losepanel.SetActive(true);
        winpanel.transform.DOScale(Vector3.one, 0.8f).From(Vector3.zero).SetEase(Ease.OutBack);
    }
    public void CloseWinPanel()
    {
        winpanel.SetActive(false);
    }
    public void CloseLosePanel()
    {
        losepanel.SetActive(false);
    }
}
}
