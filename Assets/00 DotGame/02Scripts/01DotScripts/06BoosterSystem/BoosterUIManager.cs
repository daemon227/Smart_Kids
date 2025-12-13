using Inwave.DongA.DotPuzzle.Manager;
using Inwave.DongA.DotPuzzle.Event;
using UnityEngine;
using UnityEngine.UI;

namespace Inwave.DongA.DotPuzzle.UIManager
{
    public class BoosterUIManager : MonoBehaviour
    {
        public Button fillAllPolygonButton;

        private void Start()
        {
            #if !UNITY_EDITOR
            fillAllPolygonButton.gameObject.SetActive(false);
            #endif
            fillAllPolygonButton.onClick.AddListener(OnFillAllPolygonButtonClicked);
        }
        public void OnHintButtonClicked()
        {
            Debug.Log("Hint button clicked");
        }

        public void OnFillAllPolygonButtonClicked()
        {
            if(GameManager.Instance.canInteract) EventManager.Instance.FillAllPolygons?.Invoke();
        }
    }
}
