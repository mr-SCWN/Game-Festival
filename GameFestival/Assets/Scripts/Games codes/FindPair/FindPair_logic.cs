using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;

public class FindPair_logic : MonoBehaviour
{
    [Header("Betting UI")]
    public BetUI betUI;
    public float winCoefficient = 2f;

    [Header("Cursor")]
    [Tooltip("Cursor object that indicates current selection")]
    public GameObject cursor;

    [Header("Card SetUp")]
    public Cards cardPrefab;   // Prefab of the cards
    public Sprite[] cardFaces;  // 18 unique card faces
    public Sprite cardBack;    // 1 card back

    [Header("Grid SetUp")]
    public int rows = 4;
    public int columns = 9;
    public float spacingX = 2.0f;
    public float spacingY = 2.0f;

    [Header("Timer UI")]
    public TMP_Text timerText; 

    //For selecting sards with WASD
    private Cards[,] grid;
    private int selectedRow = 0;
    private int selectedColumn = 0;

    // For matching logic
    private Cards firstCard = null;
    private Cards secondCard = null;
    private bool canFlip = true;
    private int pairsFound = 0;
    private int totalPairs = 18;

    private int currentBet;
    private float remainingTime = 120f; // 2 min
    private bool gameOver = false;

    void Start()
    {
        // сначала окно ставки
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
        // создаём перемешанную колоду пар
        List<int> deck = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            deck.Add(i);
            deck.Add(i);
        }
        for (int i = 0; i < deck.Count; i++)
        {
            int r = Random.Range(i, deck.Count);
            var tmp = deck[i];
            deck[i] = deck[r];
            deck[r] = tmp;
        }

        // рендерим сетку
        grid = new Cards[rows, columns];
        int idx = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var card = Instantiate(cardPrefab, transform);
                float x = c * spacingX - (columns - 1) * spacingX / 2;
                float y = -r * spacingY + (rows - 1) * spacingY / 2;
                card.transform.localPosition = new Vector3(x-2f, y, 0);
                card.CardID = deck[idx];
                card.frontSprite = cardFaces[deck[idx]];
                card.backSprite = cardBack;
                card.ShowBack();
                grid[r, c] = card;
                idx++;
            }
        }

        selectedRow = selectedColumn = 0;
        HighlightSelected();

        canFlip = true;
        pairsFound = 0;
        gameOver = false;

        // запускаем таймер
        StartCoroutine(Countdown());
    }
    IEnumerator Countdown()
    {
        while (remainingTime > 0f && !gameOver)
        {
            remainingTime -= Time.deltaTime;
            int sec = Mathf.CeilToInt(remainingTime);
            timerText.text = $"Time: {sec / 60:D2}:{sec % 60:D2}";
            yield return null;
        }
        if (!gameOver)
        {
            // время вышло — поражение
            gameOver = true;
            StartCoroutine(FinishAndReturn(false));
        }
    }

    void Update()
    {
        if (!canFlip) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) MoveSelection(-1, 0);   // move with wasd
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelection(0, -1);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) MoveSelection(+1, 0);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveSelection(0, +1);

        if (Input.GetKeyDown(KeyCode.E)) FlipSelectedCard();    // select card

        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameOver = true;
            StartCoroutine(FinishAndReturn(false));
        }
    }

    void MoveSelection(int deltaRow, int deltaColumn)
    {      // movement logic
        grid[selectedRow, selectedColumn].SetHighlight(false);
        selectedRow += deltaRow;
        selectedColumn += deltaColumn;

        if (selectedRow < 0) selectedRow = rows - 1;
        if (selectedRow >= rows) selectedRow = 0;
        if (selectedColumn < 0) selectedColumn = columns - 1;
        if (selectedColumn >= columns) selectedColumn = 0;

        HighlightSelected();
    }

    void HighlightSelected()
    {       // Highlighting cards
        grid[selectedRow, selectedColumn].SetHighlight(true);
        if (cursor != null)
        {
            Vector3 offset = new Vector3(0.0f, -0.75f, 0.0f);
            cursor.transform.position = grid[selectedRow, selectedColumn].transform.position + offset;
        }
    }

    void FlipSelectedCard()
    {        // flip logic
        Cards card = grid[selectedRow, selectedColumn];
        if (!card.gameObject.activeSelf) return;    // Ignore if the card is already removed
        if (card.isFlipped) return;             // Ignore if the card is already flipped
        card.Flip();

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            canFlip = false;
            Invoke("CheckMatch", 1.0f);
        }
    }

    void CheckMatch()
    {          // logic for checking pairs
        if (firstCard.CardID == secondCard.CardID)
        {     // if guess was correct
            firstCard.gameObject.SetActive(false);
            secondCard.gameObject.SetActive(false);
            pairsFound += 1;

            if (pairsFound >= totalPairs)
            {
                gameOver = true;
                StartCoroutine(FinishAndReturn(true));
                return; ;
            }
        }
        else
        {        // if guess wasnt correct
            firstCard.Flip();
            secondCard.Flip();
        }

        firstCard = null;       // reset selestion variables
        secondCard = null;
        canFlip = true;
    }
    
        IEnumerator FinishAndReturn(bool playerWin)
    {
        canFlip = false;
        yield return new WaitForSeconds(3.0f);

        if (playerWin)
        {
            int payout = Mathf.FloorToInt(currentBet * winCoefficient);
            CurrencyManager.Instance.AddCoin(payout);
        }
        // при поражении ничего не возвращаем

        GlobalGameState.comingFromMiniGame = true;
        GlobalGameState.spawnPosition      = new Vector2(-15.40047f, 1.793956f);
        SceneManager.LoadScene("Main Game Map");
    }
}
