using System.Collections;
using System.Collections.Generic;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.Manager;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.InsectSystem
{
    public class InsectManager : MonoBehaviour
    {
        [HideInInspector]public float bugLifeTime = 30f;
        [HideInInspector] public float currentLifeTime;
        [HideInInspector] public List<GameObject> insects;
        public bool isTimerActive = false; 
        public GameObject clickParticle;
        private bool isEndTurn;
        
        private InsectWarningManager insectWarningManager;
        void Start()
        {
            currentLifeTime = bugLifeTime;
            GameManager.Instance.OnLevelLoadedEvent += ResetForNewLevel;
            EventManager.Instance.OnAllPolygonsFilled += KillAllInsects;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelLoadedEvent -= ResetForNewLevel;
                EventManager.Instance.OnAllPolygonsFilled -= KillAllInsects;
            }
        }
        
        private void ResetForNewLevel()
        {
            int currentLevel = GameManager.Instance.currentLevel;
            bugLifeTime = GameManager.Instance.allLevelDataSO.allLevelData[currentLevel - 1].InsectSpawnSo.insectLifeTime;
            if (insects != null)
            {
                foreach (var insect in insects)
                {
                    if (insect != null)
                    {
                        insect.GetComponent<IInsect>().DeadBySpray();
                    }
                }
                insects.Clear();
            }
            else
            {
                insects = new List<GameObject>();
            }
            ResetLifeTime();
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.isGameOver) return;
            
            if (GameManager.Instance.canInteract)
            {
                HandleInput();
            }

            ManageGameState();
            
            if (insects.Count > 0 && !isEndTurn && !GameManager.Instance.isPaused)
            {

                if (isTimerActive)
                {
                    currentLifeTime -= Time.deltaTime;
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var insect = GetInsect(mousePos, 0.4f);
                if (insect != null)
                {
                    StartCoroutine(ShowInteractParticle(insect.transform.position));
                    
                    var insectComponent = insect.GetComponent<IInsect>();
                    insects.Remove(insect);
                    insectComponent.Dead();
                    CheckInsects();
                }
            }
        }

        private GameObject GetInsect(Vector3 position, float radius)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);
            foreach (var hit in hits)
            {
                if (hit.GetComponent<IInsect>() != null) return hit.gameObject;
            }
            return null;
        }

        private void ManageGameState()
        {
            if (isEndTurn) return;
            if (currentLifeTime <= 0 && insects.Count > 0)
            {
                Debug.Log("Game Over");
                var insect = insects[0];
                EventManager.Instance.OnLoseByBug?.Invoke(insect.GetComponent<IInsect>());
                isEndTurn = true;
            }
        }

        public void CheckInsects()
        {
            if (insects.Count <= 0)
            {
                GameManager.Instance.hasInsects = false;
                EventManager.Instance.OnEndTurn?.Invoke();
            }
        }
        
        public void ResetLifeTime()
        {
            isEndTurn = false;
            currentLifeTime = bugLifeTime;
            GameManager.Instance.hasInsects = false;
            isTimerActive = false; 
        }
        
        public void StartTimer()
        {
            isTimerActive = true;
        }

        public void KillAllInsects()
        {
            if (insects != null && insects.Count > 0)
            {
                foreach (var insect in insects)
                {
                    if (insect != null)
                    {
                        insect.GetComponent<IInsect>().DeadBySpray();
                    }
                }
                insects.Clear();
            }
        }

        private IEnumerator ShowInteractParticle(Vector3 position)
        {
            clickParticle.SetActive(true);
            clickParticle.transform.position = position;
            yield return new WaitForSeconds(0.25f);
            clickParticle.SetActive(false);
        }
    }
    
}
