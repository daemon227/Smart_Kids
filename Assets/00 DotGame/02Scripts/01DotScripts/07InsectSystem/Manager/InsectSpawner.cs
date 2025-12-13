
using Inwave.DongA.DotPuzzle.Manager;
using System.Collections;
using Inwave.DongA.DotPuzzle.Data;
using Inwave.DongA.DotPuzzle.Entity;
using Inwave.DongA.DotPuzzle.Event;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inwave.DongA.DotPuzzle.InsectSystem
{
    public class InsectSpawner : MonoBehaviour
    {
        [Header("Insects Settings")] 
        public Transform parent;
        public Transform canvasWarning;
        
        private bool hasStarted = false; 
        private PolygonManager polygonManager;
        private InsectManager insectManager;
        private InsectSpawnSO insectSpawnSo;

        void Start()
        {
            polygonManager = new PolygonManager();
            insectManager = GetComponent<InsectManager>();
            EventManager.Instance.OnPolygonComplete += StartSpawnInsect;
            EventManager.Instance.OnEndTurn += SpawnNewTurnInsect;
            
            GameManager.Instance.OnLevelLoadedEvent += ResetSpawner;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnPolygonComplete -= StartSpawnInsect;
            EventManager.Instance.OnEndTurn -= SpawnNewTurnInsect;
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelLoadedEvent -= ResetSpawner;
            }
        }

        private void ResetSpawner()
        {
            StopAllCoroutines(); 
            hasStarted = false;
            GetNewInsectSpawnData();
        }

        private void GetNewInsectSpawnData()
        {
            int currentLevel = GameManager.Instance.currentLevel;
            insectSpawnSo = GameManager.Instance.allLevelDataSO.allLevelData[currentLevel-1].InsectSpawnSo;
            if (insectSpawnSo == null) Debug.LogError("Insect Spawn Data is null");
            Debug.Log("Current Level: " + currentLevel + "");
        }
        public void StartSpawnInsect(Polygon polygon)
        {
            if (hasStarted) return;
            hasStarted = true;
            StartCoroutine(SpawnInsectCoroutine());
        }

        public void SpawnNewTurnInsect()
        {
            if(hasStarted)
            {
                StartCoroutine(SpawnInsectCoroutine());
            }
        }

        private IEnumerator SpawnInsectCoroutine()
        {
            if(GameManager.Instance.isGameOver || !GameManager.Instance.canInteract) yield break;
            float waitTime = Random.Range(insectSpawnSo.delaySpawnRangePerTurn.x, insectSpawnSo.delaySpawnRangePerTurn.y);
            yield return new WaitForSeconds(waitTime);
            
            insectManager.ResetLifeTime();
            
            EventManager.Instance.OnInsectSpawned?.Invoke();
            GameManager.Instance.hasInsects = true;

            for (int i = 0; i < Random.Range(insectSpawnSo.insectsPerturnRange.x, insectSpawnSo.insectsPerturnRange.y); i++)
            {
                SpawnInsect();
                yield return new WaitForSeconds(insectSpawnSo.spawnDelay);
            }
        }
        
        private void SpawnInsect()
        {
            Vector2 spawnPosition = GetRandomPoint();
            var insectPrefab = insectSpawnSo.pestInsects[Random.Range(0, insectSpawnSo.pestInsects.Count)];
            var newInsect = Instantiate(insectPrefab, parent);
            
            newInsect.transform.localScale = Vector3.one * Random.Range(insectSpawnSo.scaleRange.x, insectSpawnSo.scaleRange.y);
            newInsect.transform.position = spawnPosition * insectSpawnSo.spawnDistance;
            
            insectManager.insects.Add(newInsect);
            
            newInsect.GetComponent<InsectWarningManager>().parentObject = canvasWarning;
        }

        private Vector2 GetRandomPoint()
        {
            Vector2 xRange = Vector2.zero;
            Vector2 yRange = Vector2.zero;

            (xRange, yRange) = polygonManager.GetGlobalPolygonRange();

            return new Vector2(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y));
        }
    }
}
