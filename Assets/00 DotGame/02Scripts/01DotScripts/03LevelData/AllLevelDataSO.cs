using System.Collections;
using System.Collections.Generic;
using Inwave.DongA.DotPuzzle.Data;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.LeveData
{
    [CreateAssetMenu(fileName = "NewAllGameLevel", menuName = "DotPuzzle/All Level Data")]
    public class AllLevelDataSO : ScriptableObject
    {
        [SerializeField] private int allLevelId;
        public AllLevelData[] allLevelData;
    
    }
    [System.Serializable]
    public class AllLevelData
    {
        public int LevelId;
        public TextAsset LevelData;
        public InsectSpawnSO InsectSpawnSo;
    }
}

 