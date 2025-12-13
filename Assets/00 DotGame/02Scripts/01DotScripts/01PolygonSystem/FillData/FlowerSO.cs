using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flower", menuName = "ScriptableObjects/Flower", order = 1)]
public class FlowerSO: ScriptableObject
{
    public List<FlowerData> flowers;
}
[System.Serializable]
public class FlowerData
{
    public int id;
    public Sprite sprite;
}