
using DG.Tweening;
using Inwave.DongA.DotPuzzle.Entity;
using Inwave.DongA.DotPuzzle.Manager;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inwave.DongA.DotPuzzle.InsectSystem
{
    public class LaydyBug : MonoBehaviour, IInsect
    {
        public InsectSO insectData;
        public GameObject shadow;
        
        private SkeletonAnimation skeletonAnimation;
        
        private bool isMoving = true;
        private bool isDead = false;
        private bool isCrawling = false;       
        private Vector3 deathVelocity;     
        
        private Vector2 target = Vector2.zero;
        private Vector2 crawlCenter = Vector2.zero;
        private Vector2 crawlDirection = Vector2.zero;
        private float crawlTimer = 0f;
        private float crawlDuration = 0f;
        private float directionChangeTimer = 0f;
        
        private Vector2 startPosition;     
        private Vector2 currentPivotPos;   
        private float totalDistance;
        
        private float spiralDirection;
        private float currentSpiralAngle;
        
        private PolygonManager polygonManager;
        private IInsect insectImplementation;

        public Polygon targetPolygon { get; set; }
        public bool IsMoving() => isMoving;
        
        public Vector3 GetPosition()
        {
            return transform.position;
        }

        void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            polygonManager = new PolygonManager();
            SetNewTarget(); 
        }
        private void Update()
        {
            if (GameManager.Instance.isPaused|| GameManager.Instance.isGameOver) return;
            if (isMoving && !isDead)
            {
                Fly();
            }

            if (isCrawling && !isDead)
            {
                Crawl();
            }

            if (isDead)
            {
                HandleKickEffect();
            }
        }
        
        #region Flying Methods
        private void SetNewTarget()
        {
            target = GetRandomTarget();
            
            startPosition = transform.position;
            currentPivotPos = startPosition;
            totalDistance = Vector2.Distance(startPosition, target);
            
            spiralDirection = Random.value > 0.5f ? 1f : -1f;
            
            currentSpiralAngle = Random.Range(0f, Mathf.PI * 2);
        
            if (target != Vector2.zero)
            {
                isMoving = true;
            }
        }
        

        private void HandleKickEffect()
        {
            deathVelocity.y -= insectData.gravity * Time.deltaTime;
            
            transform.position += deathVelocity * Time.deltaTime;
            
            transform.Rotate(0, 0, insectData.deathSpinSpeed * Time.deltaTime);
            
            if (transform.position.y < -15f)
            {
                Destroy(gameObject);
            }
        }

        public Vector2 GetRandomTarget()
        {
            if (polygonManager == null || GameManager.Instance == null || GameManager.Instance.levelData.GetAllPolygons() == null) 
                return transform.position;

            var polygon = polygonManager.GetRandomCompletedPolygon(GameManager.Instance.levelData.GetAllPolygons());
            if (polygon == null)
            {
                return Vector2.zero;
            }

            Vector2 centroid = polygon.GetCentroid();
            bool inPolygon = false;
            var newTarget = Vector2.zero;
            int safetyCount = 0; 
            while (!inPolygon && safetyCount < 20)
            {
                safetyCount++;
                Vector2 p = centroid + Random.insideUnitCircle * Random.Range(insectData.moveRange.x, insectData.moveRange.y);
                if (polygon.IsInsidePolygonBase(p))
                {
                    inPolygon = true;
                    newTarget = p;
                }
            }
            targetPolygon = polygon;
            return inPolygon ? newTarget : centroid;
        }
        
        public void Fly()
        {
            shadow.SetActive(false);
            skeletonAnimation.AnimationName = "fly";
            
            float step = insectData.moveSpeed * Time.deltaTime;
            currentPivotPos = Vector2.MoveTowards(currentPivotPos, target, step);
            
            float distRemaining = Vector2.Distance(currentPivotPos, target);

            float currentRadius = 0f;
            if (totalDistance > 0)
            {
                // Reduce radius as it gets closer to target for smoother movement
                float progressRatio = distRemaining / totalDistance;
                currentRadius = insectData.spiralRadius * progressRatio;
            }
            
            currentSpiralAngle += Time.deltaTime * insectData.spiralSpeed * spiralDirection;
            float angle = currentSpiralAngle;

            Vector2 spiralOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * currentRadius;

            Vector2 nextPosition = currentPivotPos + spiralOffset;
        
            Vector3 direction = (Vector3)nextPosition - transform.position;
            
            if (direction != Vector3.zero)
            {
                float rotAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(rotAngle - 90, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
            }
            
            transform.position = nextPosition;
            
            if (distRemaining < 0.1f)
            {
                transform.position = target;
                isMoving = false;
                skeletonAnimation.AnimationName = "idle";
                shadow.SetActive(true);
                
                // Start crawling when landed
                StartCrawling();
            }
        }

        private void StartCrawling()
        {
            isCrawling = true;
            crawlCenter = transform.position;
            crawlDuration = Random.Range(2f, 5f);
            crawlTimer = 0f;
        }

        private void Crawl()
        {
            crawlTimer += Time.deltaTime;
            directionChangeTimer += Time.deltaTime;
            
            // Always reset duration when elapsed - keeps crawling indefinitely
            if (crawlTimer >= crawlDuration)
            {
                crawlDuration = Random.Range(1.5f, 4f);
                crawlTimer = 0f;
            }
            
            // Change direction every 0.5-1 second
            if (directionChangeTimer >= Random.Range(1f, 2f))
            {
                crawlDirection = Random.insideUnitCircle.normalized;
                directionChangeTimer = 0f;
            }
            
            // Move in consistent direction
            float crawlSpeed = insectData.moveSpeed * 0.2f;
            float crawlDistance = crawlSpeed * Time.deltaTime;
            
            Vector2 newCrawlPos = (Vector2)transform.position + crawlDirection * crawlDistance;
            
            // Keep crawling within a larger radius of landing position
            if (Vector2.Distance(newCrawlPos, crawlCenter) <= insectData.moveRange.y * 2f)
            {
                // Also check if position is inside polygon
                if (targetPolygon != null && targetPolygon.IsInsidePolygonBase(newCrawlPos))
                {
                    transform.position = newCrawlPos;
                    
                    // Rotate to face movement direction
                    if (crawlDirection != Vector2.zero)
                    {
                        float rotAngle = Mathf.Atan2(crawlDirection.y, crawlDirection.x) * Mathf.Rad2Deg;
                        Quaternion targetRotation = Quaternion.AngleAxis(rotAngle - 90, Vector3.forward);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                    }
                }
            }
        }

        
#endregion


#region Show Emote Methos

public void ShowEmote(bool isHappy)
        {
            var emoteTf = transform.GetChild(0);
            
            var sr = emoteTf.GetComponent<SpriteRenderer>();

            if (isHappy) sr.sprite = insectData.happyEmote;
            else sr.sprite = insectData.sadEmote;
            sr.gameObject.SetActive(true);
            emoteTf.localScale = new Vector2(0, 0);

            emoteTf.transform.DOScale(Vector2.one * 0.8f, 0.5f).From(Vector2.zero).SetEase(Ease.OutBack);
            emoteTf.DORotate(
                    new Vector3(0, 0, 20f),
                    0.7f
                ).From(new Vector3(0, 0, -20f))
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

        }
        
        public void HideEmote()
        {
            var emoteTf = transform.GetChild(0);
            emoteTf.transform.DOKill();
            emoteTf.gameObject.SetActive(false);
        }

        public void Dead()
        {
            if (isDead) return;

            isDead = true;
            isMoving = false;
            
            GetComponent<InsectWarningManager>().DestroyWarning();
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            
            float angleDeg = Random.Range(50f, 110f);
            float angleRad = angleDeg * Mathf.Deg2Rad;
            
            Vector2 randomDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            deathVelocity = randomDir * insectData.kickForce;
            
            insectData.deathSpinSpeed = insectData.deathSpinSpeed * (Random.value > 0.5f ? 1 : -1);
        }

        public void DeadBySpray()
        {
            isDead = true;
            isMoving = false;
            
            GetComponent<InsectWarningManager>().DestroyWarning();
            
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            
            deathVelocity.y -= insectData.gravity * Time.deltaTime;
            Destroy(gameObject, 3f);
        }

#endregion

        
    }

}