using UnityEngine;
using UnityEngine.SceneManagement;
public class ExchangeTrigger : MonoBehaviour
{
    public ExchangeUI exchangeUI; 
    public GameObject hintE;      // UI “Press E”

    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            hintE.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            hintE.SetActive(false);
        }
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            exchangeUI.Show();
    }
}
