using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSkinData",
    menuName = "Character/Skin Data",
    order = 1
)]
public class SkinData : ScriptableObject
{
    public Sprite idle;       // stay sprite
    public Sprite[] runRight; // 3 strites - run right
    public Sprite[] runLeft;  // 3 strites - run left
}