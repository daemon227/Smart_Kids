
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class SprayAnim : MonoBehaviour
{
    public GameObject sprayPrefab;
    public GameObject pariticlePrefab;
    [SerializeField] private float duration = 3f;
    private RectTransform sprayRect;
    private SkeletonGraphic skeletonGraphic;
    void Start()
    {
        sprayPrefab.SetActive(false);
        //skeletonGraphic = sprayPrefab.GetComponent<SkeletonGraphic>();
        sprayRect = sprayPrefab.GetComponent<RectTransform>();
        sprayRect.anchoredPosition = new Vector2(0, -1200f);
    }

    public IEnumerator PlaySprayAnim(System.Action callback)
    {
        sprayPrefab.SetActive(true);
        sprayPrefab.GetComponent<Image>().DOFade(1, 0f);
        sprayRect.DOAnchorPos(new Vector2(0, 0), 1f).OnComplete(() =>
        {
            SetParticlePosition();
            
            pariticlePrefab.SetActive(false);
            pariticlePrefab.SetActive(true);
            callback?.Invoke();
            
            sprayRect.DOShakeAnchorPos(
                duration: duration,
                strength: new Vector2(100f, 10),
                vibrato: 20,
                randomness: 90,
                fadeOut: true
            ).OnComplete(() =>
            {
                sprayPrefab.GetComponent<Image>().DOFade(0, 0.5f);
                sprayRect.anchoredPosition = new Vector2(0, -1200f);
            });
            //skeletonGraphic.AnimationState.SetAnimation(0, "shake", false);
            
        });
        yield return new WaitForSeconds(duration);
        sprayPrefab.SetActive(false);
        //pariticlePrefab.SetActive(false);
    }

    private void SetParticlePosition()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            Camera.main, 
            sprayRect.position
        );

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, 10f)
        );
        
        pariticlePrefab.transform.position = worldPos;
    }

}
