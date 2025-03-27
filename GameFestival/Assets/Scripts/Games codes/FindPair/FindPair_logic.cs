using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;

public class FindPair_logic : MonoBehaviour
{
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

    //For selecting sards with WASD
    private Cards[,] grid;
    private int selectedRow = 0;
    private int selectedColumn = 0;

    // For matching logic
    private Cards firstCard = null;
    private Cards secondCard = null;
    private bool canFlip = true;
    private int pairsFound = 0;
    private int totalFound = 18;

    void Start(){
        List<int> deck = new List<int>();
        for (int i =0; i<18; i++){      //creating 36 card deck
            deck.Add(i); deck.Add(i);
        }

        for (int i=0; i<deck.Count; i++){   //shuffle the dech 
            int temp = deck[i];
            int r = Random.Range(i, deck.Count);
            deck[i] = deck[r];
            deck[r] = temp;
        } 

        grid = new Cards[rows, columns];    // crete a 2d array and spawn cards
        int index = 0;
        for (int r =0 ; r<rows; r++){
            for (int c = 0; c<columns; c++){
                Cards newCard = Instantiate(cardPrefab, transform);     // clone new card from prefab
                float xOffset = -10f , yOffset = 3f; 
                float posX = c * spacingX + xOffset;
                float posY = -r * spacingY + yOffset;
                newCard.transform.localPosition = new Vector3(posX, posY, 0.0f);

                int cardID = deck[index];
                index++;
                newCard.CardID = cardID;
                newCard.frontSprite = cardFaces[cardID];
                newCard.backSprite = cardBack;
                newCard.ShowBack();
                grid[r,c] = newCard;
            }
        }
        HighlightSelected();
    }

    void Update()
    {
        if (!canFlip) return;

        if(Input.GetKeyDown(KeyCode.W)) MoveSelection(-1,0);   // move with wasd
        if(Input.GetKeyDown(KeyCode.A)) MoveSelection(0,-1);
        if(Input.GetKeyDown(KeyCode.S)) MoveSelection(+1,0);
        if(Input.GetKeyDown(KeyCode.D)) MoveSelection(0,+1);
        
        if(Input.GetKeyDown(KeyCode.E)) FlipSelectedCard();    // select card

        if(Input.GetKeyDown(KeyCode.Q)) {
            GlobalGameState.comingFromMiniGame = true;
            GlobalGameState.spawnPosition = new Vector2(-18.0f, 2.0f);
            SceneManager.LoadScene("Main Game Map");
        }
    }

    void MoveSelection(int deltaRow, int deltaColumn){      // movement logic
        grid[selectedRow, selectedColumn].SetHighlight(false);
        selectedRow+=deltaRow;
        selectedColumn+=deltaColumn;

        if (selectedRow< 0) selectedRow = rows -1;
        if (selectedRow >= rows) selectedRow = 0;
        if (selectedColumn <0) selectedColumn = columns -1;
        if (selectedColumn >= columns) selectedColumn = 0;

        HighlightSelected(); 
    }

    void HighlightSelected(){       // Highlighting cards
        grid[selectedRow, selectedColumn].SetHighlight(true);
        if (cursor != null) {
            Vector3 offset = new Vector3(0.0f, -0.75f, 0.0f);
            cursor.transform.position = grid[selectedRow, selectedColumn].transform.position + offset;
        }
    }

    void FlipSelectedCard(){        // flip logic
        Cards card  = grid[selectedRow, selectedColumn];
        if (!card.gameObject.activeSelf) return;    // Ignore if the card is already removed
        card.Flip();

        if (firstCard == null){
            firstCard = card;
        } else if (secondCard == null && card!=firstCard){
            secondCard = card;
            canFlip = false;
            Invoke("CheckMatch", 1.0f);
        }
    }

    void CheckMatch(){          // logic for checking pairs
        if (firstCard.CardID == secondCard.CardID){     // if guess was correct
            firstCard.gameObject.SetActive(false);
            secondCard.gameObject.SetActive(false);
            pairsFound+=1;

            if (pairsFound >= totalFound){
                GlobalGameState.comingFromMiniGame = true;
                GlobalGameState.spawnPosition = new Vector2(-18.0f, 2.0f);
                SceneManager.LoadScene("Main Game Map");
            }
        } else {        // if guess wasnt correct
            firstCard.Flip();
            secondCard.Flip();
        }

        firstCard = null;       // reset selestion variables
        secondCard = null;
        canFlip = true;
    }
}
