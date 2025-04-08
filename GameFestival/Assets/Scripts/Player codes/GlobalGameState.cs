using UnityEngine;

public static class GlobalGameState
{
    // if TRUE - returning from minigame to spawnPosition
    public static bool comingFromMiniGame = false;

    // Save coordinate where we want to appear after minigame
    public static Vector2 spawnPosition = Vector2.zero;
}
