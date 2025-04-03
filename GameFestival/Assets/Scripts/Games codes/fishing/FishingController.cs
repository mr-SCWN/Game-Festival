using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FishingController : MonoBehaviour
{
    [Header("Fishing Setup")]
    public GameObject fish;  // Рибка
    public GameObject fishingLine;  // Лінія
    public GameObject fishingRod;  // Вудка
    public Slider fishingProgressBar; // Прогрес-бар
    public Text resultText;   // Результат гри (спіймав рибу чи ні)

    private bool isFishing = false;  // Перевіряє, чи зараз рибалка
    private float fishingTime = 0f;  // Час, коли з'являється рибка
    private float lineSpeed = 1.0f; // Швидкість руху лінії
    private float fishPosition = 0f; // Позиція рибки на шкалі
    private float linePosition = 0f; // Позиція лінії
    private bool isFishCaught = false;

    void Start()
    {
        // Початкове приховування рибки і прогрес-бару
        fish.SetActive(false);
        fishingProgressBar.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isFishing)
        {
            MoveFishingLine();
            CheckCatch();

            fishingProgressBar.value = fishingTime;
            fishingTime -= Time.deltaTime;  // Час на рибу (зменшується)

            // Якщо рибка вже зловлена
            if (isFishCaught)
            {
                ShowResult("You caught the fish!");
            }
        }
        else
        {
            // Старт риболовлі через натискання кнопки E
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartFishing();
            }
        }
    }

    void StartFishing()
    {
        // Запускаємо риболовлю, де рибка з'явиться через випадковий час (до 5 секунд)
        isFishing = true;
        fish.SetActive(false);  // Приховуємо рибку на початку
        fishingProgressBar.gameObject.SetActive(true);
        fishingTime = Random.Range(1f, 5f); // Час для появи рибки
        resultText.gameObject.SetActive(false);
    }

    void MoveFishingLine()
    {
        // Переміщаємо лінію, якщо гравець тримає на клавіші
        if (Input.GetKey(KeyCode.W))
        {
            linePosition += lineSpeed * Time.deltaTime;  // Рух лінії вгору
        }
        else if (Input.GetKey(KeyCode.S))
        {
            linePosition -= lineSpeed * Time.deltaTime;  // Рух лінії вниз
        }

        // Обмежуємо рух лінії в межах шкали
        linePosition = Mathf.Clamp(linePosition, 0f, 1f);

        fishingLine.transform.position = new Vector3(fishingLine.transform.position.x, Mathf.Lerp(-5f, 5f, linePosition), fishingLine.transform.position.z);
    }

    void CheckCatch()
    {
        // Перевірка, чи співпала позиція лінії з позицією рибки
        if (fish.activeSelf && Mathf.Abs(linePosition - fishPosition) < 0.1f)
        {
            isFishCaught = true;
        }

        // Якщо час закінчився і рибка з'явилася, показуємо її
        if (fishingTime <= 0f && !fish.activeSelf)
        {
            fishPosition = Random.Range(0f, 1f);  // Випадкова позиція рибки на шкалі
            fish.transform.position = new Vector3(fish.transform.position.x, Mathf.Lerp(-5f, 5f, fishPosition), fish.transform.position.z);
            fish.SetActive(true);
        }
    }

    void ShowResult(string message)
    {
        resultText.gameObject.SetActive(true);
        resultText.text = message;
        isFishing = false;
    }
}
