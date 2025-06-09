using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    [Header("Text fields for each currency")]
    public TextMeshProUGUI coin;
    public TextMeshProUGUI grey_fish;
    public TextMeshProUGUI green_fish;
    public TextMeshProUGUI gold_fish;

    void Update() {
        coin.text       = CurrencyManager.Instance.coin.ToString();
        grey_fish.text  = CurrencyManager.Instance.grey_fish.ToString();
        green_fish.text = CurrencyManager.Instance.green_fish.ToString();
        gold_fish.text  = CurrencyManager.Instance.gold_fish.ToString();
    }
} 
