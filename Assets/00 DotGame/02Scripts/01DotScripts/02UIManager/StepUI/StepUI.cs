
using System.Collections;
using System.Collections.Generic;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.Manager;
using TMPro;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.UI.StepUI
{
    public class StepUI : MonoBehaviour
    {
        public TextMeshProUGUI levelText;

        [Header("Layout Settings")]
        public RectTransform bgNodesRoot;   
        public RectTransform frontNodesRoot;

        public GameObject whiteNodePrefab;
        public GameObject brownNodePrefab;
        public float spacing = 80f;
        
        private int currentStep = 0;
        private List<StepNode> allNodes = new List<StepNode>();
        

        private IEnumerator Start()
        {
            GameManager.Instance.OnLevelLoadedEvent += OnLevelLoadedHandler;
            EventManager.Instance.ChangeToNewStep += OnNextStepHandle;
            
            if (GameManager.Instance.isLevelLoaded)
            {
                OnLevelLoadedHandler();
            }
            
            yield return null; 
        }

        void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnLevelLoadedEvent -= OnLevelLoadedHandler;
            }
            if (EventManager.Instance != null)
            {
                EventManager.Instance.ChangeToNewStep -= OnNextStepHandle;
            }
        }

        private void OnLevelLoadedHandler()
        {
            GenerateStepUI();
        }
        
        void GenerateStepUI()
        {
            ResetUI();

            if (GameManager.Instance.levelData == null) return;
            
            levelText.text = "Level " + (GameManager.Instance.currentLevel);
            int numberNode = GameManager.Instance.levelData.allStep.Count;
            Debug.Log("Spawn node: " + numberNode);
            
            float totalWidth = (numberNode - 1) * spacing;
            
            float startX = - totalWidth / 2f;

            for (int i = 0; i < numberNode; i++)
            {
                float posX = startX + i * spacing;
                Vector2 pos = new Vector2(posX, 0);
            
                var whiteNode = Instantiate(whiteNodePrefab, bgNodesRoot);
                whiteNode.GetComponent<RectTransform>().anchoredPosition = pos;
        
                var brownNode = Instantiate(brownNodePrefab, frontNodesRoot);
                brownNode.GetComponent<RectTransform>().anchoredPosition = pos;
                
                
                StepNode stepNode = brownNode.GetComponent<StepNode>();
                if (stepNode == null) stepNode = whiteNode.GetComponent<StepNode>();

                if (stepNode != null)
                {
                    allNodes.Add(stepNode);
                    brownNode.SetActive(false);
                }
            }

            if (allNodes.Count > 0)
            {
                allNodes[0].gameObject.SetActive(true);
                allNodes[0].SetStepNodeStatus(false);
            }
        }
    
        private void ResetUI()
        {
            foreach (Transform child in bgNodesRoot)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in frontNodesRoot)
            {
                Destroy(child.gameObject);
            }
            allNodes.Clear();
            
            currentStep = 0;
            
            
        }

        public void OnNextStepHandle()
        {
            Debug.Log("Next step");
            
            if (currentStep >= allNodes.Count) return;

            allNodes[currentStep].gameObject.SetActive(true);
            allNodes[currentStep].SetStepNodeStatus(true);
            
            if (currentStep < allNodes.Count - 1)
            {
                currentStep++;
                //UpdateFillBar();
                allNodes[currentStep].gameObject.SetActive(true);
                allNodes[currentStep].SetStepNodeStatus(false);
            }
        }

        /*private void UpdateFillBar()
        {
            int nodeCount = allNodes.Count;
            if (nodeCount <= 1) return;

            if (nodeCount == 2)
            {
                fillBarImage.DOFillAmount(1, 1f).SetEase(Ease.OutCubic); 
                return;
            }
            float value = (1f/(nodeCount - 1)) * currentStep;
            fillBarImage.DOFillAmount(value, 1f).SetEase(Ease.OutCubic); 
        }*/

    }
}


