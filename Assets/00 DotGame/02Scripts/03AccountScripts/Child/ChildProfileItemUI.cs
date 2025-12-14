using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DACN.Account
{
    public class ChildProfileItemUI : MonoBehaviour
    {
        public Image childAvatar;
        public TMP_Text childName;
        public Button selectButton;

        public void SetChildProfileItem(Sprite avatarSprite, string name, int age, UnityEngine.Events.UnityAction onClickAction)
        {
            childAvatar.sprite = avatarSprite;
            childName.text = name + ", " + age.ToString() + " yrs";
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(onClickAction);
        }
    }
}

