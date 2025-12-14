using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DACN.Account
{
    public class AvatarService : MonoBehaviour
    {
        public static AvatarService Instance;
        public List<Sprite> avatarSprites;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        // ===== GET AVATAR SPRITE BY ID =====
        public Sprite GetAvatarSpriteById(int avatarId)
        {
            if(avatarId < 0 || avatarId >= Instance.avatarSprites.Count)
            {
                Debug.LogWarning($"Avatar ID {avatarId} is out of range. Returning default avatar.");
                return Instance.avatarSprites[0]; // Return default avatar
            }
            return Instance.avatarSprites[avatarId];
        }
    }
}
