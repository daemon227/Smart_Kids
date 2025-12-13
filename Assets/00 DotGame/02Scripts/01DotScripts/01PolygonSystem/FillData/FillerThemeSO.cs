using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inwave.DongA.DotPuzzle.Data
{
    [CreateAssetMenu(fileName = "NewFillerTheme", menuName = "DotPuzzle/Filler Theme")]
    public class FillerThemeSO : ScriptableObject
    {
        [Header("--- Flower Settings ---")]
        public List<FillData> fillDatas;
        
        [Header("--- General Settings ---")]
        public GameObject prefabs;
        public Sprite defaultSprite;
        public Sprite wiltedSprite;
        public float scaleTime = 1f;
        public float spawnDelay = 0.02f;
        public float angleRotation = 25f;

        public float wiltedDelaySpawn = 0.04f;
        [Header("--- Depth Settings ---")]
        public float zMultiplier = 1f;
        public float globalZMultiplier = 0.5f;
    }
    
    // Class chứa cấu hình Hoa
    [System.Serializable]
    public class FillData
    {
        public int id;
        public string displayName;
        
        public Sprite sprite;
        public bool hasAnimation = false;
        [Header("Fill At Center")]
        public Vector2 stepRange = new Vector2(0.02f, 0.03f);
        public float inset = 0.22f;
        
        [Header("Fill At Line")]
        public float lineStep = 0.02f;
        public float lineInset = 0.02f;
        
        [Header("Fill At Random Point")]
        public float zOffset = 5f;
        public Vector2 scaleRange = new Vector2(0.03f, 0.05f);
        public Vector3 percentRange = new Vector3(0.3f, 0.5f, 0.2f);
    }
    
}