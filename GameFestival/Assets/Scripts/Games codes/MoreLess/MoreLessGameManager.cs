using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEditor.ShaderGraph.Internal;
using System.Diagnostics;

public class MoreLessGameManager : MonoBehaviour
{
    [Header ("UI Elements")]
    [Tooltip ("Image showing the current open card on the table")]
    public Image currentCardImage;

    [Tooltip ("Image showing deck")]
    public Image deckImage;
    
    [Tooltip ("Array of deck sprites")]
    public Sprite[] deckSprites;
    
    [Tooltip ("Text to show Player Score")]
    public Text playerScoreText;
    
    [Tooltip ("Text to show AI Score")]
    public Text aiScoreText;
    
    [Tooltip ("Text for informational messages")]
    public Text infoText;

    [Header("Option UI")]
    [Tooltip("UI object representing the 'More' option")]
    public GameObject moreOption;
    
    [Tooltip("UI object representing the 'Less' option")]
    public GameObject lessOption;

    [Tooltip("Cursor object that indicates the current selection")]
    public GameObject cursor;

    [Header("AI Display")]
    [Tooltip("Image showing the AI's last chosen button (for visual feedback)")]
    public Image aiChoiceImage;
    [Tooltip("Sprite for AI 'More' selection")]
    public Sprite aiMoreSprite;
    [Tooltip("Sprite for AI 'Less' selection")]
    public Sprite aiLessSprite;

    [Header ("Card Sprites")]
    [Tooltip("Array of 52 card sprites (0 to 51)")]
    public Sprite[] cardSprites;

    [Header ("Game Settings")]
    public int maxCards = 52;
    public float aiDelay=1.0f;
    public float postTurnDelay = 1.0f;

    private List<int> deck; // savnig number of card (0..51)
    private int currentCard;     // card of table
    private int currentIndex = 0;   // index of the next card 
    private int playerScore = 0;
    private int aiScore = 0;
    private bool playerTurn = true;
    private int currentSelection =0; // 0 - more ; 1 - less 

    void Start()
    {
        InitDeck();
        ShuffleDeck();
        currentCard = deck[currentIndex];
        currentIndex++;
        UpdateCurrentCardUI();
        UpdateDeckUI();
        UpdateScoreUI();

        playerTurn = true;
        infoText.text = "Your turn! Use A/D to select, then press E.";

        currentSelection = 0;
        UpdateChoiseCursor();
    }

    void InitDeck(){
        deck = new List<int>();
        for (int i = 0; i<maxCards; i++){
            deck.Add(i);
        }
    }

    void ShuffleDeck(){
        for (int i = 0; i<maxCards; i++){
            int temp = deck[i];
            int r = Random.Range(i, deck.Count);
            deck[i] = deck[r];
            deck[r] = temp;
        }
    }

    void UpdateCurrentCardUI(){
        if (currentCardImage != null && cardSprites.Length >= maxCards){
            currentCardImage.sprite = cardSprites[currentCard];
        }
    }

    void UpdateDeckUI(){
        if (deckImage!=null && deckSprites!=null && deckSprites.Length >= 3){
            int remaining = deck.Count - currentIndex;
            if (remaining > 30){
                deckImage.sprite = deckSprites[0];
            } else if (remaining > 10) {
                deckImage.sprite = deckSprites[1];
            } else {
                deckImage.sprite = deckSprites[2];
            }
        }
    }

    void UpdateScoreUI(){
        if (playerScoreText != null){
            playerScoreText.text = "Player: " + playerScore;
        }
        if (aiScoreText != null){
            aiScoreText.text = "Enemy: " + aiScore;
        }
    }

    void UpdateChoiseCursor(){
        if (cursor != null ){
            Vector3 offset = new Vector3(0f, -20f, 0f);
            if (currentSelection == 0 && moreOption!=null){
                cursor.transform.position = moreOption.transform.position + offset;
            } else if (currentSelection == 1 && lessOption!=null){
                cursor.transform.position = lessOption.transform.position + offset;
            }
        }
    }

