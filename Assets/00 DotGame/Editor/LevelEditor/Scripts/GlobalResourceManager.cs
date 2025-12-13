using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DotPuzzle.Editor
{
    public static class AssetLoader
    {
        public static List<T> LoadAllAssetsInFolder<T>(string folderPath) where T : UnityEngine.Object
        {
            var assets = new List<T>();

            // Get all GUIDs (asset references) in folder
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { folderPath });

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                    assets.Add(asset);
            }

            return assets;
        }
    }
    public class GlobalResourceManager
    {
        private static FlowerSO _flowerSo;
        public static void LoadFlowerSO()
        {
            string path = "Assets\\00 Game\\05SOs\\FillDataSO\\FlowerDatas\\FlowerDataWithDropShadow.asset";
            _flowerSo = AssetDatabase.LoadAssetAtPath<FlowerSO>(path);
        }

        public static int GetFlowerDataCount()
        {
            //if (_flowerSo == null)
            //{
            //    LoadFlowerSO();
            //}

            return _flowerSo.flowers.Count;
        }

        public static FlowerData GetFlowerData(int id)
        {
            //if (_flowerSo == null)
            //{
            //    LoadFlowerSO();
            //}

            if (_flowerSo != null)
            {
                if (id >= 1 && id <= _flowerSo.flowers.Count)
                {
                    return _flowerSo.flowers[id-1];
                }
            }
            return _flowerSo.flowers[0];
        }

        public static List<FlowerData> Flowers => _flowerSo.flowers;
    }
}

