
using System.Collections.Generic;
using Inwave.DongA.DotPuzzle.Entity;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.Manager;
using TMPro;
using UnityEngine;

public class VerticeCountTextManager : MonoBehaviour
{
    public GameObject parentObject;
    public TextMeshProUGUI textPrefab;

    private StepManager stepManager;

    private void Start()
    {
        EventManager.Instance.ShowLevelData += SpawnText;
    }
    private void OnDisable()
    {
        EventManager.Instance.ShowLevelData -= SpawnText;
    }
    public void SpawnText(List<Polygon> polygons)
    {
        foreach (Transform child in parentObject.transform)
        {
            Destroy(child.gameObject);
        }
        if (polygons == null)
        {
            return;
        }
        foreach (var polygon in polygons)
        {
            var textObject = Instantiate(textPrefab, parentObject.transform);
            textObject.transform.position = polygon.GetCentroid();
            textObject.text = polygon.vertices.Count.ToString();
            polygon.VerticeCountText = textObject;
        }
    }
}
