using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FishingController : MonoBehaviour
{
    public RectTransform indicator;
    public RectTransform indicatorInside;
    public RectTransform fish;
    public TextMeshProUGUI resultText;
    public GameObject gameOverPanel;  // Панель для результатів гри
    public Button restartButton; // Кнопка для перезапуску гри
    public Button mainMenuButton; // Кнопка для повернення в головне меню
    public Button startButton;  // Кнопка старту гри
    public TextMeshProUGUI VictoryText; // Текст для перемоги
public TextMeshProUGUI FailureText; // Текст для поразки
public Image holdProgressBar; // 🔁 Сюди підтягнемо прогрес-бар з UI
public AudioSource audioSource;
public AudioClip victorySound;

private float holdTime = 0f;
private float requiredHoldTime = 5f;

private float fishMoveTimer = 0f;
private float fishMoveSpeed = 0.5f;

    private bool isFishing = false;
    private float fishPosition;
    private float linePosition = -2.94f;
    private bool isFishCaught = false;
    private float gameTime = 10f;  // Таймер 10 секунд
    private float remainingTime;  // Залишковий час для гри
    private float fishStartX;

    void Start()
    {
        remainingTime = gameTime;  // Ініціалізуємо таймер
        gameOverPanel.SetActive(false);  // Панель результатів прихована
        resultText.gameObject.SetActive(false); // Текст результату не показується
        restartButton.gameObject.SetActive(false); // Кнопка перезапуску не показується
        mainMenuButton.gameObject.SetActive(false); // Кнопка повернення не показується
        fishStartX = fish.anchoredPosition.x;

        if (fish != null)
            fish.gameObject.SetActive(false);

        resultText.gameObject.SetActive(false);

        linePosition = -263f;  // Початкова позиція
        SetIndicatorPosition();
        SetFishPosition();  // Встановлюємо стартову позицію для рибки
    }

    void Update()
    {
        if (isFishing)
{
    remainingTime -= Time.deltaTime;
    if (remainingTime <= 0f)
    {
        EndGame(false);
    }
    else
    {
        resultText.text = "Час: " + Mathf.Ceil(remainingTime).ToString();
    }

    MoveIndicator();

    // ⬆️ Рух рибки рандомно
    fishMoveTimer += Time.deltaTime * fishMoveSpeed;
    fishPosition = Mathf.PingPong(fishMoveTimer + Random.Range(0f, 9f), 1f);
    SetFishPosition();

    CheckIfFishCaught();
}
    }

    public void StartFishing()
    {
        isFishing = true;
        isFishCaught = false;
        resultText.gameObject.SetActive(false);

        if (startButton != null)
        {
            startButton.gameObject.SetActive(false); // Це приховає кнопку
        }

        fishPosition = Random.Range(0f, 1f); // Генерація позиції рибки
        SetFishPosition(); // Встановлення позиції рибки після старту риболовлі

        Debug.Log("🐟 Fish set at Y: " + fish.anchoredPosition.y);

        fish.gameObject.SetActive(true);
    }

    void MoveIndicator()
    {
        // Зменшуємо швидкість руху
        float moveSpeed = 50f;  // Повільніший рух

        // Рух індикатора
        if (Input.GetKey(KeyCode.W))
            linePosition += moveSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            linePosition -= moveSpeed * Time.deltaTime;

        // Обмеження руху індикатора
        linePosition = Mathf.Clamp(linePosition, -185f, 165f);

        // Перевіряємо, чи зловлена рибка
        CheckIfFishCaught();

        SetIndicatorPosition();
        SetFishPosition(); // Зміщення рибки при зміні індикатора

        Debug.Log("📍 Indicator at Y: " + indicatorInside.anchoredPosition.y);
    }

    void CheckIfFishCaught()
{
    float fishY = fish.anchoredPosition.y;
    float indicatorY = indicatorInside.anchoredPosition.y;
    float threshold = 10f;

    if (Mathf.Abs(fishY - indicatorY) < threshold)
    {
        holdTime += Time.deltaTime;

        // Оновлення UI-бара
        if (holdProgressBar != null)
            holdProgressBar.fillAmount = holdTime / requiredHoldTime;

        if (holdTime >= requiredHoldTime)
        {
            if (audioSource != null && victorySound != null)
                audioSource.PlayOneShot(victorySound);

            EndGame(true);
        }
    }
    else
    {
        holdTime = Mathf.Max(holdTime - Time.deltaTime * 2f, 0f);

        if (holdProgressBar != null)
            holdProgressBar.fillAmount = holdTime / requiredHoldTime;
    }
}

   void SetIndicatorPosition()
{
    if (indicator == null || indicatorInside == null) return;

    float height = indicator.rect.height;
    float yOffset = Mathf.Lerp(-height / 2, height / 2, Mathf.InverseLerp(-263f, 229f, linePosition));

    indicatorInside.anchoredPosition = new Vector2(
        indicatorInside.anchoredPosition.x,
        indicator.anchoredPosition.y + yOffset
    );
}
  void SetFishPosition()
{
    if (fish == null || indicator == null) return;

    float height = indicator.rect.height;
    float fishYOffset = Mathf.Lerp(-height / 2, height / 2, Mathf.InverseLerp(-263f, 229f, fishPosition));

    float newY = indicator.anchoredPosition.y + fishYOffset;

    // Захист від виходу за межі
    newY = Mathf.Clamp(newY, -263f, 229f);

    fish.anchoredPosition = new Vector2(
        fish.anchoredPosition.x,
        newY
    );
}

    void EndGame(bool isVictory)
{
    gameOverPanel.SetActive(true);

    if (isVictory)
    {
        VictoryText.gameObject.SetActive(true);  // Показуємо текст перемоги
        FailureText.gameObject.SetActive(false); // Ховаємо текст поразки
    }
    else
    {
        VictoryText.gameObject.SetActive(false);  // Ховаємо текст перемоги
        FailureText.gameObject.SetActive(true);  // Показуємо текст поразки
    }

    indicator.gameObject.SetActive(false);
    fish.gameObject.SetActive(false);

    restartButton.gameObject.SetActive(true);  // Показуємо кнопку перезапуску
    mainMenuButton.gameObject.SetActive(true); // Показуємо кнопку повернення в головне меню
}
public void RestartGame()
{
    remainingTime = gameTime;
    resultText.gameObject.SetActive(false);
    gameOverPanel.SetActive(false);
    restartButton.gameObject.SetActive(false);
    mainMenuButton.gameObject.SetActive(false);

    // Скидаємо індикатор
    indicator.gameObject.SetActive(true);
    if (indicatorInside != null)
    {
        linePosition = -263f; // Скидання
        SetIndicatorPosition(); // Оновлення
    }

    // Нова позиція рибки
    fishPosition = Random.Range(0f, 1f); // 🎯 Скидання
    SetFishPosition();

    fish.gameObject.SetActive(true);

    // Скидаємо стани
    isFishing = true;
    VictoryText.gameObject.SetActive(false);
    FailureText.gameObject.SetActive(false);
    startButton.gameObject.SetActive(false);

    holdTime = 0f;
if (holdProgressBar != null)
    holdProgressBar.fillAmount = 0f;

}

    public void GoToMainMenu()
    {
        gameOverPanel.SetActive(false);
        resultText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);  // Показуємо кнопку старту
        SceneManager.LoadScene("Main Game Map");
    }
}
