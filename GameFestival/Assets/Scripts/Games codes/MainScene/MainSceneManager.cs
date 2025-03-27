using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        if (GlobalGameState.comingFromMiniGame)
        {
            
            player.position = GlobalGameState.spawnPosition;

            
            GlobalGameState.comingFromMiniGame = false;
        }
    }
}
