using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Inwave.DongA.DotPuzzle.UI.StepUI
{
    public class StepNode : MonoBehaviour
    {
        public GameObject succesImage;
        public GameObject inStepImage;

        public void SetStepNodeStatus(bool isSuccess)
        {
            succesImage.SetActive(isSuccess);
            succesImage.transform.DOScale(Vector3.one, 1f).From(Vector3.zero).SetEase(Ease.OutBack);
            inStepImage.SetActive(!isSuccess);
        }
        
   
    }
}

