using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles UI references for AYSTTC.
/// </summary>
public class UIManagerAYSTTC : MonoBehaviour
{
    [Tooltip ("The category selection screen.")]
    public GameObject selectionScreen;
    [Tooltip("The screen participants see during the category selection phase.")]
    public GameObject participantWaitingScreen;
    [Header("Game Screen (H)")]
    [Tooltip("The host's status info screen.")]
    public GameObject hostStatusScreen;
    [Tooltip("The host's numbers to hide answer choice.")]
    public GameObject hostNumbers;
    [Header("Game Screen (P)")]
    [Tooltip("The main game screen.")]
    public GameObject gameScreen;
    [Tooltip("The question display.")]
    public TextMeshProUGUI questionDisplay;
    [Tooltip("The timer displaying the amount of time remaining.")]
    public Slider timerSlider;
    [Tooltip("The answer choice buttons.")]
    public List<AnswerButton> answerButtons;
    [Header("Outcome Screen (P)")]
    [Tooltip("The outcome screen.")]
    public GameObject outcomeScreen;
    [Tooltip("The outcome text.")]
    public TextMeshProUGUI outcomeText;
    [Header("Instructions Screen")]
    [Tooltip("The instructions screen.")]
    public GameObject instructionsScreen;
    [Tooltip("The host's instructions.")]
    public GameObject hostInstructions;
    [Tooltip("The participants's instructions.")]
    public GameObject pInstructions;
    [Header("Pre-Start Screen")]
    [Tooltip("The pre-start screen.")]
    public GameObject preStartScreen;
    [Header("Game End Screen")]
    [Tooltip("The end-of-game screen.")]
    public GameObject gameEndScreen;
    [Tooltip("Displays the number of remaining players.")]
    public TextMeshProUGUI playersRemainingDisplay;

    public static UIManagerAYSTTC current;

    [HideInNormalInspector] public float timeRemaining;  

    private void Awake()
    {
        current = this;
    }

