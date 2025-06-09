using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class RepeatMelody_logic : MonoBehaviour
{
    [Header("Frog SetUp")]
    [Tooltip("Array of 4 frog objects")]
    public Frog[] frogs;

    [Header("Cursor")]
    [Tooltip("Cursor object that indicates current selection")]
    public GameObject cursor;

    [Header("Outline Colors")]
    public Color normalColor = Color.white;
    public Color sequenceColor = Color.yellow;
    public Color correctColor = Color.green * 2f;
    public Color wrongColor   = Color.red * 1f;

    [Header("Timing Settings")]
    public float highlightDuration  = 0.3f;
    public float delayBetweenFrogs = 0.3f;

    [Header("Game Settings")]
    public int maxLevel = 10;
    public int lives    = 3;

    [Header("UI")]
    public TMP_Text infoText; 

    // internal
    private List<int> sequence       = new List<int>();
    private int       currentLevel   = 1;
    private int       playerInputIdx = 0;
    private bool      inputEnabled   = false;
    private bool      rewardGiven    = false;

    private int currentSelectionIdx = 0;

    void Start()
    {
       
        foreach (var f in frogs) f.SetOutlineColor(normalColor);
        UpdateCursor();
        StartCoroutine(StartLevel());
    }

    IEnumerator StartLevel()
    {
        inputEnabled      = false;
        playerInputIdx    = 0;
        int newFrog       = Random.Range(0, frogs.Length);
        sequence.Add(newFrog);
        yield return StartCoroutine(PlaySequence());
        inputEnabled = true;
    }

    IEnumerator PlaySequence()
    {
        foreach (int idx in sequence)
        {
            frogs[idx].SetOutlineColor(frogs[idx].GetDarkerOutline(0.5f));
            yield return new WaitForSeconds(highlightDuration);
            frogs[idx].SetOutlineColor(normalColor);
            yield return new WaitForSeconds(delayBetweenFrogs);
        }
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveSelection(-1);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveSelection(+1);
        else if (Input.GetKeyDown(KeyCode.E))
            ConfirmSelection();
        else if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(FinishAndExit());  // quit with delay
    }

    void MoveSelection(int delta)
    {
        frogs[currentSelectionIdx].SetOutlineColor(normalColor);
        currentSelectionIdx = (currentSelectionIdx + delta + frogs.Length) % frogs.Length;
        UpdateCursor();
    }

    void UpdateCursor()
    {
        if (cursor != null)
        {
            Vector3 off = new Vector3(0, -0.5f, 0);
            cursor.transform.position = frogs[currentSelectionIdx].transform.position + off;
        }
    }

    void ConfirmSelection()
    {
        if (!inputEnabled) return;

        bool correct = currentSelectionIdx == sequence[playerInputIdx];
        frogs[currentSelectionIdx].SetOutlineColor(correct ? correctColor : wrongColor);
        StartCoroutine(ClearFrogOutline(frogs[currentSelectionIdx], highlightDuration));

        if (correct)
        {
            playerInputIdx++;
            if (playerInputIdx >= sequence.Count)
            {
                // level complited
                if (currentLevel >= maxLevel)
                {
                    Debug.Log("Victory!");
                    StartCoroutine(FinishAndExit());
                }
                else
                {
                    currentLevel++;
                    StartCoroutine(DelayBeforeNextLevel());
                }
            }
        }
        else
        {
            lives--;
            if (lives <= 0)
            {
                Debug.Log("Game Over!");
                StartCoroutine(FinishAndExit());
            }
            else
            {
                StartCoroutine(ResetLevel());
            }
        }
    }

    IEnumerator DelayBeforeNextLevel()
    {
        inputEnabled = false;
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartLevel());
    }

    IEnumerator ClearFrogOutline(Frog f, float t)
    {
        yield return new WaitForSeconds(t);
        f.SetOutlineColor(normalColor);
    }

    IEnumerator ResetLevel()
    {
        inputEnabled = false;
        yield return new WaitForSeconds(1f);
        foreach (var f in frogs) f.SetOutlineColor(normalColor);
        playerInputIdx = 0;
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlaySequence());
        inputEnabled = true;
    }

    /// delay 1.5s, return to main scene
    private IEnumerator FinishAndExit()
    {
        inputEnabled = false;
        if (infoText != null)
            infoText.text = "Returning to world…";

        yield return new WaitForSeconds(1.5f);

        GiveFishReward();

        GlobalGameState.comingFromMiniGame = true;
        GlobalGameState.spawnPosition      = new Vector2(-42f, 20f);
        SceneManager.LoadScene("Main Game Map");
    }

    private void GiveFishReward()
    {
        if (rewardGiven) return;
        rewardGiven = true;

        int completed   = currentLevel - 1;
        int fishToGive  = Mathf.Max(0, completed - 2);

        for (int i = 0; i < fishToGive; i++)
        {
            float r = Random.value;
            if (r < 0.6f)       CurrencyManager.Instance.grey_fish++;
            else if (r < 0.9f)  CurrencyManager.Instance.green_fish++;
            else                CurrencyManager.Instance.gold_fish++;
        }
    }
}
