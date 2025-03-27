
using UnityEngine;
using UnityEngine.Rendering;

public class Cards : MonoBehaviour {
public int CardID;
public Sprite frontSprite;
public Sprite backSprite;

private SpriteRenderer spriteRenderer;

public bool isFlipped = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ShowBack();
    }

    public void Flip(){
        isFlipped = !isFlipped;
        if (isFlipped) ShowFront();
        else ShowBack();
    }

    public void ShowFront(){
        spriteRenderer.sprite = frontSprite;
    }

    public void ShowBack(){
        spriteRenderer.sprite = backSprite;
    }

    public void SetHighlight(bool highlight) {   // creating highlight to show what card was chosen
        spriteRenderer.color = highlight ? Color.yellow : Color.white;
    }
}