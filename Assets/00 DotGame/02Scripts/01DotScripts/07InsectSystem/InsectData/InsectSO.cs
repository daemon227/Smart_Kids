
using UnityEngine;

[CreateAssetMenu(fileName = "NewInsect", menuName = "DotPuzzle/Insect")]
public class InsectSO : ScriptableObject
{
    [Header("Movement Config")]
    public Vector2 moveRange = new Vector2(1f, 2f);
    public float moveSpeed = 2f;      
        
    [Header("Spiral Effect Config")]
    public float spiralSpeed = 10f;    
    public float spiralRadius = 0.5f;  
        
    [Header("Death Settings")]
    public float kickForce = 10f;      
    public float gravity = 25f;        
    public float deathSpinSpeed = 720f;

    [Header("Emotes settings")]
    public SpriteRenderer emoteSpriteRenderer;
    public Sprite happyEmote;
    public Sprite sadEmote;
}
