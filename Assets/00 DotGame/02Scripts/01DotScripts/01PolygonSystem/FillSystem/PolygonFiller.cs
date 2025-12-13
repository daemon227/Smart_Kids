using DG.Tweening;
using Inwave.DongA.DotPuzzle.Entity;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.Data; // Nhớ using namespace chứa SO
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.Manager
{
    public class PolygonFiller : MonoBehaviour
    {
        [Header("Theme Configuration")]
        public bool fillRandom;
        public FillerThemeSO theme; 
        public FlowerSO flowerData;
        private PolygonManager polygonManager;
        private GameObject prefab;

        private List<Sprite> flowerSprites = new List<Sprite>();
        private void Start()
        {
            if (theme == null)
            {
                Debug.LogError("Chưa gán FillerThemeSO vào PolygonFiller!");
                return;
            }
            polygonManager = new PolygonManager();
            prefab = theme.prefabs;
            
            EventManager.Instance.OnPolygonComplete += Fill;
            EventManager.Instance.OnFillWiltedFlower += FillWiltedFlower;

            //
            flowerSprites = flowerData.flowers.Select(f => f.sprite).ToList();
        }

        private void OnDisable()
        {
            EventManager.Instance.OnPolygonComplete -= Fill;
            EventManager.Instance.OnFillWiltedFlower -= FillWiltedFlower;
        }

        public void Fill(Polygon polygon)
        {
            if (theme == null) return;
            if (theme.fillDatas.Count > 0)
            {
                foreach (var fillData in theme.fillDatas)
                {
                    StartCoroutine(FillPolygon(polygon, fillData, fillData.hasAnimation));
                }
                EventManager.Instance.FilledOnePolygon?.Invoke(polygon);
            }
        }
 
        private IEnumerator FillPolygon(Polygon poly, FillData fillData, bool hasAnim)
        {
            Rect bounds = GetBounds(poly);
            Vector2 centroid = poly.GetCentroid();

            Vector2 globalCentroid = polygonManager.GetGlobalCentroid();
            float distToGlobal = Vector2.Distance(centroid, globalCentroid);
            
            float zOffset = distToGlobal * theme.globalZMultiplier + fillData.zOffset;

            Sprite flower = fillData.sprite;
            if (fillData.id == 0)
            {
                flower = GetFLowerSprite(poly);
                //flower = GetRandomFlower();
            }
            // Dùng theme.flower
            StartCoroutine(FillEdges(poly, flower, fillData, zOffset));
            yield return null;
            for (float x = bounds.xMin; x <= bounds.xMax; x += Random.Range(fillData.stepRange.x, fillData.stepRange.y))
            {
                for (float y = bounds.yMin; y <= bounds.yMax; y += Random.Range(fillData.stepRange.x, fillData.stepRange.y))
                {
                    Vector3 p = new Vector2(x, y);

                    if (!IsInsidePolygon(p, poly, fillData.inset))
                        continue;
                    
                    SpawnObject(poly, p, flower,fillData,hasAnim,zOffset);

                    if (hasAnim)
                    {
                        yield return new WaitForSeconds(theme.spawnDelay);
                    }
                }
            }
        }

        // SPAWN
        private void SpawnObject(Polygon poly, Vector3 pos, Sprite sprite,FillData fillData, bool hasAnim,float zOffset)
        {
            GameObject obj = Instantiate(prefab, pos, Quaternion.identity, poly.transform);
            obj.GetComponent<SpriteRenderer>().sprite = sprite;
            Vector2 centroid = poly.GetCentroid();
            float dist = Vector2.Distance(pos, centroid);
            obj.transform.position = new Vector3(pos.x, pos.y, dist * theme.zMultiplier + zOffset);
            
            if (!hasAnim)
            {
                obj.transform.localScale = Vector3.one * GetRandomScaleByPercent(fillData.percentRange, fillData.scaleRange);
            }
            else
            {
                obj.transform
                    .DOScale(Vector3.one * GetRandomScaleByPercent(fillData.percentRange, fillData.scaleRange), theme.scaleTime)
                    .From(Vector3.zero)
                    .SetEase(Ease.OutBack);
                poly.flowerObjects.Add(obj);
            }
            // Random rotation
            float rot = theme.angleRotation;  
            obj.transform.Rotate(0, 0, Random.Range(-rot, rot));
        }


        private IEnumerator FillEdges(Polygon poly, Sprite sprite, FillData fillData ,float zOffset)
        {
            // Debug.Log("fill edge");
            foreach (var e in poly.edges)
            {
                Vector2 a = e.PosA;
                Vector2 b = e.PosB;

                Vector2 dir = (b - a).normalized;
                Vector2 normal = ComputeInsetNormal(a, b, poly);

                Vector2 ia = a + normal * fillData.lineInset;
                Vector2 ib = b + normal * fillData.lineInset;

                float len = Vector2.Distance(ia, ib);

                for (float d = 0; d <= len; d += fillData.lineStep)
                {
                    Vector2 p = ia + dir * d;

                    if (IsInsidePolygon(p, poly,  fillData.lineInset * 0.995f))
                        SpawnObject(poly, p, sprite, fillData, fillData.hasAnimation, zOffset);
                    if (fillData.hasAnimation)
                    {
                        yield return new WaitForSeconds(theme.spawnDelay);
                    }
                }
            }
        }

        public Sprite GetFLowerSprite(Polygon polygon)
        {
            if (polygon.flowerId == 0)
            {
                return GetRandomFlower();
            }
            var flowerSprite = flowerData.flowers
                .FirstOrDefault(f => f.id == polygon.flowerId);
            if (flowerSprite != null)
            {
                return flowerSprite.sprite;
            }
            return theme.defaultSprite;
        }

        public void FillWiltedFlower(Polygon polygon)
        {
            if (polygon == null||polygon.flowerObjects.Count == 0) return;
            StartCoroutine(ChangeWiltedFlowerSprite(polygon));
        }
        public IEnumerator ChangeWiltedFlowerSprite(Polygon polygon)
        {
            foreach (var flower in polygon.flowerObjects)
            {
                flower.transform.DOKill();
                flower.GetComponent<SpriteRenderer>().sprite = theme.wiltedSprite;
                yield return new WaitForSeconds(theme.wiltedDelaySpawn);
            }
        }
        
        #region Helper

        private Sprite GetRandomFlower()
        {
            var flower = flowerSprites[Random.Range(0, flowerSprites.Count)];
            flowerSprites.Remove(flower);
            if (flowerSprites.Count == 0) flowerSprites = flowerData.flowers.Select(f => f.sprite).ToList();
            return flower;
        }
        
        private float GetRandomScaleByPercent(Vector3 percentRange, Vector2 scaleRange)
        {
            float r = Random.value;
            float min = scaleRange.x;
            float max = scaleRange.y;
            float total = max - min;

            float sMax = min + total * percentRange.x;
            float mMax = sMax + total * percentRange.y;

            if (r < percentRange.x) return Random.Range(min, sMax);
            if (r < percentRange.x + percentRange.y) return Random.Range(sMax, mMax);
            return Random.Range(mMax, max);
        }
        private Vector2 ComputeInsetNormal(Vector2 a, Vector2 b, Polygon poly)
        {
            Vector2 dir = (b - a).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x);

            Vector2 mid = (a + b) * 0.5f;
            Vector2 test = mid + normal * 0.05f;

            if (!poly.IsInsidePolygonBase(test))
                return -normal;
            return normal;
        }

        private Rect GetBounds(Polygon poly)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (var v in poly.vertices)
            {
                Vector2 p = v.transform.position;
                minX = Mathf.Min(minX, p.x);
                maxX = Mathf.Max(maxX, p.x);
                minY = Mathf.Min(minY, p.y);
                maxY = Mathf.Max(maxY, p.y);
            }

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        private bool IsInsidePolygon(Vector2 p, Polygon poly, float inset)
        {
            if (!poly.IsInsidePolygonBase(p))
                return false;

            foreach (var e in poly.edges)
            {
                if (DistancePointToSegment(p, e.PosA, e.PosB) < inset)
                    return false;
            }
            return true;
        }

        private float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ap = p - a, ab = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / ab.sqrMagnitude);
            Vector2 proj = a + ab * t;
            return Vector2.Distance(p, proj);
        }
        #endregion
    }
}