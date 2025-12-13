
using System;
using System.Collections;
using DG.Tweening;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpUIManager : MonoBehaviour
{
    public GameObject hpUI;
    public GameObject particleEffect;
    public Image hpBar;
    public TextMeshProUGUI hpText;
    private int maxHp = 10;
    [HideInInspector] public int currentHp = 10;
    
    private void Start()
    {
        GameManager.Instance.OnLevelLoadedEvent += ()=> StartCoroutine(ResetHp());
        EventManager.Instance.OnTakeDame += TakeDame;
        particleEffect.SetActive(false);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnLevelLoadedEvent -= ()=> StartCoroutine(ResetHp());
        EventManager.Instance.OnTakeDame -= TakeDame;
    }

    public IEnumerator ResetHp()
    {
        yield return new WaitUntil (()=> GameManager.Instance.isLevelLoaded);
        maxHp = GameManager.Instance.levelData.maxHp;
        currentHp = maxHp;
        hpBar.DOFillAmount(1, 0.5f).SetEase(Ease.OutQuad);
        hpText.text = currentHp.ToString();
    }
    void TakeDame()
    {
        hpUI.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 1);
        particleEffect.SetActive(true);
        StartCoroutine(DisableParticleEffect());
        currentHp--;
        hpBar.DOFillAmount((float)currentHp / maxHp, 0.5f).SetEase(Ease.OutQuad);
        hpText.text = currentHp.ToString();

        if (currentHp <= 0)
        {
            EventManager.Instance.OnGameOver?.Invoke(false);
        }
        
    }

    private IEnumerator DisableParticleEffect()
    {
        yield return new WaitForSeconds(1f);
        particleEffect.SetActive(false);
    }
}
