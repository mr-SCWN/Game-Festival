using UnityEngine;

public class FishingInteraction : MonoBehaviour
{
    public GameObject indicator;
    public GameObject catchZone; 
    private bool isNearFishingSpot = false; 

    void Update()
    {
        if (isNearFishingSpot && Input.GetKeyDown(KeyCode.E))
        {
            indicator.SetActive(true);
            catchZone.SetActive(true); 
        }
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