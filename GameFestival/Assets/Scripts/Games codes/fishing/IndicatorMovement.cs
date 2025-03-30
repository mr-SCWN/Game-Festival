using UnityEngine;
using UnityEngine.UI;

public class IndicatorMovement : MonoBehaviour
{
    public RectTransform indicator; 
    public RectTransform catchZone; 
    public float moveSpeed = 300f;  

    private float minY, maxY;  
    void Start()
    {
        RectTransform panel = transform.parent.GetComponent<RectTransform>();
        minY = panel.rect.yMin;
        maxY = panel.rect.yMax;
    }

    void Update()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float newY = Mathf.Clamp(indicator.anchoredPosition.y + move, minY, maxY);
        indicator.anchoredPosition = new Vector2(indicator.anchoredPosition.x, newY);
    }
}
