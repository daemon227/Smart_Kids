using System;
using System.Collections;
using System.Collections.Generic;
using Inwave.DongA.DotPuzzle.Manager;
using UnityEngine;

public class BackgroundSetting : MonoBehaviour
{
    private PolygonManager polygonManager;
    private IEnumerator Start()
    {
        polygonManager = new PolygonManager();
        yield return new WaitUntil(() => GameManager.Instance.isLevelLoaded);
        GameManager.Instance.OnLevelLoadedEvent += OnlevelLoadedHandle;
        SetBackgroundPosition();
    }

    void OnDisable()
    {
        GameManager.Instance.OnLevelLoadedEvent -= OnlevelLoadedHandle;
    }

    private void OnlevelLoadedHandle()
    {
        var center =  polygonManager.GetGlobalCentroid();
        transform.position = new Vector3(center.x, center.y, 0);
    }

    private void SetBackgroundPosition()
    {
        var center =  polygonManager.GetGlobalCentroid();
        transform.position = new Vector3(center.x, center.y, 0);
    }
}
    