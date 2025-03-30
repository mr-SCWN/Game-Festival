using UnityEngine;

public class Frog : MonoBehaviour
{
    [Header("Frog settings")]
    public SpriteRenderer frogRenderer;     // Main sprite
    public SpriteRenderer outlineRenderer;  // Extra sprite, conditioned by outline
    
    [Tooltip("Base outline color, set in Inspector or auto-assigned in Awake")]
    public Color baseOutlineColor = Color.white;

    public void SetOutlineColor(Color color){
        if (outlineRenderer!=null){
            outlineRenderer.color = color;
        }
    }

    public Color GetDarkerOutline(float factor = 0.5f)
    {
        return new Color(baseOutlineColor.r * factor, baseOutlineColor.g * factor, baseOutlineColor.b * factor, baseOutlineColor.a);
    }
}
