using UnityEngine;
using UnityEngine.UI;
using TMPro; // ВИНОСИМО сюди!

public class FishingController : MonoBehaviour
{
    [Header("Fishing Setup")]
    public Slider fishingProgressBar; // Прогрес-бар для часу
    public GameObject fish;           // Рибка
    public GameObject fishingLine;    // Лінія, що утримує рибу
    //public GameObject fishingRod;     // Вудка

    public TextMeshProUGUI resultText; // Використання TextMeshPro для тексту

    private bool isFishing = false;   // Перевіряє, чи зараз рибалка
    private float fishingTime = 0f;   // Час, коли з'являється рибка
    private float fishPosition = 0f;  // Позиція рибки на шкалі
    private float linePosition = 0f;  // Позиція лінії
    private bool isFishCaught = false; // Чи спіймана риба

    private float fishAppearTime;     // Час до появи рибки

    void Start()
    {
        fishingProgressBar.value = 0; // Початкове значення
        fish.SetActive(false);        // Рибка прихована спочатку
        fishingProgressBar.gameObject.SetActive(false); // Прихований прогрес-бар
        resultText.gameObject.SetActive(false); // Прихований результат
        fishAppearTime = Random.Range(1f, 5f);  // Випадковий час для появи рибки
    }

    void Update()
    {
        if (isFishing)
        {
            fishingTime += Time.deltaTime; // Збільшуємо час риболовлі
            fishingProgressBar.value = fishingTime / fishAppearTime; // Оновлюємо прогрес-бар

            if (fishingTime >= fishAppearTime && !isFishCaught)
            {
                fish.SetActive(true); // Показуємо рибку
                fishPosition = Random.Range(0f, 1f); // Випадкова позиція рибки на шкалі
                fish.transform.position = new Vector3(fish.transform.position.x, Mathf.Lerp(-5f, 5f, fishPosition), fish.transform.position.z);
                isFishCaught = true; // Рибка готова до ловлі
            }

            MoveFishingLine(); // Рух лінії
            CheckCatch(); // Перевірка на лов риби
        }
    }

    public void StartFishing()
    {
        isFishing = true;
        fishingTime = 0f;
        fishingProgressBar.value = 0;
        resultText.gameObject.SetActive(false);
    }

    void MoveFishingLine()
    {
        if (Input.GetKey(KeyCode.W))
        {
            linePosition += 1.0f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            linePosition -= 1.0f * Time.deltaTime;
        }

        linePosition = Mathf.Clamp(linePosition, 0f, 1f);
        fishingLine.transform.position = new Vector3(fishingLine.transform.position.x, Mathf.Lerp(-5f, 5f, linePosition), fishingLine.transform.position.z);
    }

    void CheckCatch()
    {
        if (Mathf.Abs(linePosition - fishPosition) < 0.1f)
        {
            isFishCaught = true;
            ShowResult("You caught the fish!");
        }
    }

    void ShowResult(string message)
    {
        resultText.text = message;
        resultText.gameObject.SetActive(true);
        isFishing = false;
    }
}
