using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class MoreLessGameManager : MonoBehaviour
{
    [Header("Betting UI")]
    public BetUI betUI;                   
    public float winCoefficient = 1.8f;   // payout on win
    public float tieCoefficient = 1.0f;   // payout on tie

    [Header("UI Elements")]
    public Image currentCardImage;
    public Image deckImage;
    public TMP_Text  playerScoreText;
    public TMP_Text  aiScoreText;
    public TMP_Text  infoText;

    [Header("Option UI")]
    public GameObject moreOption;
    public GameObject lessOption;
    public GameObject cursorImage;  // cursor
    public Vector3 cursorOffset;    // offset for cursor

    [Header("AI Feedback (optional)")]
    public TMP_Text aiChoiceText; 

    [Header("Sprites")]
    public Sprite[] cardSprites;    // 52 card sprites
    public Sprite[] deckSprites;    // 3 deck sprites

    [Header("Game Settings")]
    public float aiDelay = 1.0f;
    public float postTurnDelay = 1.0f;

    // logic
    private List<int> deck;
    private int currentIndex;
    private int currentCard;
    private int playerScore;
    private int aiScore;
    private bool lastAIGuessMore;

    private bool playerTurn;
    private int currentSelection; // 0 = More, 1 = Less
    private int currentBet;

 void Start()
    {
        // showing bet window
        int coins = CurrencyManager.Instance.coin;
        betUI.Show(coins, bet =>
        {
            currentBet = bet;
            CurrencyManager.Instance.AddCoin(-bet);
            BeginGame();
        });
    }

    void BeginGame()
    {
        InitDeck();
        ShuffleDeck();
        currentIndex = 0;
        DrawFirstCard();
        UpdateAllUI();

        playerTurn = true;
        currentSelection = 0;
        PositionCursor();
        infoText.text = "Your turn: A/D to select, E to confirm.";
    }


    void InitDeck()
    {
        deck = new List<int>();
        for (int i = 0; i < 52; i++) deck.Add(i);
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int r = Random.Range(i, deck.Count);
            int tmp = deck[i];
            deck[i] = deck[r];
            deck[r] = tmp;
        }
    }

    void DrawFirstCard()
    {
        currentCard = deck[currentIndex++];
    }

    void UpdateAllUI()
    {
        UpdateCardUI();
        UpdateDeckUI();
        UpdateScoreUI();
    }

    void UpdateCardUI()
    {
        currentCardImage.sprite = cardSprites[currentCard];
        Debug.Log($"Showing sprite {cardSprites[currentCard].name} (id={currentCard}, rank={currentCard % 13})");
    }


    void UpdateDeckUI()
    {
        int left = deck.Count - currentIndex;
        if (left > 30) deckImage.sprite = deckSprites[0];
        else if (left > 10) deckImage.sprite = deckSprites[1];
        else deckImage.sprite = deckSprites[2];
    }

    void UpdateScoreUI()
    {
        playerScoreText.text = "Player: " + playerScore;
        aiScoreText.text    = "Opponent: " + aiScore;
    }

    void Update()
    {
        // player turn
        if (playerTurn)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                currentSelection = 1 - currentSelection;
                PositionCursor();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                // 0 = More, 1 = Less
                bool guessMore = (currentSelection == 0);
                HandlePlayerGuess(guessMore);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(FinishAndReturn(false, false));
        }
    }

    void PositionCursor()
    {
        // moving cursor
        var target = (currentSelection == 0 ? moreOption : lessOption).transform;
        cursorImage.transform.position = target.position + cursorOffset;
    }

    void HandlePlayerGuess(bool guessMore)
    {
        if (currentIndex >= deck.Count) return;
        int next = deck[currentIndex++];
        bool correct = CheckGuess(currentCard, next, guessMore);

       // Debug.Log($"Player guessed {(guessMore?"More":"Less")} - oldR={(currentCard%13)}, newR={(next%13)}, correct={correct}");

        if (correct)
        {
            playerScore++;
            infoText.text = "Correct! You keep turn.";
        }
        else
        {
            infoText.text = "Wrong! Opponent's turn.";
            playerTurn = false;
            Invoke(nameof(AITurn), aiDelay);
        }

        currentCard = next;
        UpdateAllUI();
        if (currentIndex >= deck.Count)
        {
            // end of the game
            bool tie = (playerScore == aiScore);
            bool win = (playerScore > aiScore);
            StartCoroutine(FinishAndReturn(win, tie));
        }
    }

    int GetRank(int cardIndex)
    {
        return (cardIndex / 4) + 2;
    }

    bool CheckGuess(int oldCard, int newCard, bool more)
    {
        int oldR = GetRank(oldCard);
        int newR = GetRank(newCard);

        if (newR == oldR) return false;
        return more ? (newR > oldR) : (newR < oldR);
    }

    void AITurn()
    {
        if (currentIndex >= deck.Count) return;
        infoText.text = "Opponent is thinking…";

        // count probabilities
        int oldR = GetRank(currentCard);
        int countMore = 0, countLess = 0;
        for (int i = currentIndex; i < deck.Count; i++)
        {
            int r = deck[i] % 13;
            if (r > oldR) countMore++;
            else if (r < oldR) countLess++;
        }
        lastAIGuessMore = (countMore >= countLess);

        // showing AI chose
        if (aiChoiceText != null)
        {
            aiChoiceText.text = lastAIGuessMore ? "MORE" : "LESS";
            aiChoiceText.gameObject.SetActive(true);
        }

        Invoke(nameof(ProcessAIGuess), postTurnDelay);
    }

    void ProcessAIGuess()
    {
        // hide AI text
        if (aiChoiceText != null)
            aiChoiceText.gameObject.SetActive(false);

        if (currentIndex >= deck.Count) return;

        int next = deck[currentIndex++];
        bool correct = CheckGuess(currentCard, next, lastAIGuessMore);

         Debug.Log($"AI guessed {(lastAIGuessMore?"More":"Less")} - oldR={(currentCard%13)}, newR={(next%13)}, correct={correct}");

        if (correct)
        {
            aiScore++;
            infoText.text = "Opponent was correct! Opponent keeps turn.";
            playerTurn = false;           // **AI continues turn**
            Invoke(nameof(AITurn), aiDelay);
        }
        else
        {
            infoText.text = "Opponent was wrong! Your turn.";
        }

        currentCard = next;
        UpdateAllUI();

        playerTurn = true;
        if (currentIndex >= deck.Count)
        {
            bool tie = (playerScore == aiScore);
            bool win = (playerScore > aiScore);
            StartCoroutine(FinishAndReturn(win, tie));
        }
    }



    IEnumerator FinishAndReturn(bool playerWin, bool playerTie)
        {
            yield return new WaitForSeconds(2.5f);

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
            GlobalGameState.spawnPosition = new Vector2(-21.97706f, 1.37644f);
            SceneManager.LoadScene("Main Game Map");
        }
}
