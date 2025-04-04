using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEditor.ShaderGraph.Internal;

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
        
    }
}
