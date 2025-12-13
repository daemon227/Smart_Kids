
using System.Collections;
using Inwave.DongA.DotPuzzle.Event;
using Inwave.DongA.DotPuzzle.InsectSystem;
using Inwave.DongA.DotPuzzle.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

public class SprayManager : MonoBehaviour
{
    public InsectManager insectManager;
    public SprayAnim sprayAnim;
    public GameObject pariticelPrefab;
    
    private void Start()
    {
        ButtonEventManager.Instance.onSprayButtonClick += SprayButtonClickHandle;
    }

    void OnDisable()
    {
        ButtonEventManager.Instance.onSprayButtonClick -= SprayButtonClickHandle;
    }

    void SprayButtonClickHandle()
    {
        //EventManager.Instance.OnEndTurn?.Invoke();
        StartCoroutine(StartSprayAnim());
    }
    private IEnumerator StartSprayAnim()
    {
        GameManager.Instance.canInteract = false;
        sprayAnim.gameObject.SetActive(true);
        
        insectManager.isTimerActive = false;
        sprayAnim.gameObject.SetActive(true);
        yield return sprayAnim.PlaySprayAnim(() =>
        {
            ShowInsectsEmote();
        });
        GameManager.Instance.canInteract = true;
        DestroyInsects();
    }
    
    public GameObject GetInsects()
    {
        if (insectManager.insects.Count > 0)
        {
            return insectManager.insects[Random.Range(0, insectManager.insects.Count)];
        }
        return null;
    }

    private void DestroyInsects()
    {
        var insects = insectManager.insects;
        Debug.Log("Insects Count: " + insects.Count);
        foreach (var insect in insects)
        {
            var insectComponet = insect.GetComponent<IInsect>();
            insectComponet.HideEmote();
            insectComponet.DeadBySpray();
        }
        insectManager.insects.Clear();
        insectManager.ResetLifeTime();
        insectManager.CheckInsects();
    }
    
    private void ShowInsectsEmote()
    {
        var insects = insectManager.insects;
        foreach (var insect in insects)
        {
            insect.GetComponent<IInsect>().ShowEmote(false);
        }
    }
    
}
