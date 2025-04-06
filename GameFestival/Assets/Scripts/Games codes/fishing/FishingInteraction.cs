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
    SceneManager.LoadScene("Main Game Map");
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

        // Повернення на головну сцену
        GlobalGameState.comingFromMiniGame = true;
        GlobalGameState.spawnPosition = new Vector2(-42.0f, 20.0f);  // Позиція на головній карті
        SceneManager.LoadScene("Main Game Map");  // Назва твоєї головної сцени
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
