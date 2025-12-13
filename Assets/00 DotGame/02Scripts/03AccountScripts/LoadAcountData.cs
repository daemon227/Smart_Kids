using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DACN.Account
{
    public class LocalDataManager : MonoBehaviour
    {
        public static LocalDataManager Instance;
        public AppData data;
        string path;
        public UserAccount currentUser;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            path = Path.Combine(Application.persistentDataPath, "user_data.json");
            Load();
        }

        void Load()
        {
            if (!File.Exists(path))
            {
                data = new AppData();
                Save();
                return;
            }

            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<AppData>(json);
        }

        public void Save()
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

    }
}

