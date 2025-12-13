using System.Collections;
using System.Collections.Generic;
using Inwave.DongA.DotPuzzle.Entity;
using UnityEngine;

public interface IInsect
{
    public Polygon targetPolygon { get; set; }
    public bool IsMoving();
    public Vector3 GetPosition();
    public void Fly();
    public void ShowEmote(bool isHappy);
    public void HideEmote();
    public void Dead();

    public void DeadBySpray();
}