    void Update()
    {
        if (playerTurn){
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
                currentSelection--;
                if (currentSelection < 0){
                    currentSelection=1;
                }
                UpdateChoiseCursor();
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
                currentSelection++;
                if (currentSelection > 1){
                    currentSelection = 0;
                }
                UpdateChoiseCursor();
            } else if (Input.GetKeyDown(KeyCode.E)){
                ConfirmPlayerGuess();
            }
        }
        if (Input.GetKeyDown(KeyCode.Q)){
            GlobalGameState.comingFromMiniGame = true;
            GlobalGameState.spawnPosition = new Vector2(-24.0f, 2.0f);
            SceneManager.LoadScene("Main Game Map");
        }
    }

    void ConfirmPlayerGuess(){
        bool guessMore = (currentSelection == 0);
        ProcessPlayerGuess(guessMore);
    }

    void ProcessPlayerGuess(bool guessMore){
        if (currentIndex >= deck.Count){
            EndGame();
            return ;
        }
        int nextCard = deck[currentIndex];
        currentIndex++;
        int currentRank = currentCard%13;
        int nextRank = nextCard%13;
        bool correct = false;

        if (nextRank == currentRank){
            correct = false;
        } else if (guessMore){
            correct = (nextRank > currentRank);
        } else {
            correct = (nextRank < currentRank);
        }

        if (correct){
            playerScore++;
            infoText.text = "Correct! Your turn continues.";
        } else {
            infoText.text = "Wrong! Turn passes to enemy.";
            playerTurn = false;
            Invoke("AIMove", aiDelay);
        }
        
        currentCard = nextCard;
        UpdateCurrentCardUI();
        UpdateDeckUI();
        UpdateScoreUI();
        CheckDeckEnd();
    }

    bool DecideAIGuess(){
        int currentRank = currentCard % 13;
    int countMore = 0;
    int countLess = 0;
    
    for (int i = currentIndex; i < deck.Count; i++)
    {
        int rank = deck[i] % 13;
        if (rank > currentRank)
            countMore++;
        else if (rank < currentRank)
            countLess++;
    }
    
    if (countMore > countLess)
        return true;  // choose More
    else if (countLess > countMore)
        return false; // choose Less
    else
        return (Random.Range(0, 2) == 0);
    }

    void AIMove(){
        if (currentIndex >= deck.Count){
            return;
        }
        infoText.text = "Enemy is thinking";
        bool aiGuessMore = DecideAIGuess();
        if (aiChoiceImage != null){
            aiChoiceImage.sprite = aiGuessMore ? aiMoreSprite : aiLessSprite;
            aiChoiceImage.enabled = true;
        } 
        Invoke("ProcessAIMove", postTurnDelay);
    }

    void ProcessAIMove(){
        if (currentIndex >= deck.Count){
            return;
        }
        int nextCard = deck[currentIndex];
        currentIndex++;
        int currentRank = currentCard%13;
        int nextRank = nextCard%13;
        bool aiGuessMore = DecideAIGuess();
        bool correct = false;

        if (nextRank == currentRank){
            correct = false;
        } else if (aiGuessMore){
            correct = ( nextRank > currentRank);
        } else {
            correct = (nextRank < currentRank);
        }

        if (correct) {
            aiScore++;
            infoText.text = "AI guessed correctly! Your turn now.";
        } else {
            infoText.text = "AI guessed wrong! Your turn.";
        }
        
        if (aiChoiceImage != null)
            aiChoiceImage.enabled = false;
        
        currentCard = nextCard;
        UpdateCurrentCardUI();
        UpdateDeckUI();
        UpdateScoreUI();
        
        playerTurn = true;
        CheckDeckEnd();
    }

    void CheckDeckEnd(){
        if (currentIndex >= deck.Count){
            EndGame();
        }
    }

    void EndGame(){
        string result = "";
        if (playerScore > aiScore){
            result = "You win!";
        } else if (playerScore <aiScore){
            result = "You loose!";
        } else {
            result = "Draw!";
        }

        infoText.text = $"Game Over!\nFinal Score: Player {playerScore} - AI {aiScore}\n{result}";
        moreOption.SetActive(false);
        lessOption.SetActive(false);
        Invoke("ReturnToMain", 3f);
    }

    void ReturnToMain()
    {
        GlobalGameState.comingFromMiniGame = true;
        GlobalGameState.spawnPosition = new Vector2(-24.0f, 2.0f);
        SceneManager.LoadScene("Main Game Map");
    }
}
