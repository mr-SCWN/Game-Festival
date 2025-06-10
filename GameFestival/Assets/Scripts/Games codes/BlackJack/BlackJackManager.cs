using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TwentyOneManager : MonoBehaviour
{
    [Header("Betting UI")]
    public BetUI betUI;                   
    public float winCoefficient = 1.4f;   // payout on win
    public float tieCoefficient = 1.0f;   // payout on tie

    [Header("Deck Sprites (0–51)")]
    public Sprite[] cardSprites; // card sprites

    [Header("UI Containers")]
    public Transform playerHandContainer; // empty GameObject for PlayerPanel
    public Transform aiHandContainer;     // empty GameObject for AI

    [Header("Card Template")]
    public Image cardTemplate; 

    [Header("Options UI")]
    public GameObject hitOption; 
    public GameObject standOption; 
    public GameObject cursorImage; 
    public Vector3   cursorOffset; 

    [Header("Texts")]
    public TMP_Text playerSumText;
    public TMP_Text aiSumText;
    public TMP_Text infoText;

    [Header("Settings")]
    public float aiDelay = 0.5f;    // delay for AI

    // internal state
    private List<int> deck;
    private int currentIndex;

    private List<Image> playerCards = new List<Image>();
    private List<Image> aiCards     = new List<Image>();

    private int playerSelection = 0; // 0=Hit,1=Stand
    private bool playerTurn = true;
    private bool gameOver = false;
    private bool playerBust = false;
    private bool aiBust = false;
    private int currentBet = 0; 

        void Start()
    {
                // showing Bet window
        int coins = CurrencyManager.Instance.coin;
        betUI.Show(coins, bet => {
            currentBet = bet;                       
            CurrencyManager.Instance.coin -= bet;      
            BeginRound();                              
        });
    }
    void BeginRound()
    {
        // 1) build and shuffle deck
        deck = new List<int>();
        for(int i=0;i<52;i++) deck.Add(i);
        Shuffle(deck);
        currentIndex = 0;

        // 2) initial deal: one card each
        DealCard(playerHandContainer, playerCards);
        DealCard(aiHandContainer,     aiCards);
        UpdateSums();

        // 3) hide template
        cardTemplate.gameObject.SetActive(false);

        // 4) position cursor
        UpdateCursor();

        infoText.text = "Your turn: Hit or Stand";

    }

    void Update()
    {
        if (gameOver) 
        {
            if (Input.GetKeyDown(KeyCode.Q)) StartCoroutine(FinishAndReturn(false));
            return;
        }

        if (!playerTurn) return;

        // A/D toggles Hit/Stand
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerSelection = 1 - playerSelection;
            UpdateCursor();
        }

        // E confirms choice
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerSelection == 0)   // Hit
                OnPlayerHit();
            else                         // Stand
                StartCoroutine(AITurn());
        }

        // Q also can exit
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(FinishAndReturn(false));
    }

    // shuffle in place
    void Shuffle(List<int> list)
    {
        for(int i=0;i<list.Count;i++){
            int r = Random.Range(i, list.Count);
            int tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    // deal one card: instantiate image from template under parent, record it
    void DealCard(Transform parent, List<Image> hand)
    {
        int cardIndex = deck[currentIndex++];
        var img = Instantiate(cardTemplate, parent);
        img.gameObject.SetActive(true);
        img.sprite = cardSprites[cardIndex];
        hand.Add(img);
    }

    void UpdateCursor()
    {
        var target = (playerSelection == 0 ? hitOption : standOption).transform;
        cursorImage.transform.position = target.position + cursorOffset;
    }

    // when player chooses Hit
    void OnPlayerHit()
    {
        DealCard(playerHandContainer, playerCards);
        UpdateSums();

        int sum = BestSum(playerCards);
        if (sum > 21)
        {
            // bust => immediate end
            playerBust = true;
            infoText.text = $"You BUST! Dealer will still draw...";
            playerTurn = false;
            StartCoroutine(AITurn());
        }
        else if (sum == 21)
        {
            // 21 => dealer turn
           playerTurn = false;
            infoText.text = "You got 21! Dealer's turn...";
            StartCoroutine(AITurn());
        }
    }

    // dealer draws until sum>=17
    IEnumerator AITurn()
    {
        infoText.text = "Dealer's turn...";
        yield return new WaitForSeconds(aiDelay);

        // dealer draws until >=17
        int sum;
        while ((sum = BestSum(aiCards)) < 17)
        {
            DealCard(aiHandContainer, aiCards);
            UpdateSums();
            yield return new WaitForSeconds(aiDelay);
        }

        if (sum > 21)
        {
            aiBust = true;
            infoText.text = $"Dealer BUSTS too!";

            DetermineOutcome();
            yield break;
        }
        else
        {
            // dealer stands with sum <=21
            infoText.text = $"Dealer stands at {sum}.";
            DetermineOutcome();
        }
    }

    void DetermineOutcome()
    {
        int playerSum = BestSum(playerCards);
        int dealerSum = BestSum(aiCards);
        bool playerWin, playerTie;
        string result;
        if (playerBust && aiBust)
        {
            playerWin = false; playerTie = true;
            result = "Both BUST—Tie!";
        }
        else if (playerBust)
        {
            playerWin = false; playerTie = false;
            result = $"You BUST ({playerSum})—Dealer wins!";
        }
        else if (aiBust)
        {
            playerWin = true;  playerTie = false;
            result = $"Dealer BUST ({dealerSum})—You win!";
        }
        else
        {
            if (dealerSum > playerSum)
            {
                playerWin = false; playerTie = false;
                result = $"Dealer wins ({dealerSum} vs {playerSum})";
            }

            else if (dealerSum < playerSum)
            {
                playerWin = true; playerTie = false;
                result = $"You win! ({playerSum} vs {dealerSum})";
            }
            else
            {
                playerWin = false; playerTie = true;
                result = $"Tie at {playerSum}";
            }
                
        }

        infoText.text = result;
        gameOver = true;
        StartCoroutine(FinishAndReturn(playerWin, playerTie));
    }

      IEnumerator FinishAndReturn(bool playerWin, bool playerTie = false)
    {
        // delay
        yield return new WaitForSeconds(4.5f);

        
        if (playerWin)
        {
            int payout = Mathf.FloorToInt(currentBet * winCoefficient);
            CurrencyManager.Instance.AddCoin(payout);
        }
        else if (playerTie)
        {
            int refund = Mathf.FloorToInt(currentBet * tieCoefficient);
            CurrencyManager.Instance.AddCoin(refund);
        }
        

        GlobalGameState.comingFromMiniGame = true;
        GlobalGameState.spawnPosition      = new Vector2(-15.73607f, 7.956881f);
        SceneManager.LoadScene("Main Game Map");
    }

    // compute best sum for a hand of images, auto Ace=1 or 11
    int BestSum(List<Image> handImages)
    {
        return ComputeBestSum( handImages.ConvertAll(img => cardValueFromSprite(img.sprite)) );
    }

    // given sprite, return card index 0..51 then rank
    int cardValueFromSprite(Sprite s)
    {
      
        for(int i=0;i<cardSprites.Length;i++)
            if(cardSprites[i]==s) 
                return i;
        return 0;
    }

    // compute best sum of ranks with A=1 or 11
    int ComputeBestSum(List<int> cardIndices)
    {
        int sum = 0, aces = 0;
        foreach(int idx in cardIndices)
        {
            int rank = (idx / 4) + 2; // 1..13
            if (rank >= 1 && rank <= 10) sum += rank;
            else if (rank >= 11 && rank <= 13) sum += 10; // J,Q,K
            else // Ace
            {
                sum += 11;
                aces++;
            }
        }
        // downgrade Aces from 11 to 1 while bust
        while(sum > 21 && aces > 0)
        {
            sum -= 10;
            aces--;
        }
        return sum;
    }

    // update both sum texts
    void UpdateSums()
    {
        playerSumText.text = "You: " + BestSum(playerCards);
        aiSumText.text     = "Dealer: " + (playerTurn ? "?" : BestSum(aiCards));
    }

    void ExitToMain()
    {
        GlobalGameState.comingFromMiniGame = true;
        GlobalGameState.spawnPosition = new Vector2(-15.73607f, 7.956881f);
        SceneManager.LoadScene("Main Game Map");
    }
}
