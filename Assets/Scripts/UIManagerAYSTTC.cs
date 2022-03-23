using System.Collections;
using System.Collections.Generic;
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

    public static UIManagerAYSTTC current;

    private void Awake()
    {
        current = this;
    }

    public void Start()
    {
        gameScreen.SetActive(false);
        timerSlider.gameObject.SetActive(false);
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
            outcomeText.text = "Wrong!" + "\n" +
                "You have been eliminated.";
        }
        else if (outcome == OutcomeType.TimeOut)
        {
            outcomeText.text = "You ran out of time!" + "\n" +
                "You have been eliminated.";
        }
    }

    public void DisplayInstructionsScreen(PlayerStatus playerStatus)
    {
        instructionsScreen.SetActive(true);
        if (playerStatus == PlayerStatus.Host)
        {
            hostInstructions.SetActive(true);
            selectionScreen.SetActive(false);
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
}
