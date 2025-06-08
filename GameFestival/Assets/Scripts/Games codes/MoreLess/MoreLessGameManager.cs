using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class MoreLessGameManager : MonoBehaviour
{
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
    public Image aiChoiceImage;
    public Sprite aiMoreSprite;
    public Sprite aiLessSprite;

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

    void Start()
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
        aiScoreText.text    = "AI: " + aiScore;
    }

    void Update()
    {
        if (playerTurn)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentSelection = (currentSelection + 1) % 2; // changing 0 - 1
                PositionCursor();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentSelection = (currentSelection + 1) % 2;
                PositionCursor();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                HandlePlayerGuess(currentSelection == 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
            ExitToMain();
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

        if (correct)
        {
            playerScore++;
            infoText.text = "Correct! You keep turn.";
        }
        else
        {
            infoText.text = "Wrong! AI's turn.";
            playerTurn = false;
            Invoke(nameof(AITurn), aiDelay);
        }

        currentCard = next;
        UpdateAllUI();
        CheckEnd();
    }

    bool CheckGuess(int oldCard, int newCard, bool more)
    {
        int oldR = oldCard % 13;
        int newR = newCard % 13;
        if (newR == oldR) return false;
        return more ? (newR > oldR) : (newR < oldR);
    }

    void AITurn()
    {
        if (currentIndex >= deck.Count) return;
        infoText.text = "AI is thinking…";

        // counting probabilities
        int oldR = currentCard % 13;
        int countMore = 0, countLess = 0;
        for (int i = currentIndex; i < deck.Count; i++)
        {
            int r = deck[i] % 13;
            if (r > oldR) countMore++;
            else if (r < oldR) countLess++;
        }
        lastAIGuessMore = (countMore >= countLess);

        // Show AI choose
        if (aiChoiceImage != null)
        {
            aiChoiceImage.sprite = lastAIGuessMore ? aiMoreSprite : aiLessSprite;
            aiChoiceImage.enabled = true;
        }

        Invoke(nameof(ProcessAIGuess), postTurnDelay);
    }


    void ProcessAIGuess()
    {
        aiChoiceImage.enabled = false;
        if (currentIndex >= deck.Count) return;

        int next = deck[currentIndex++];
        bool correct = CheckGuess(currentCard, next, lastAIGuessMore);

        if (correct)
        {
            aiScore++;
            infoText.text = "AI was correct! Your turn.";
        }
        else
        {
            infoText.text = "AI was wrong! Your turn.";
        }

        currentCard = next;
        UpdateAllUI();
        playerTurn = true;
        CheckEnd();
    }


    void CheckEnd()
    {
        if (currentIndex >= deck.Count)
        {
            string res = playerScore > aiScore ? "You Win!" :
                         playerScore < aiScore ? "AI Wins!" : "Draw!";
            infoText.text = $"Game Over!\nPlayer {playerScore} – AI {aiScore}\n{res}";
            // block moving
            playerTurn = false;
        }
    }

    void ExitToMain()
    {
        SceneManager.LoadScene("MainGameScene");
    }
}
