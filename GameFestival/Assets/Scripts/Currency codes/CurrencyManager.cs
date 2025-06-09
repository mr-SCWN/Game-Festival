using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("Player Currency")]
    public int coin = 0;
    public int grey_fish = 0;
    public int green_fish= 0;
    public int gold_fish = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddFish(string type, int amount = 1)
    {
        switch(type.ToLower())
        {
            case "grey":
                grey_fish += amount;
                break;
            case "green":
                green_fish += amount;
                break;
            case "gold":
                gold_fish += amount;
                break;
            default:
                Debug.LogWarning($"Unknown fish type «{type}»");
                break;
        }
    }

    public void AddCoin(int amount = 1)
    {
        coin += amount;
    }
}
