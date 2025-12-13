using System.Collections;
using System.Collections.Generic;
using System.IO;
using Inwave.DongA.DotPuzzle.Entity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class SaveLoadData : MonoBehaviour
{
    public static SaveLoadData Instance { get; private set; }

    [Header("Prefabs Needed for Loading")]
    [SerializeField] private Vertice verticePrefab;
    [SerializeField] private Polygon polygonPrefab;
    [SerializeField] private LevelData levelDataPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    #region Save Logic

    public void SaveLevelToJson(LevelData levelData, string fileName)
    {
        LevelDataSave saveData = new LevelDataSave();
        saveData.levelId = levelData.levelId;
        saveData.maxHp = levelData.maxHp;
        saveData.insectLifeTime = levelData.insectLifeTime;

        Dictionary<Vertice, int> verticeToIdMap = MapAndSaveVertices(levelData, saveData);

        SaveStepsAndPolygons(levelData, saveData, verticeToIdMap);

        WriteDataToFile(saveData, fileName);
    }

    private Dictionary<Vertice, int> MapAndSaveVertices(LevelData levelData, LevelDataSave saveData)
    {
        Dictionary<Vertice, int> verticeToIdMap = new Dictionary<Vertice, int>();
        int nextId = 0;

        foreach (var step in levelData.allStep)
        {
            foreach (var poly in step.polygons)
            {
                if (poly == null) continue;

                foreach (var vert in poly.vertices)
                {
                    if (vert != null && !verticeToIdMap.ContainsKey(vert))
                    {
                        verticeToIdMap.Add(vert, nextId);

                        VertexDataSave vData = new VertexDataSave
                        {
                            id = nextId,
                            position = vert.transform.position
                        };
                        saveData.allUniqueVertices.Add(vData);

                        nextId++;
                    }
                }
            }
        }
        return verticeToIdMap;
    }

    private void SaveStepsAndPolygons(LevelData levelData, LevelDataSave saveData, Dictionary<Vertice, int> verticeToIdMap)
    {
        foreach (var step in levelData.allStep)
        {
            StepDataSave stepSave = new StepDataSave { stepId = step.stepId };

            foreach (var poly in step.polygons)
            {
                if (poly == null) continue;

                PolygonDataSave polySave = new PolygonDataSave
                {
                    polygonId = poly.polygonId,
                    flowerId = poly.flowerId
                };

                foreach (var vert in poly.vertices)
                {
                    if (vert != null && verticeToIdMap.TryGetValue(vert, out int id))
                    {
                        polySave.verticeIds.Add(id);
                    }
                }
                stepSave.polygons.Add(polySave);
            }
            saveData.steps.Add(stepSave);
        }
    }

    #endregion

    #region  Load Logic
    
    public void LoadLevelAndSpawn(string fileName, Transform parentContainer, Action<LevelData> onLoaded)
    {
        StartCoroutine(LoadLevelAndSpawnCoroutine(fileName, parentContainer, onLoaded));
    }
    
    // Load level data from JSON content
    public void LoadLevelFromContent(string jsonContent, Transform parentContainer, Action<LevelData> onLoaded)
    {
        LevelDataSave data = JsonUtility.FromJson<LevelDataSave>(jsonContent);

        if (data == null)
        {
            Debug.LogError("Failed to parse level data from JSON content.");
            onLoaded?.Invoke(null);
            return;
        }

        SpawnLevelFromSaveData(data, parentContainer, onLoaded);
    }

    private void SpawnLevelFromSaveData(LevelDataSave data, Transform parentContainer, Action<LevelData> onLoaded)
    {
        LevelData newLevelData = Instantiate(levelDataPrefab, parentContainer);
        newLevelData.levelId = data.levelId;
        newLevelData.maxHp = data.maxHp;
        newLevelData.insectLifeTime = data.insectLifeTime;
        newLevelData.name = $"Level_{data.levelId}";

        Dictionary<int, Vertice> idToVerticeMap = ReconstructVertices(data.allUniqueVertices, newLevelData.transform);
        ReconstructStepsAndPolygons(data, newLevelData, idToVerticeMap);

        onLoaded?.Invoke(newLevelData);
    }

    private IEnumerator LoadLevelAndSpawnCoroutine(string fileName, Transform parentContainer, Action<LevelData> onLoaded)
    {
        LevelDataSave data = null;
        yield return StartCoroutine(LoadDataFromAddressableCoroutine(fileName, loadedData => data = loadedData));

        if (data == null)
        {
            Debug.LogError($"Failed to load level data from file: {fileName}");
            onLoaded?.Invoke(null);
            yield break;
        }

        // Tái sử dụng logic spawn
        SpawnLevelFromSaveData(data, parentContainer, onLoaded);
    }

    private Dictionary<int, Vertice> ReconstructVertices(List<VertexDataSave> vertexDataList, Transform parent)
    {
        Dictionary<int, Vertice> idToVerticeMap = new Dictionary<int, Vertice>();
        GameObject verticesContainer = new GameObject("Vertices");
        verticesContainer.transform.SetParent(parent);

        foreach (var vData in vertexDataList)
        {
            Vertice newVert = Instantiate(verticePrefab, vData.position, Quaternion.identity, verticesContainer.transform);
            newVert.name = $"Vertice_{vData.id}";
            idToVerticeMap.Add(vData.id, newVert);
        }
        return idToVerticeMap;
    }

    private void ReconstructStepsAndPolygons(LevelDataSave data, LevelData levelDataObj, Dictionary<int, Vertice> idToVerticeMap)
    {
        GameObject polygonsContainer = new GameObject("Polygons");
        polygonsContainer.transform.SetParent(levelDataObj.transform);

        foreach (var stepData in data.steps)
        {
            StepData newStep = new StepData
            {
                stepId = stepData.stepId,
                polygons = new List<Polygon>()
            };

            foreach (var polyData in stepData.polygons)
            {
                Polygon newPoly = Instantiate(polygonPrefab, polygonsContainer.transform);
                newPoly.polygonId = polyData.polygonId;
                newPoly.flowerId = polyData.flowerId;
                newPoly.name = $"Polygon_{polyData.polygonId}";

                // Link các đỉnh
                foreach (int vId in polyData.verticeIds)
                {
                    if (idToVerticeMap.TryGetValue(vId, out Vertice loadedVert))
                    {
                        newPoly.vertices.Add(loadedVert);
                    }
                    else
                    {
                        Debug.LogWarning($"Vertex ID {vId} missing for Polygon {newPoly.polygonId}");
                    }
                }

                // newPoly.CreateEdge(); 
                newStep.polygons.Add(newPoly);
            }
            levelDataObj.allStep.Add(newStep);
        }
    }
    #endregion

    #region IO Logic
    private void WriteDataToFile(LevelDataSave data, string fileName)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = GetSavePath(fileName);

        string folderPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        File.WriteAllText(path, json);
        Debug.Log($"Level Saved to: {path}");

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
    
    private IEnumerator LoadDataFromAddressableCoroutine(string key, Action<LevelDataSave> onLoaded)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(key);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            string json = handle.Result.text;
            LevelDataSave data = JsonUtility.FromJson<LevelDataSave>(json);
            onLoaded?.Invoke(data);
        }
        else
        {
            Debug.LogError($"Failed to load Addressable with key: {key}");
            onLoaded?.Invoke(null);
        }

        Addressables.Release(handle);
    }

    private string GetSavePath(string fileName)
    {
        if (!fileName.EndsWith(".json")) fileName += ".json";
        return Path.Combine(Application.dataPath, "00 Game/06LevelData", fileName);
    }
    #endregion
}



#region Save Data Structures
[System.Serializable]
public class LevelDataSave
{
    public int levelId;
    public int maxHp;
    public int insectLifeTime;
    public List<VertexDataSave> allUniqueVertices = new List<VertexDataSave>();
    public List<StepDataSave> steps = new List<StepDataSave>();
}

[System.Serializable]
public class StepDataSave
{
    public int stepId;
    public List<PolygonDataSave> polygons = new List<PolygonDataSave>();
}

[System.Serializable]
public class PolygonDataSave
{
    public int polygonId;
    public int flowerId;
    public List<int> verticeIds = new List<int>();
}

[System.Serializable]
public class VertexDataSave
{
    public int id;
    public Vector3 position;
}
#endregion
