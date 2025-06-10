using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExchangeUI : MonoBehaviour
{
    [Header("Panel Elements")]
    public GameObject panel;               // ExchangePanel
    public TMP_Text errorText;

    [Header("Rows")]
    public FishRow greyRow;
    public FishRow greenRow;
    public FishRow goldRow;

    [Header("Confirm/Close")]
    public Button confirmBtn;
    public Button closeBtn;

    // Rates
    private readonly int rateGrey  = 2;
    private readonly int rateGreen = 5;
    private readonly int rateGold  = 15;

    void Awake()
    {
        panel.SetActive(false);
        errorText.text = "";

        // setup rows
        greyRow.Setup("grey", rateGrey);
        greenRow.Setup("green", rateGreen);
        goldRow.Setup("gold", rateGold);

        // buttons
        confirmBtn.onClick.AddListener(OnConfirm);
        closeBtn.onClick.AddListener(() => panel.SetActive(false));
    }

    
    public void Show()
    {
        // refresh available counts
        greyRow.RefreshHave();
        greenRow.RefreshHave();
        goldRow.RefreshHave();

        errorText.text = "";
        panel.SetActive(true);
    }

    void OnConfirm()
    {
        int gCount  = greyRow.GetCount();
        int gnCount = greenRow.GetCount();
        int glCount = goldRow.GetCount();

        // Check enough fish
        if (gCount  > CurrencyManager.Instance.grey_fish  ||
            gnCount > CurrencyManager.Instance.green_fish ||
            glCount > CurrencyManager.Instance.gold_fish)
        {
            errorText.text = "Not enough fish!";
            return;
        }

        // Perform exchange
        int coins = gCount * rateGrey 
                  + gnCount * rateGreen 
                  + glCount * rateGold;
        CurrencyManager.Instance.AddCoin(coins);

        CurrencyManager.Instance.grey_fish  -= gCount;
        CurrencyManager.Instance.green_fish -= gnCount;
        CurrencyManager.Instance.gold_fish  -= glCount;

        panel.SetActive(false);
    }
}


[Serializable]
public class FishRow
{
    public string       fishType;    // "grey", "green" or "gold"
    public Image        fishIcon;
    public TMP_Text     rateText;
    public TMP_Text     countText;
    public Button       minusBtn;
    public Button       plusBtn;
    public TMP_Text     haveText;

    private int rate;
    private int count = 0;

    public void Setup(string type, int rate)
    {
        fishType = type;
        this.rate = rate;
        rateText.text  = $"1 {type} → {rate} coins";
        count = 0;
        countText.text = "0";

        minusBtn.onClick.RemoveAllListeners();
        plusBtn.onClick.RemoveAllListeners();

        minusBtn.onClick.AddListener(() => ChangeCount(-1));
        plusBtn.onClick.AddListener(() => ChangeCount(+1));
    }

    public void RefreshHave()
    {
        int have = 0;
        switch(fishType)
        {
            case "grey":  have = CurrencyManager.Instance.grey_fish;  break;
            case "green": have = CurrencyManager.Instance.green_fish; break;
            case "gold":  have = CurrencyManager.Instance.gold_fish;  break;
        }
        haveText.text = $"You have: {have}";
    }

    void ChangeCount(int delta)
    {
        RefreshHave();
        int have = 0;
        switch(fishType)
        {
            case "grey":  have = CurrencyManager.Instance.grey_fish;  break;
            case "green": have = CurrencyManager.Instance.green_fish; break;
            case "gold":  have = CurrencyManager.Instance.gold_fish;  break;
        }
        count = Mathf.Clamp(count + delta, 0, have);
        countText.text = count.ToString();
    }

    public int GetCount() => count;
}
