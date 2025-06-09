using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BetUI : MonoBehaviour {
  public GameObject panel;
  public TMP_Text coinsText;
  public TMP_InputField betInput;
  public TMP_Text errorText;
  public Button plus, minus, ok;

  private int maxCoins;
  private Action<int> onBetConfirmed;

  void Awake() {
    panel.SetActive(false);
    plus.onClick.AddListener(() => ChangeBet(+1));
    minus.onClick.AddListener(() => ChangeBet(-1));
    ok.onClick.AddListener(Confirm);
  }

  public void Show(int currentCoins, Action<int> callback) {
    maxCoins = currentCoins;
    onBetConfirmed = callback;
    coinsText.text = $"Your coins: {currentCoins}";
    betInput.text = "1";
    errorText.text = "";
    panel.SetActive(true);
  }

  void ChangeBet(int delta) {
    int v = Int32.Parse(betInput.text);
    v = Mathf.Clamp(v + delta, 1, maxCoins);
    betInput.text = v.ToString();
  }

  void Confirm() {
    if (!Int32.TryParse(betInput.text, out int bet) || bet < 1 || bet > maxCoins) {
      errorText.text = "Invalid bet!";
      return;
    }
    panel.SetActive(false);
    onBetConfirmed?.Invoke(bet);
  }
}
