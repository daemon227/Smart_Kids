
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public GameObject cloud;
    [SerializeField] private Vector2 spawnTimeRange = new Vector2(15, 30);
    [SerializeField] private RectTransform endPoint;

    private RectTransform rectTransform;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        rectTransform = cloud.GetComponent<RectTransform>();
        CloundMovement();
    }

    private void CloundMovement()
    {
        rectTransform.DOAnchorPos(endPoint.anchoredPosition, 5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
