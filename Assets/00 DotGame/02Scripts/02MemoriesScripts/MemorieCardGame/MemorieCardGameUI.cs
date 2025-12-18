using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DACN.Memories
{
public class MemorieCardGameUI : MonoBehaviour
{
    public TextMeshProUGUI specialCardFoundCountUI;
    public TextMeshProUGUI levelUI;
    public TextMeshProUGUI scoreUI;

    [Header("Buttons")]
    public GameObject pausePanel;
    public Button settingButton;
    public Button closeButton;
    public Button homeButton;
    private MemoriesGame memorieCardGame;

    private void Awake()
    {
        memorieCardGame = this.GetComponent<MemoriesGame>();
    }
    private void Start()
    {
        settingButton.onClick.AddListener(OnSettingButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        specialCardFoundCountUI.text = "Số hình đã được tìm thấy: " + 0 + "/" + memorieCardGame.SpecialCardCount;
    }
    private void OnEnable()
    {
        memorieCardGame.OnUpdateSpecialCardFound += UpdateSpecialCardFoundCount;
    }
    private void OnDisable()
    {
        memorieCardGame.OnUpdateSpecialCardFound -= UpdateSpecialCardFoundCount;
    }
    public void UpdateSpecialCardFoundCount(int count)
    {
        specialCardFoundCountUI.text = "Số hình đã được tìm thấy: " + count.ToString() + "/"+ memorieCardGame.SpecialCardCount;
    }

    void OnSettingButtonClicked()
    {
        pausePanel.SetActive(true);    
        pausePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    void OnCloseButtonClicked()
    {
        pausePanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            pausePanel.SetActive(false);
        });
    }

    void OnHomeButtonClicked()
    {
        SceneManager.LoadScene("ChildScene");
    }

}
}
