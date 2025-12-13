using DG.Tweening;
using UnityEngine;

public class WinLoseUIManager : MonoBehaviour
{
    public GameObject panel;
    public GameObject winpanel;
    public GameObject losepanel;

    public void ShowWinPanel()
    {
        panel.SetActive(true);
        winpanel.SetActive(true);
        winpanel.transform.DOScale(Vector3.one, 0.8f).From(Vector3.zero).SetEase(Ease.OutBack);
    }
    public void ShowLosePanel()
    {
        panel.SetActive(true);
        losepanel.SetActive(true);
        winpanel.transform.DOScale(Vector3.one, 0.8f).From(Vector3.zero).SetEase(Ease.OutBack);
    }
    public void CloseWinPanel()
    {
        panel.SetActive(false);
        winpanel.SetActive(false);
    }
    public void CloseLosePanel()
    {
        panel.SetActive(false);
        losepanel.SetActive(false);
    }
}
