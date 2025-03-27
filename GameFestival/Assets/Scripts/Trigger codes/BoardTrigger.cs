using UnityEngine;

public class BoardTrigger : MonoBehaviour
{
    [Header ("UI Elements")]
    [Tooltip("Main window with text, that shows after clicking E")]
    public GameObject infoPanel;
    
    [Tooltip("Small tip, click E")]
    public GameObject pressEHint;

    private bool playerInRange = false;
    private bool panelOpen = false;

    private void Start(){
        if (infoPanel!=null){
            infoPanel.SetActive(false);
        }
        if (pressEHint != null){
            pressEHint.SetActive(false);
        }
    }

    private void OnTriggerEnter2D (Collider2D other){
        if (other.CompareTag("Player")){
            playerInRange = true;
            if (pressEHint != null){
                pressEHint.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D (Collider2D other){
        if (other.CompareTag("Player")){
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


    private void Update (){
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !panelOpen){   // open info board using E
            panelOpen = true;
            if (infoPanel != null){
                infoPanel.SetActive(true);
            }
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.Q) && panelOpen){    // open info board using Q
            panelOpen = false;
            if (infoPanel != null){
                infoPanel.SetActive(false);
            }
        }
    }
}
