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
        public ChildAccount currentChild;
        public AudioSource bgmSource;
        public AudioClip bgmClip;


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
            Debug.Log(Application.persistentDataPath);
            Load();
            
            // Initialize currentUser and currentChild as null
            currentUser = null;
            currentChild = null;

            // Setup AudioSource for BGM
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.clip = bgmClip;
            bgmSource.Play();
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

        /// <summary>
        /// Set current child account
        /// </summary>
        public void SetCurrentChild(ChildAccount child)
        {
            currentChild = child;
            if (child != null)
            {
                Debug.Log($"Set current child to: {child.name}");
            }
        }

        /// <summary>
        /// Clear current user session
        /// </summary>
        public void Logout()
        {
            currentUser = null;
            currentChild = null;
            Debug.Log("User logged out");
        }

    }
}