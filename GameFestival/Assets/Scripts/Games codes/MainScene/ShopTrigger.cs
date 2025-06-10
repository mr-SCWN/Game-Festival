using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    public ShopManager shopManager;
    public GameObject  hintText; // «Нажмите E»

    bool inRange;

    void Start() => hintText.SetActive(false);

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            inRange = true;
            hintText.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            inRange = false;
            hintText.SetActive(false);
            shopManager.CloseShop();
        }
    }

    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
            shopManager.OpenShop();
        if (inRange && Input.GetKeyDown(KeyCode.Q))
            shopManager.CloseShop();
    }
}
