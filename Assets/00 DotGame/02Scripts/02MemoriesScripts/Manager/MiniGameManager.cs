using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public bool isFinishedMinigame = false;

    public static MiniGameManager Instance;
    public WinLoseUIManager winLoseUIManager;
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
    public void EndMiniGame(bool result)
    {
        if(result)
        {
            isFinishedMinigame = true;
            winLoseUIManager.ShowWinPanel();
        }
        else
        {
            isFinishedMinigame = false;
            winLoseUIManager.ShowLosePanel();
        }
    }
}
