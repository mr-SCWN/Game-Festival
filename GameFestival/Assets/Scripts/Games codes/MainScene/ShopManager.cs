using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup shopCanvas;
    public SkinSlotUI[] slots;            
    public Button      closeButton;       

    [Header("Prices")]
    public int[] prices = new int[4];     

    [Header("References")]
    public SkinManager skinManager;     

    const string PREFS_KEY_PURCHASE = "SkinPurchased";   
    const string PREFS_KEY_COUNT    = "SkinCount";      

    private bool[] purchased;           

    void Awake()
    {
        int count = slots.Length;
        purchased = new bool[count];

        if (!PlayerPrefs.HasKey(PREFS_KEY_COUNT))
        {
            PlayerPrefs.SetInt(PREFS_KEY_COUNT, count);
            PlayerPrefs.SetString(PREFS_KEY_PURCHASE, "1" + new string('0', count - 1));
            PlayerPrefs.Save();
        }

       
        string mask = PlayerPrefs.GetString(PREFS_KEY_PURCHASE);
        for (int i = 0; i < count && i < mask.Length; i++)
            purchased[i] = (mask[i] == '1');

        shopCanvas.alpha            = 0;
        shopCanvas.interactable     = false;
        shopCanvas.blocksRaycasts   = false;

        closeButton.onClick.AddListener(CloseShop);
        for (int i = 0; i < count; i++)
        {
            int idx = i; 
            slots[i].buyButton.onClick  .AddListener(() => OnBuy(idx));
            slots[i].equipButton.onClick.AddListener(() => OnEquip(idx));
        }
    }

    public void OpenShop()
    {
        RefreshUI();
        shopCanvas.alpha          = 1;
        shopCanvas.interactable   = true;
        shopCanvas.blocksRaycasts = true;
    }

    public void CloseShop()
    {
        shopCanvas.alpha          = 0;
        shopCanvas.interactable   = false;
        shopCanvas.blocksRaycasts = false;
    }

    private void RefreshUI()
    {
        int coins = CurrencyManager.Instance.coin;
        for (int i = 0; i < slots.Length; i++)
        {
            bool owned = purchased[i];
            slots[i].priceText.text       = prices[i].ToString();
            slots[i].buyButton.gameObject .SetActive(!owned);
            slots[i].buyButton.interactable= (!owned && coins >= prices[i]);
            slots[i].equipButton.gameObject.SetActive(owned);
            slots[i].checkmark.SetActive(owned);
        }
    }

    private void OnBuy(int idx)
    {
        if (purchased[idx]) return;
        var cm = CurrencyManager.Instance;
        if (cm.coin < prices[idx]) return;

        cm.AddCoin(-prices[idx]);
        purchased[idx] = true;
        SavePurchases();
        RefreshUI();
    }

    private void OnEquip(int idx)
{
    if (!purchased[idx]) return;
    skinManager.Equip(idx);  
}


    private void SavePurchases()
    {
        var sb = new System.Text.StringBuilder();
        foreach (bool b in purchased)
            sb.Append(b ? '1' : '0');
        PlayerPrefs.SetString(PREFS_KEY_PURCHASE, sb.ToString());
        PlayerPrefs.Save();
    }
}