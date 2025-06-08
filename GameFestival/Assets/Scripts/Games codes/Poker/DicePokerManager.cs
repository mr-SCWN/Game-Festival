using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DicePokerManager : MonoBehaviour
{
    [Header("Player Dice UI")]
    public Image[] playerDiceImages = new Image[5];
    public Button[] playerDiceButtons = new Button[5];  // buttons on same GameObjects

    [Header("AI Dice UI")]
    public Image[] aiDiceImages = new Image[5];

    [Header("Face Sprites")]
    [Tooltip("1-pip through 6-pip")]
    public Sprite[] faceSprites = new Sprite[6];

    [Header("Reroll UI")]
    public Button rerollButton;
    public TMP_Text rerollsLeftText;
    public TMP_Text aiRerollsLeftText;
    public TMP_Text infoText;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("Game Settings")]
    public int maxRerolls = 2;


    int[] playerDice, aiDice;
    bool[] playerMark;
    int playerRerolls, aiRerolls, currentRoll;

    void Start()
    {

        playerDice = new int[5];
        aiDice = new int[5];
        playerMark = new bool[5];

        playerRerolls = aiRerolls = maxRerolls;
        currentRoll = 1;

        RollAll(playerDice);
        RollAll(aiDice);
        RefreshUI();
        UpdateRerollUI();

        rerollButton.onClick.AddListener(OnPlayerReroll);

        for (int i = 0; i < 5; i++)
        {
            int idx = i; // capture
            playerDiceButtons[i].onClick.AddListener(() => ToggleDice(idx));
        }

        infoText.text = "Roll #1 → select dice and press Re-roll";
    }
    void Update()
    {
        // Exit on Q at any time
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ExitToMainMap();
        }
    }

    // roll every slot in dice[]
    void RollAll(int[] dice)
    {
        for (int i = 0; i < dice.Length; i++)
            dice[i] = Random.Range(1, 7);
    }

    // toggle mark on player dice
    void ToggleDice(int idx)
    {
        if (currentRoll >= 3) return;      // no toggling on last round
        playerMark[idx] = !playerMark[idx];
        playerDiceImages[idx].color = playerMark[idx] ? selectedColor : normalColor;
    }

    // called when player clicks Re-roll
    void OnPlayerReroll()
    {
        if (playerRerolls <= 0 || currentRoll >= 3) return;

        // reroll marked dice
        for (int i = 0; i < 5; i++)
        {
            if (playerMark[i])
                playerDice[i] = Random.Range(1, 7);
            // reset mark and color
            playerMark[i] = false;
            playerDiceImages[i].color = normalColor;
        }
        playerRerolls--;
        currentRoll++;
        RefreshUI();
        UpdateRerollUI();

        // AI reroll next
        StartCoroutine(AIReroll());
    }

    IEnumerator AIReroll()
    {
        yield return new WaitForSeconds(0.5f);

        if (aiRerolls > 0 && currentRoll <= 3)
        {
            // determine best value group
            var counts = new Dictionary<int, int>();
            foreach (int v in aiDice)
                counts[v] = counts.ContainsKey(v) ? counts[v] + 1 : 1;

            int bestVal = 1, bestCount = 0;
            foreach (var kv in counts)
                if (kv.Value > bestCount)
                { bestCount = kv.Value; bestVal = kv.Key; }

            // reroll all not equal to bestVal
            for (int i = 0; i < 5; i++)
                if (aiDice[i] != bestVal)
                    aiDice[i] = Random.Range(1, 7);

            aiRerolls--;
        }

        RefreshUI();
        UpdateRerollUI();

        // if last roll, decide winner
        if (currentRoll >= 3)
            DetermineWinner();
        else
            infoText.text = $"Roll #{currentRoll} → select dice and press Re-roll";
    }

    // refresh all dice images
    void RefreshUI()
    {
        for (int i = 0; i < 5; i++)
        {
            playerDiceImages[i].sprite = faceSprites[playerDice[i] - 1];
            aiDiceImages[i].sprite = faceSprites[aiDice[i] - 1];
        }
    }

    // update reroll counters and button state
    void UpdateRerollUI()
    {
        rerollsLeftText.text = $"Your rerolls: {playerRerolls}";
        aiRerollsLeftText.text = $"AI rerolls: {aiRerolls}";
        rerollButton.interactable = (playerRerolls > 0 && currentRoll < 3);
    }

    // after 3 rolls, compare hands
    void DetermineWinner()
    {
        int pRank = EvaluateHand(playerDice);
        int aRank = EvaluateHand(aiDice);

        if (pRank > aRank) infoText.text = "You Win!";
        else if (pRank < aRank) infoText.text = "AI Wins!";
        else infoText.text = "Draw!";

        // disabled further rerolls
        rerollButton.interactable = false;

        ExitToMainMap();
    }

    // rank hands: 1=High,2=Pair,3=TwoPair,4=Three,5=FullHouse,6=Str15,7=Str26,8=Four,9=Five
    int EvaluateHand(int[] dice)
    {
        int[] cnt = new int[7];
        foreach (int v in dice) cnt[v]++;

        bool five = false, four = false, three = false;
        int pairs = 0;
        for (int v = 1; v <= 6; v++)
        {
            if (cnt[v] == 5) five = true;
            if (cnt[v] == 4) four = true;
            if (cnt[v] == 3) three = true;
            if (cnt[v] == 2) pairs++;
        }

        bool str15 = cnt[1] == 1 && cnt[2] == 1 && cnt[3] == 1 && cnt[4] == 1 && cnt[5] == 1;
        bool str26 = cnt[2] == 1 && cnt[3] == 1 && cnt[4] == 1 && cnt[5] == 1 && cnt[6] == 1;

        if (five) return 9;
        if (four) return 8;
        if (str26) return 7;
        if (str15) return 6;
        if (three && pairs == 1) return 5;
        if (three) return 4;
        if (pairs == 2) return 3;
        if (pairs == 1) return 2;
        return 1;
    }
    
        void ExitToMainMap()
    {
        GlobalGameState.comingFromMiniGame = true;
        GlobalGameState.spawnPosition = new Vector2(-22.02046f, 8.365251f);
        SceneManager.LoadScene("Main Game Map");
    }

}
