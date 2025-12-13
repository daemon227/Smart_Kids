using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Inwave.DongA.DotPuzzle.Entity;
using Inwave.DongA.DotPuzzle.Event;
using UnityEngine;

public class FlowerAnim : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float angleStrength = 10f;       // Góc xoay tối đa
    [SerializeField] private float duration = 1f;
    private void Start()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.FilledOnePolygon += OnPolygonFilled;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.FilledOnePolygon -= OnPolygonFilled;
    }

    public void OnPolygonFilled(Polygon polygon)
    {
        StartCoroutine(AnimatePolygonPop(polygon));
    }
        public IEnumerator AnimatePolygonPop(Polygon polygon )
        {
            yield return new WaitForSeconds(0.5f);
            if (polygon == null) yield break;

            var centerPoint = polygon?.GetCentroid() ?? Vector3.zero;
            foreach (var flower in polygon.flowerObjects)
            {
                if (flower == null) continue;

                if (Random.value > 0.5f) continue;
                float dist = Vector3.Distance(flower.transform.position, centerPoint);
                
                flower.transform.DORotate(new Vector3(0, 0, Random.Range(-angleStrength, angleStrength)),duration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetLink(flower.gameObject);
            }
        }
    }
