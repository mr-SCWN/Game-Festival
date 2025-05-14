using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        if (GlobalGameState.comingFromMiniGame)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = GlobalGameState.spawnPosition;
            }

            GlobalGameState.comingFromMiniGame = false;
        }
    }
}
