using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingController : MonoBehaviour
{
    [Header("Fishing Setup")]
    public RectTransform indicator;        // Рамка для руху
    public RectTransform indicatorInside;  // Лінія, якою рухаємось
    public RectTransform fish;             // Рибка

    public TextMeshProUGUI resultText;     // Текст результату

    private bool isFishing = false;
    private float fishPosition;            // Випадкова позиція рибки
    private float linePosition = 0.5f;     // Початкова позиція лінії
    private bool isFishCaught = false;     // Чи спіймана рибка

    void Start()
{
    if (fish != null)
    {
        fish.gameObject.SetActive(false);
    }
    else
    {
        Debug.LogError("Об'єкт fish не призначений в інспекторі!");
    }

    resultText.gameObject.SetActive(false);
}

    void Update()
    {
        if (isFishing)
        {
            MoveIndicator();
            CheckCatch();
        }
    }

    public void StartFishing()
    {
        isFishing = true;
        resultText.gameObject.SetActive(false);
        isFishCaught = false;

        // Випадкове розміщення рибки
        fishPosition = Random.Range(0f, 1f);
        fish.anchoredPosition = new Vector2(fish.anchoredPosition.x, 
            Mathf.Lerp(indicator.rect.yMin, indicator.rect.yMax, fishPosition));

        fish.gameObject.SetActive(true);
    }

    void MoveIndicator()
    {
        if (Input.GetKey(KeyCode.W)) {
            linePosition += 1.5f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            linePosition -= 1.5f * Time.deltaTime;
        }

        linePosition = Mathf.Clamp(linePosition, 0f, 1f);
        indicatorInside.anchoredPosition = new Vector2(indicatorInside.anchoredPosition.x, 
            Mathf.Lerp(indicator.rect.yMin, indicator.rect.yMax, linePosition));
    }

    void CheckCatch()
    {
        if (Mathf.Abs(linePosition - fishPosition) < 0.05f)
        {
            isFishCaught = true;
            ShowResult("You caught the fish!");
        }
    }

   void ShowResult(string message)
    {
    resultText.text = message + (isFishCaught ? " - Fish caught!" : " - Fish missed!");
    resultText.gameObject.SetActive(true);
    isFishing = false;
    }

}
