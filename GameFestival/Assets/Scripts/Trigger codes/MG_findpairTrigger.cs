using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Окно с информацией, которое показывается перед переходом в новую сцену (например, описание миниигры)")]
    public GameObject infoPanel;
    
    [Tooltip("Подсказка, например 'Нажмите E', которая показывается при входе в зону")]
    public GameObject pressEHint;
    
    [Tooltip("Имя сцены, которую нужно загрузить")]
    public string sceneToLoad = "MiniGameScene";

    private bool playerInRange = false;

    private void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
        if (pressEHint != null)
            pressEHint.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressEHint != null)
                pressEHint.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressEHint != null)
                pressEHint.SetActive(false);
            if (infoPanel != null)
                infoPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Если информационное окно ещё не активно, показываем его
            if (infoPanel != null && !infoPanel.activeSelf)
            {
                infoPanel.SetActive(true);
                // Если есть подсказка, можно её скрыть после открытия окна
                if (pressEHint != null)
                    pressEHint.SetActive(false);
            }
            // Если окно уже открыто, загружаем новую сцену
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
