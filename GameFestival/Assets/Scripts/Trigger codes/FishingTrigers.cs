using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingTrigger : MonoBehaviour
{
    private bool isPlayerNear = false;
    [Header("UI Elements")]
    public GameObject pressEHint;  // Підказка для взаємодії

    void Start()
    {
        if (pressEHint != null)
            pressEHint.SetActive(false); // Початково підказка не видима
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) 
        {
            // Завантажити сцену риболовлі, коли гравець натискає E
            SceneManager.LoadScene("MG_Fishing");
        }

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Q)) 
        {
            // Вихід зі сцени або повернення на головну карту
            SceneManager.LoadScene("Main Game Map");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerNear = true;
            // Показуємо підказку при вході персонажа в тригер
            if (pressEHint != null)
                pressEHint.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerNear = false;
            // При виході з тригера приховуємо підказку
            if (pressEHint != null)
                pressEHint.SetActive(false);
        }
    }
}
