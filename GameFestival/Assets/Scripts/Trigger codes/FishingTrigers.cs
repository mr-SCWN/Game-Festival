using UnityEngine;
using UnityEngine.SceneManagement; 

public class FishingTrigger : MonoBehaviour
{
    private bool isPlayerNear = false;
    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) 
        {
            SceneManager.LoadScene("MG_Fishing"); 
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerNear = true;
            Debug.Log("Player nearby, press E to fish");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerNear = false;
        }
    }
}