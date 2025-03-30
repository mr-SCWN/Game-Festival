using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The information window that appears before switching to the new scene (e.g., mini-game description)")]
    public GameObject infoPanel;
    
    [Tooltip("The hint (e.g., 'Press E') that appears when entering the trigger zone")]
    public GameObject pressEHint;
    
    [Tooltip("The name of the scene to load")]
    public string sceneToLoad = "MiniGameScene";

    private bool playerInRange = false;
    private bool panelOpen = false;

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
            if (pressEHint!=null){
                pressEHint.SetActive(false);
            }
            if (panelOpen && infoPanel != null){
                panelOpen = false;
                infoPanel.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !panelOpen)
        {
            panelOpen = true;
            // If the information window is not active yet, show it
            if (infoPanel != null && !infoPanel.activeSelf)
            {
                infoPanel.SetActive(true);
                // Hide the hint after opening the window
                if (pressEHint != null)
                    pressEHint.SetActive(false);
            }
            // If the window is already open, load the new scene
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.Q) && panelOpen){    // close info board using Q
            panelOpen = false;
            playerInRange = true;
            if (infoPanel != null){
                infoPanel.SetActive(false);
            }
            if (pressEHint != null)
                pressEHint.SetActive(true);
        }
    }
}
