
using System.Collections.Generic;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.Data
{
    [CreateAssetMenu(fileName = "NewInsectSpawnData", menuName = "DotPuzzle/Insect Spawn")]
    public class InsectSpawnSO : ScriptableObject
    {
        [Header("Insects Spawn Settings")]
        public float insectLifeTime = 10f;
        public Vector2 insectsPerturnRange = new Vector2(2, 5);
        public Vector2 scaleRange = new Vector2(0.06f, 0.08f);
        public List<GameObject> pestInsects;
        public List<GameObject> beneficialInsects;
        public float spawnDistance = 3f;
        public float spawnDelay = 0.5f;
        public Vector2 delaySpawnRangePerTurn = new Vector2(30, 50);
        public float spawnPercent = 15f;
    }
}

