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
    [Tooltip("The answer choice buttons.")]
    public List<Button> answerButtons;

    public static UIManagerAYSTTC current;

    private void Awake()
    {
        current = this;
    }

    public void Start()
    {
        gameScreen.SetActive(false);
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
        selectionScreen.SetActive(false);
        selectionScreen.SetActive(false);
        hostStatusScreen.SetActive(true);
    }

    public void SetGameStageP()
    {
        participantWaitingScreen.SetActive(false);
        gameScreen.SetActive(true);
    }
}
