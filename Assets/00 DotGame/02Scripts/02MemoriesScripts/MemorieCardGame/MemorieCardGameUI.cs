using TMPro;
using UnityEngine;

namespace DACN.Memories
{
public class MemorieCardGameUI : MonoBehaviour
{
    public TextMeshProUGUI specialCardFoundCountUI;
    private MemoriesGame memorieCardGame;

    private void Awake()
    {
        memorieCardGame = this.GetComponent<MemoriesGame>();
    }
    private void Start()
    {
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
}
}
