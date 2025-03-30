using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class RepeatMelody_logic : MonoBehaviour{
    [Header("Frog SetUp")]
    [Tooltip("Array of 4 frog objects")]
    public Frog[] frogs;

    [Header("Cursor")]
    [Tooltip("Cursor object that indicates current selection")]
    public GameObject cursor;

    [Header("Outline Colors")]
    public Color normalColor = Color.white;     // normal outline
    public Color sequenceColor = Color.yellow;  // when playing the sequence
    public Color correctColor = Color.green *2.0f;    // when choosing correctly
    public Color wrongColor = Color.red*2.0f ;        // when choosing incorrectly

    [Header ("Timing Settings")]
    public float highlightDuration = 0.3f;
    public float delayBetweenFrogs = 0.3f;

    [Header("Game Settings")]
    public int maxLevel = 10;
    public int lives = 3;

    // sequence logic
    private List<int> sequence = new List<int>();
    private int currentLevel = 1;
    private int playerInputIndex = 0;
    private bool inputEnabled = false;

    // choosing player
    private int currentSelectionIndex = 0;

    void Start()
    {
        foreach (Frog frog in frogs){
            frog.SetOutlineColor(normalColor);
        }
        currentSelectionIndex =0;
        UpdateCursor();
        StartCoroutine(StartLevel());
    }

    IEnumerator StartLevel(){
        inputEnabled = false;
        playerInputIndex = 0;
        int newFrog = Random.Range(0, frogs.Length);    // adding new element for sequence
        sequence.Add(newFrog);
        yield return StartCoroutine(PlaySequence());    // Playing the sequence
        inputEnabled = true;
    }

    IEnumerator PlaySequence(){
        foreach (int frogIndex in sequence){
            Color darkColor = frogs[frogIndex].GetDarkerOutline(0.5f);
            frogs[frogIndex].SetOutlineColor(darkColor);
            yield return new WaitForSeconds(highlightDuration);
            frogs[frogIndex].SetOutlineColor(frogs[frogIndex].baseOutlineColor);
            yield return new WaitForSeconds(delayBetweenFrogs);
        }
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
            MoveSelection(-1);
        } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
            MoveSelection(+1);
        } else if (Input.GetKeyDown(KeyCode.E)){
            ConfirmSelection();
        } else if (Input.GetKeyDown(KeyCode.Q)) {
            GlobalGameState.comingFromMiniGame = true;
            GlobalGameState.spawnPosition = new Vector2(-42.0f, 20.0f);
            SceneManager.LoadScene("Main Game Map");
        }
    }

    void MoveSelection(int delta){
        frogs[currentSelectionIndex].SetOutlineColor(normalColor);
        currentSelectionIndex += delta;
        if (currentSelectionIndex < 0 ){
            currentSelectionIndex = frogs.Length-1;
        }
        if (currentSelectionIndex >= frogs.Length){
            currentSelectionIndex = 0;
        }
        UpdateCursor();
    }

    void UpdateCursor(){
        if (cursor!=null){
            Vector3  offset = new Vector3(0.0f, -0.5f, 0.0f);
            cursor.transform.position = frogs[currentSelectionIndex].transform.position+offset; 
        }
    }

    void ConfirmSelection(){
        if (!inputEnabled) return;
        if (currentSelectionIndex == sequence[playerInputIndex]){    // if correct shows green outline
            frogs[currentSelectionIndex].SetOutlineColor(correctColor);
            StartCoroutine(ClearFrogOutline(frogs[currentSelectionIndex], highlightDuration));
            playerInputIndex++;

            if(playerInputIndex >= sequence.Count){     // end game or next level
                if (currentLevel >= maxLevel){
                    Debug.Log("Victory!");
                    GlobalGameState.comingFromMiniGame = true;
                    GlobalGameState.spawnPosition = new Vector2(-42.0f, 20.0f);
                    SceneManager.LoadScene("Main Game Map");
                } else {
                    currentLevel++;
                    StartCoroutine(DelayBeforeNextLevel());
                }
            }
        } else {        // if wrong shows red outline
            frogs[currentSelectionIndex].SetOutlineColor(wrongColor);
            lives--;
            if (lives <= 0){        // end game or retry
                Debug.Log("Game Over!");
                GlobalGameState.comingFromMiniGame = true;
                GlobalGameState.spawnPosition = new Vector2(-42.0f, 20.0f);
                SceneManager.LoadScene("Main Game Map");
            } else {
                StartCoroutine(ResetLevel());
            }
        }
    }

    IEnumerator DelayBeforeNextLevel(){
        inputEnabled = false;                 // input block
        yield return new WaitForSeconds(1.0f);  //delay
        StartCoroutine(StartLevel());
    }

    IEnumerator ClearFrogOutline(Frog frog, float delay){
        yield return new WaitForSeconds(delay);
        frog.SetOutlineColor(frog.baseOutlineColor);
    }

    IEnumerator ResetLevel(){
        inputEnabled = false;
        yield return new WaitForSeconds(1.0f);
        foreach(Frog frog in frogs){
            frog.SetOutlineColor(normalColor);
        }
        playerInputIndex = 0;
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(PlaySequence());
        inputEnabled = true;
    }
}