    public void Start()
    {
        gameScreen.SetActive(false);
        timerSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.current.playerStatus == PlayerStatus.Host)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                GameManagerAYSTTC.current.selectedAnswer = answerButtons[0].answer;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                GameManagerAYSTTC.current.selectedAnswer = answerButtons[1].answer;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                GameManagerAYSTTC.current.selectedAnswer = answerButtons[2].answer;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                GameManagerAYSTTC.current.selectedAnswer = answerButtons[3].answer;
            }
        }
    }

    // Selection Stage. ---------------------------------------------------------
    public void SetSelectionStageH()
    {
        selectionScreen.SetActive(true);
        participantWaitingScreen.SetActive(false);
    }

    public void SetSelectionStageP()
    {
        selectionScreen.SetActive(false);
        participantWaitingScreen.SetActive(true);
    }

    // Game Stage. ---------------------------------------------------------
    public void SetGameStageH()
    {
        instructionsScreen.SetActive(false);
        preStartScreen.SetActive(false);
        outcomeScreen.SetActive(false);
        selectionScreen.SetActive(false);
        hostStatusScreen.SetActive(true);
        timerSlider.gameObject.SetActive(true);
    }

    public void SetGameStageP(Question question)
    {
        instructionsScreen.SetActive(false);
        preStartScreen.SetActive(false);
        outcomeScreen.SetActive(false);
        selectionScreen.SetActive(false);
        participantWaitingScreen.SetActive(false);
        gameScreen.SetActive(true);
        timerSlider.gameObject.SetActive(true);

        questionDisplay.text = question.question;
        List<Answer> answerList = new List<Answer>(question.answerList);
        foreach (AnswerButton button in answerButtons)
        {
            int answerIndex = Random.Range(0, answerList.Count - 1);
            button.answer = answerList[answerIndex];
            button.GetComponentInChildren<TextMeshProUGUI>().text = answerList[answerIndex].answer;
            button.GetComponent<Image>().color = button.colors.normalColor;
            answerList.RemoveAt(answerIndex);
            if (GameManager.current.playerStatus == PlayerStatus.Host)
            {
                ColorBlock colorVar = button.colors;
                colorVar.selectedColor = new Color(255, 255, 255);
                button.colors = colorVar;
            }
        }
    }

    public void ShowTimer(float val, float maxVal)
    {
        timerSlider.gameObject.SetActive(true);
        timerSlider.maxValue = maxVal;
        timerSlider.value = val;
    }

    public void SelectAnswerChoice(AnswerButton answerChoice)
    {
        if (GameManager.current.playerStatus == PlayerStatus.Host)
        {
            return;
        }
        foreach (AnswerButton button in answerButtons)
        {
            //button.GetComponent<Image>().color = Color.white;
            button.GetComponent<Image>().color = button.colors.normalColor;
        }
        //answerChoice.GetComponent<Image>().color = Color.yellow;
        answerChoice.GetComponent<Image>().color = answerChoice.colors.selectedColor;

        GameManagerAYSTTC.current.selectedAnswer = answerChoice.answer;
    }

    public void DisplayOutcomeScreen(OutcomeType outcome)
    {
        outcomeScreen.SetActive(true);
        gameScreen.SetActive(false);

        if (outcome == OutcomeType.Correct)
        {
            outcomeText.text = "Correct!" + "\n" +
                "Awaiting next question.";
        }
        else if (outcome == OutcomeType.Wrong)
        {
            if (GameManager.current.playerStatus == PlayerStatus.Host)
            {
                outcomeText.text = "Wrong! You have been eliminated." + "\n" +
                "However, you can stay answering questions because you are the host.";
            }
            else
            {
                outcomeText.text = "Wrong!" + "\n" +
                "You have been eliminated.";
            }
        }
        else if (outcome == OutcomeType.TimeOut)
        {
            if (GameManager.current.playerStatus == PlayerStatus.Host)
            {
                outcomeText.text = "You ran out of time! You have been eliminated." + "\n" +
                "However, you can stay answering questions because you are the host.";
            }
            else
            {
                outcomeText.text = "You ran out of time!" + "\n" +
                "You have been eliminated.";
            }
        }
    }

    public void DisplayInstructionsScreen(PlayerStatus playerStatus)
    {
        instructionsScreen.SetActive(true);
        if (playerStatus == PlayerStatus.Host)
        {
            hostInstructions.SetActive(true);
            selectionScreen.SetActive(false);
            hostNumbers.SetActive(true);
        }
        else
        {
            pInstructions.SetActive(true);
        }
    }

    public void DisplayPreStartScreen()
    {
        preStartScreen.SetActive(true);
        participantWaitingScreen.SetActive(false);
        instructionsScreen.SetActive(false);
    }

    public void DisplayGameEndScreen(PlayerStatus playerStatus)
    {
        gameEndScreen.SetActive(true);
        // TODO: Display number of remaining players.
        // TODO: Display eliminated players (in memoriam).
        // TODO: Host should have Create New Lobby button.
        StartCoroutine(_Timer(2f, () =>
        {
            playersRemainingDisplay.text = "You and " + GameManagerAYSTTC.current.remainingPlayerCount + " others made it to the end!";
            playersRemainingDisplay.gameObject.SetActive(true);
        }));
    }

    /// <summary>
    /// Timer used for UI elements.
    /// </summary>
    /// <param name="startTime">The starting time.</param>
    /// <returns></returns>
    IEnumerator _Timer(float startTime, Action func = null)
    {
        Debug.Log("UI Timer begun.");
        timeRemaining = startTime;
        while (true)
        {
            UIManagerAYSTTC.current.ShowTimer(timeRemaining, startTime);                                               // Keeps the timer slider display updated.
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                if (func != null)
                {
                    func.Invoke();
                }
            }
            yield return null;
        }
    }
}
