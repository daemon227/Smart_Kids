
using DG.Tweening;
using UnityEngine;

public class Vertice : MonoBehaviour
{
    public Sprite defaultSprite;
    public Sprite clickedSprite;
    public float defaultScale = 0.02f;
    public float clickedScale = 0.03f;
    public float zoomScaleTime = 0.1f;
    
    private int maxVerticeCanConnect = 0;
    private int currentConnectCount = 0;
    public int MaxVerticeCanConnect { get => maxVerticeCanConnect; set => maxVerticeCanConnect = value; }
    public int CurrentConnectCount { get => currentConnectCount; set => currentConnectCount = value; }

    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public bool IsCanConnect()
    {
        return MaxVerticeCanConnect > CurrentConnectCount;
    }
    public void SetVerticeScale(float scale)
    {
        transform.localScale = new Vector3 (scale, scale, 0);
    }

    public void ChangeVerticeSprite(bool onClicked = true, bool isError = true)
    {
        var newSprite = defaultSprite;
        var scale = defaultScale;
        var newColor = isError ? Color.red : Color.white;
        if (onClicked)
        {
            newSprite = clickedSprite;
            scale = clickedScale;
            newColor = Color.white;
        }
        spriteRenderer.color = newColor;
        spriteRenderer.sprite = newSprite;
        transform.DOScale(Vector3.one * scale, zoomScaleTime)
            .From(Vector3.zero)
            .SetEase(Ease.OutBack).OnComplete(() =>
            {
                spriteRenderer.color = Color.white;
            });

        transform.DORotate(
            new Vector3(0, 0, 360f),
            zoomScaleTime,
            RotateMode.FastBeyond360
        );
    }

}
