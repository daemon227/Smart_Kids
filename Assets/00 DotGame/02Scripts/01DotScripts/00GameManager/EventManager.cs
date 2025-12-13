using Inwave.DongA.DotPuzzle.Entity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.Event
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;
        // Game Events
        public Action<Polygon> OnGameStart;
        public Action<Polygon> OnPolygonComplete;
        public Action FillAllPolygons;
        public Action<Polygon> FilledOnePolygon;
        public Action<Polygon> OnStepComplete;
        public Action ChangeToNewStep;
        public Action OnTakeDame;
        
        // Insect Events
        public Action OnInsectSpawned;
        public Action OnEndTurn;

        // UI Events
        public Action<IInsect> OnLoseByBug;
        public Action<Polygon> OnFillWiltedFlower;
        public Action OnAllPolygonsFilled;
        public Action<bool> OnGameOver;
        
        
        public Action<List<Polygon>> ShowLevelData;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
