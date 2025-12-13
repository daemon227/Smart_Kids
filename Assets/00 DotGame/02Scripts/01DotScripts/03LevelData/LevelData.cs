using System.Collections;
using System.Collections.Generic;
using Inwave.DongA.DotPuzzle.Entity;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public int levelId;
    public List<StepData> allStep = new List<StepData>();
    public int maxHp;
    public int insectLifeTime;
    public List<Polygon> GetAllPolygons()
    {
        var result = new List<Polygon>();
        foreach (var item in allStep)
        {
            result.AddRange(item.polygons);
        }
        return result;
    }
}

[System.Serializable]
public class StepData
{
    public int stepId;
    public List<Polygon> polygons;

    public StepData()
    {
    }

    public StepData(List<Polygon> polygons)
    {
        this.polygons = polygons;
    }
}