using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingInteraction : MonoBehaviour
{
    public GameObject indicator;
    public GameObject catchZone;
    private bool isNearFishingSpot = false;
    private bool isFishingActive = false;

    void Update()
    {
        if (isNearFishingSpot && Input.GetKeyDown(KeyCode.E))
        {
            StartFishing();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ExitFishing(); // <-- викликаємо правильний метод
        }
    }

    void StartFishing()
    {
        indicator.SetActive(true);
        catchZone.SetActive(true);
        isFishingActive = true;
    }

    void ExitFishing()
    {
        indicator.SetActive(false);
        catchZone.SetActive(false);
        isFishingActive = false;

        // Зберігаємо позицію повернення
        GlobalGameState.comingFromMiniGame = true;

        // Можеш виставити координати прямо тут, або брати позицію гравця:
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            GlobalGameState.spawnPosition = player.transform.position;
        }
        else
        {
            GlobalGameState.spawnPosition = new Vector2(-4.0f, -1f); // запасний варіант
        }

        SceneManager.LoadScene("Main Game Map");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearFishingSpot = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearFishingSpot = false;
        }
    }
}
