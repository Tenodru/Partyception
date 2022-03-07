using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles gameplay and question looping for AYSTTC.
/// </summary>
public class GameManagerAYSTTC : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip ("The number of rounds, or questions, to play.")]
    public int roundCount = 5;
    [Tooltip ("Timer duration in seconds.")]
    public float timerDuration = 15f;
    [Tooltip("The question categories.")]
    public List<QuestionCategory> categories;
    [Header ("UI")]
    [Tooltip("The start button.")]
    public Button startButton;

    [HideInNormalInspector] public float timeRemaining = 15f;
    [HideInNormalInspector] public bool timerBegun = false;
    [HideInNormalInspector] public bool isTimerRunning = false;
    [HideInNormalInspector] public bool roundInProgress = false;
    [HideInNormalInspector] public int currentRound = 0;
    [HideInNormalInspector] public QuestionCategory chosenCategory;

    public static GameManagerAYSTTC current;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Host must choose a category. This is done through Buttons.
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Grab random question from questions list.
        //TODO: Initialize round.
        //TODO: Do pre-timer stuff (like a "ready-set-go").
        //TODO: Initialize timer.
        //TODO: Display question and answer choices.

        //TODO: When timer ends, check if player's chosen answer matches correct answer.
        //TODO: Reward correct players.
        //TODO: Eliminate incorrect players.
        //TODO: Repeat loop until end.

        if (roundInProgress)
        {
            if (!timerBegun)
            {
                isTimerRunning = true;
                StartCoroutine(Timer());
                timerBegun = true;
            }
        }
    }

    /// <summary>
    /// Selects the specified Category as the chosen question Category.
    /// </summary>
    /// <param name="category"></param>
    public void ChooseCategory(QuestionCategory category)
    {
        chosenCategory = category;
        ShowStartButton();
    }

    /// <summary>
    /// Selects a random category.
    /// </summary>
    public void ChooseRandomCategory()
    {
        // Choose a category at random.
        int catIndex = Random.Range(0, categories.Count);
        chosenCategory = categories[catIndex];

        ShowStartButton();
    }

    /// <summary>
    /// Shows or de-greyifies the Start Game button.
    /// </summary>
    public void ShowStartButton()
    {
        // Perhaps have a "Start Game" button that is greyed out,
        // then made clickable when category is chosen.
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        timeRemaining = timerDuration;
        currentRound = 1;

        roundInProgress = true;
    }

    /*
    void Timer()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining = -Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
            }
        }
    }*/
    
    IEnumerator Timer()
    {
        Debug.Log("Timer begun.");
        while (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
            }
            yield return null;
        }
    }
}
