using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Handles UI references for AYSTTC.
/// </summary>
public class UIManagerAYSTTC : MonoBehaviour
{
    public Image bgBrightness;

    [Tooltip ("The category selection screen.")]
    public GameObject selectionScreen;
    public List<Button> categoryButtons;
    public List<QuestionCategory> categories;
    public Slider maxRoundsSlider;
    public Slider roundTimeSlider;
    public Slider difficultySlider;
    public TextMeshProUGUI maxRoundsText;
    public TextMeshProUGUI roundTimeText;
    public TextMeshProUGUI difficultySliderText;
    [Tooltip("The screen participants see during the category selection phase.")]
    public GameObject participantWaitingScreen;
    public GameObject hoverOverlay;
    public GameObject infoBox;
    public Toggle hideHostAnswerCheckbox;
    public TextMeshProUGUI hideHostAnswersText;
    public bool hideHostAnswer = false;

    [Header("Game Screen (H)")]
    [Tooltip("The host's status info screen.")]
    public GameObject hostStatusScreen;
    [Tooltip("The host's numbers to hide answer choice.")]
    public GameObject hostNumbers;
    public AudioClip buttonPressSound;

    [Header("Game Screen (P)")]
    [Tooltip("The main game screen.")]
    public GameObject gameScreen;
    [Tooltip("The question display.")]
    public TextMeshProUGUI questionNum;
    public TextMeshProUGUI questionDisplay;
    [Tooltip("The timer displaying the amount of time remaining.")]
    public TextMeshProUGUI timerText;
    public Slider timerSlider;
    [Tooltip("The answer choice buttons.")]
    public List<AnswerButton> answerButtons;
    [Tooltip("Spectate filter panel.")]
    public GameObject hostSpectatePanel;

    [Header("Outcome Screen (P)")]
    [Tooltip("The outcome screen.")]
    public GameObject outcomeScreen;
    [Tooltip("The outcome text.")]
    public TextMeshProUGUI outcomeText;
    public Animator outcomeAnim;

    public int numRemainingPlayers;
    public Text remainingPlayerCount;
    public GameObject playerList;
    public Transform playerHolder;
    public List<PlayerCard> playerCards;
    public int playerCardSize;

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
    public Animator categorySelectAnimation;
    public AudioClip twitchIntro;
    public AudioClip filmIntro;
    public AudioClip gamesIntro;
    public AudioClip allIntro;

    [Header("Game End Screen")]
    [Tooltip("The end-of-game screen.")]
    public GameObject gameEndScreen;
    [Tooltip("Displays the number of remaining players.")]
    public TextMeshProUGUI playersRemainingDisplay;
    [Tooltip("Displays the congrats splash text.")]
    public TextMeshProUGUI congratsText;

    [Header("Game Recap Screen")]
    public GameObject gameRecapScreen;
    public Transform questionHolder;
    public List<QuestionCard> questionCards;

    public static UIManagerAYSTTC current;

    [HideInNormalInspector] public float timeRemaining;
    [HideInNormalInspector] public int playerCount;

    private void Awake()
    {
        current = this;
    }

    public void Start()
    {
        gameScreen.SetActive(false);
        timerSlider.gameObject.SetActive(false);
        gameEndScreen.SetActive(false);

        for(int i = 0; i < questionHolder.childCount; i++)
        {
            questionCards.Add(questionHolder.GetChild(i).GetComponent<QuestionCard>());
        }

        for (int i = 0; i < playerHolder.childCount; i++)
        {
            playerCards.Add(playerHolder.GetChild(i).GetComponent<PlayerCard>());
        }
    }

    public IEnumerator Timer(Action<bool> assigner, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        assigner(true);
    }

    private void Update()
    {
        if (GameManager.current.playerStatus == PlayerStatus.Host && !GameManagerAYSTTC.current.hostEliminated && hideHostAnswer)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                GameManagerAYSTTC.current.selectedAnswer = answerButtons[0].answer;
                GameManagerAYSTTC.current.ChooseAnswer(answerButtons[0].answer);
                AudioManager.current.PlaySound(buttonPressSound);
                Background.current.PlayParticleEffect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                GameManagerAYSTTC.current.selectedAnswer = answerButtons[1].answer;
                GameManagerAYSTTC.current.ChooseAnswer(answerButtons[1].answer);
                AudioManager.current.PlaySound(buttonPressSound);
                Background.current.PlayParticleEffect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                GameManagerAYSTTC.current.selectedAnswer = answerButtons[2].answer;
                GameManagerAYSTTC.current.ChooseAnswer(answerButtons[2].answer);
                AudioManager.current.PlaySound(buttonPressSound);
                Background.current.PlayParticleEffect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                GameManagerAYSTTC.current.selectedAnswer = answerButtons[3].answer;
                GameManagerAYSTTC.current.ChooseAnswer(answerButtons[3].answer);
                AudioManager.current.PlaySound(buttonPressSound);
                Background.current.PlayParticleEffect();
            }
        }
    }

    /// <summary>
    /// Selects the specified category to pull questions from. Recolors other Category buttons.
    /// </summary>
    /// <param name="category"></param>
    public void ChooseCategory(QuestionCategory category)
    {
        foreach (Button button in categoryButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
        categoryButtons[categories.IndexOf(category)].GetComponent<Image>().color = Color.yellow;
        GameManagerAYSTTC.current.ChooseCategory(category);
    }

    public void HideHostAnswerMode()
    {
        if (hideHostAnswerCheckbox.isOn)
        {
            hideHostAnswersText.color = new Color(0, 0, 0, 1);
            hideHostAnswer = true;
        }
        else
        {
            hideHostAnswer = false;
            hideHostAnswersText.color = new Color(0, 0, 0, 0.5f);
        }
    }

    public void HoverOver(bool active)
    {
        if (active)
        {
            hoverOverlay.SetActive(true);
            infoBox.SetActive(true);
        }
        else
        {
            hoverOverlay.SetActive(false);
            infoBox.SetActive(false);
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

        questionNum.text = "Question " + GameManagerAYSTTC.current.currentRound.ToString();
        questionDisplay.text = question.question;
        List<Answer> answerList = new List<Answer>(question.answerList);
        foreach (AnswerButton button in answerButtons)
        {
            int answerIndex = UnityEngine.Random.Range(0, answerList.Count - 1);
            button.answer = answerList[answerIndex];
            button.GetComponentInChildren<TextMeshProUGUI>().text = answerList[answerIndex].answer;
            button.GetComponent<Image>().color = button.colors.normalColor;
            answerList.RemoveAt(answerIndex);
            if (GameManager.current.playerStatus == PlayerStatus.Host && hideHostAnswer)
            {
                ColorBlock colorVar = button.colors;
                colorVar.selectedColor = new Color(255, 255, 255);
                button.colors = colorVar;
            }
        }

        if (GameManagerAYSTTC.current.hostEliminated)
        {
            hostSpectatePanel.SetActive(true);
        }
    }

    public void ShowTimer(float val, float maxVal)
    {
        timerText.text = Mathf.Ceil(val).ToString();
        //timerSlider.gameObject.SetActive(true);
        //timerSlider.maxValue = maxVal;
        //timerSlider.value = val;
    }

    public void SelectAnswerChoice(AnswerButton answerChoice)
    {
        if (GameManager.current.playerStatus == PlayerStatus.Host && hideHostAnswer)
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
        AudioManager.current.PlaySound(buttonPressSound);
        GameManagerAYSTTC.current.selectedAnswer = answerChoice.answer;
        Background.current.PlayParticleEffect();
    }

    // Intro and Ending Stages. ---------------------------------------------------------
    
    /// <summary>
    /// Displays the Outcome screen after the timer for a question hits 0.
    /// </summary>
    /// <param name="outcome">The Outcome (Correct, TimeOut, Wrong) for the player.</param>
    public void DisplayOutcomeScreen(OutcomeType outcome)
    {
        outcomeScreen.SetActive(true);
        gameScreen.SetActive(false);

        if (outcome == OutcomeType.Correct)
        {
            outcomeText.text = "Correct!" + "\n" +
                "Awaiting next question.";
            outcomeAnim.SetTrigger("Win");
        }
        else if (outcome == OutcomeType.Wrong)
        {
            if (GameManager.current.playerStatus == PlayerStatus.Host)
            {
                outcomeText.text = "Wrong! You have been eliminated." + "\n" +
                "However, you will stay and spectate because you are the host.";
            }
            else
            {
                outcomeText.text = "Wrong!" + "\n" +
                "You have been eliminated.";
            }
            outcomeAnim.SetTrigger("Lose");
        }
        else if (outcome == OutcomeType.TimeOut)
        {
            if (GameManager.current.playerStatus == PlayerStatus.Host)
            {
                outcomeText.text = "You ran out of time! You have been eliminated." + "\n" +
                "However, you will stay and spectate because you are the host.";
            }
            else
            {
                outcomeText.text = "You ran out of time!" + "\n" +
                "You have been eliminated.";
            }
            outcomeAnim.SetTrigger("Lose");
        }
        else if (outcome == OutcomeType.HostSpectate)
        {
            outcomeText.text = "You've already been eliminated." + "\n" +
            "You will continue spectating.";
            
            outcomeAnim.SetTrigger("Lose");
        }

        StartCoroutine(Timer(x => StartCoroutine(ReduceRemainingPlayerCount()), 2f));
        StartCoroutine(Timer(x => StartCoroutine(Memoriam()), 2f));
    }

    public void UpdateEliminatedPlayers(int remainingPlayers, int eliminatedPlayers, List<string> playerNames)
    {
        playerHolder.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -130.2f, 0);
        numRemainingPlayers = remainingPlayers + playerNames.Count;
        remainingPlayerCount.text = numRemainingPlayers.ToString();

        foreach (PlayerCard playerCard in playerCards)
        {
            playerCard.gameObject.SetActive(false);
        }
        for (int i = 0; i < playerNames.Count; i++)
        {
            playerCards[i].gameObject.SetActive(true);
            playerCards[i].AssignPlayerName(playerNames[i]);
        }
    }

    IEnumerator ReduceRemainingPlayerCount()
    {
        int numEliminations = 0;
        foreach (PlayerCard playerCard in playerCards)
        {
            if (playerCard.gameObject.activeInHierarchy)
            {
                numEliminations += 1;
            }
        }
        
        int ticks = 0;
        while (ticks < numEliminations)
        {
            yield return new WaitForSeconds(1.5f / numEliminations);
            numRemainingPlayers -= 1;
            remainingPlayerCount.text = numRemainingPlayers.ToString();
            ticks += 1;
        }
    }

    IEnumerator Memoriam()
    {
        int numEliminations = 0;
        foreach (PlayerCard playerCard in playerCards)
        {
            if (playerCard.gameObject.activeInHierarchy)
            {
                numEliminations += 1;
            }
        }

        playerHolder.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -130.2f, 0);
        float topPadding = playerHolder.GetComponent<VerticalLayoutGroup>().padding.top;
        float bottomPadding = playerHolder.GetComponent<VerticalLayoutGroup>().padding.bottom;
        float spacing = playerHolder.GetComponent<VerticalLayoutGroup>().spacing;
        float playerHolderHeight = topPadding + bottomPadding + (spacing * (numEliminations - 1)) + (numEliminations * playerCardSize) + 10;
        float newY = -130.2f + (playerHolderHeight * 2);
        float currentY = -130.2f;
        while (playerHolder.GetComponent<RectTransform>().anchoredPosition.y < newY)
        {
            yield return new WaitForSeconds(0.02f);
            currentY += (playerHolderHeight * 2)/150;
            playerHolder.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, currentY, 0);
        }
    }

    /// <summary>
    /// Displays the instructions screen to the player. Doubles as a "waiting room."
    /// </summary>
    /// <param name="playerStatus">Whether the player is Host or Participant.</param>
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

    /// <summary>
    /// Displays the animated category intro screen after Category has been chosen.
    /// </summary>
    /// <param name="category">The specified Category that was chosen.</param>
    public void DisplayPreStartScreen(string category)
    {
        preStartScreen.SetActive(true);
        participantWaitingScreen.SetActive(false);
        instructionsScreen.SetActive(false);

        if (category == "0")
        {
            categorySelectAnimation.SetTrigger("Twitch");
            AudioManager.current.PlaySound(twitchIntro);
        }
        else if (category == "1")
        {
            categorySelectAnimation.SetTrigger("Film");
            AudioManager.current.PlaySound(filmIntro);
        }
        else if (category == "2")
        {
            categorySelectAnimation.SetTrigger("Games");
            AudioManager.current.PlaySound(gamesIntro);
        }
        else if (category == "3")
        {
            categorySelectAnimation.SetTrigger("All");
            AudioManager.current.PlaySound(allIntro);
        }
    }

    public void ToggleGameRecapScreen()
    {
        if (gameRecapScreen.activeInHierarchy)
        {
            gameRecapScreen.SetActive(false);
            gameEndScreen.SetActive(true);
        }
        else
        {
            gameRecapScreen.SetActive(true);
            gameEndScreen.SetActive(false);
        }
    }

    /// <summary>
    /// Displays the end screen, after all rounds have been played or (theoretically) final player has been eliminated.
    /// </summary>
    /// <param name="playerStatus">Whether player is Host or Participant.</param>
    public void DisplayGameEndScreen(PlayerStatus playerStatus)
    {
        UpdateQuestionRecap(GameManagerAYSTTC.current.usedQuestions);
        gameScreen.SetActive(false);
        outcomeScreen.SetActive(false);
        timerSlider.gameObject.SetActive(false);
        gameEndScreen.SetActive(true);
        UpdatePlayerCount();
        if (GameManagerAYSTTC.current.remainingPlayerCount == 1)
        {
            StartCoroutine(_Timer(2f, () =>
            {
                playersRemainingDisplay.text = "You were the ONLY player to get every question right!";
                playersRemainingDisplay.gameObject.SetActive(true);
                StartCoroutine(_FadeObjectIn(playersRemainingDisplay.gameObject, 1f));
            }));
        }
        else if (GameManagerAYSTTC.current.remainingPlayerCount == 2)
        {
            StartCoroutine(_Timer(2f, () =>
            {
                playersRemainingDisplay.text = "You and 1 other made it to the end!";
                playersRemainingDisplay.gameObject.SetActive(true);
                StartCoroutine(_FadeObjectIn(playersRemainingDisplay.gameObject, 1f));
            }));
        }
        else if (GameManagerAYSTTC.current.remainingPlayerCount > 2)
        {
            StartCoroutine(_Timer(2f, () =>
            {
                playersRemainingDisplay.text = "You and " + (GameManagerAYSTTC.current.remainingPlayerCount - 1) + " others made it to the end!";
                playersRemainingDisplay.gameObject.SetActive(true);
                StartCoroutine(_FadeObjectIn(playersRemainingDisplay.gameObject, 1f));
            }));
        }
        else if (GameManagerAYSTTC.current.remainingPlayerCount == 0)
        {
            StartCoroutine(_Timer(2f, () =>
            {
                congratsText.text = "Well...";
                playersRemainingDisplay.text = "...it seems that NOBODY made it to the end!";
                playersRemainingDisplay.gameObject.SetActive(true);
                StartCoroutine(_FadeObjectIn(playersRemainingDisplay.gameObject, 1f));
            }));
        }
    }

    /// <summary>
    /// Returns the player to the main menu.
    /// </summary>
    public void BackToMainMenu()
    {
        if (GameManager.current.playerStatus == PlayerStatus.Host)
        {
            // Delete lobby.
            GameManager.current.StartCoroutine(GameManager.current._DeleteLobby());
            Debug.Log("Deleting Lobby.");
        }
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Updates the player count from AYSTTC's GameManager.
    /// </summary>
    public void UpdatePlayerCount()
    {
        playerCount = GameManagerAYSTTC.current.remainingPlayerCount;
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
            //ShowTimer(timeRemaining, startTime);                                               // Keeps the timer slider display updated.
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

    /// <summary>
    /// Updates the lobby settings based on Host selections.
    /// </summary>
    /// <param name="settingType">The specified Setting Type to change.</param>
    public void UpdateSettings(int settingType)
    {
        if ((SettingType)settingType == SettingType.MaxRounds)
        {
            GameManagerAYSTTC.current.roundCount = (int)maxRoundsSlider.value;
            maxRoundsText.text = maxRoundsSlider.value.ToString();
            if (maxRoundsSlider.value == 16)
            {
                maxRoundsText.text = "Endless";
                GameManagerAYSTTC.current.unlimitedRounds = true;
            }
            else
            {
                maxRoundsText.text = maxRoundsSlider.value.ToString();
            }
        }
        else if ((SettingType)settingType == SettingType.RoundTime)
        {
            GameManagerAYSTTC.current.timerDuration = (int)roundTimeSlider.value;
            roundTimeText.text = roundTimeSlider.value.ToString();
        }
        else if ((SettingType)settingType == SettingType.Difficulty)
        {
            GameManagerAYSTTC.current.difficulty = (int)difficultySlider.value;
            if (difficultySlider.value == 0)
            {
                difficultySliderText.text = "Easy";
            }
            else if (difficultySlider.value == 1)
            {
                difficultySliderText.text = "Normal";
            }
            else if (difficultySlider.value == 2)
            {
                difficultySliderText.text = "Hard";
            }
            else if (difficultySlider.value == 3)
            {
                difficultySliderText.text = "Very Hard";
            }
        }
        else if ((SettingType)settingType == SettingType.HostAnswers)
        {
            
        }
    }

    /// <summary>
    /// Starts a fade-out/fade-in cycle for the specified object.
    /// </summary>
    /// <param name="obj">The object to begin fading out/in.</param>
    /// <param name="co">The reference to save this coroutine cycle to. Used to track and remotely stop the coroutine.</param>
    /// <param name="fadeOut">Whether the object should begin with fadeOut or fadeIn.</param>
    public void _FadeObjectCycle(GameObject obj, Coroutine co, bool fadeOut = true)
    {
        if (fadeOut)
            StartCoroutine(_FadeObjectOut(obj, 1, true, co));
        else
            StartCoroutine(_FadeObjectIn(obj, 1, true, co));
    }

    public void UpdateQuestionRecap(List<Question> questions)
    {
        for(int i = 0; i < questions.Count; i++)
        {
            questionCards[i].gameObject.SetActive(true);
            questionCards[i].UpdateQuestionCard(i, questions[i]);
        }
    }

    /// <summary>
    /// Fades the object alpha out to 0 over time.
    /// </summary>
    /// <param name="obj">The object to fade out.</param>
    /// <param name="dur">The fade out time.</param>
    /// <param name="doFadeInAfter">Whether the object should fade back in afterwards.</param>
    /// <param name="co">An existing FadeOut/FadeIn coroutine. Used to track and remotely stop the coroutine when needed.</param>
    /// <returns></returns>
    IEnumerator _FadeObjectOut(GameObject obj, float dur, bool doFadeInAfter = false, Coroutine co = null)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        float startVal = canvasGroup.alpha;
        float time = 0;

        while (time < dur)
        {
            canvasGroup.alpha = Mathf.Lerp(startVal, 0, time / dur);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;
        if (doFadeInAfter)
        {
            if (co != null)
                StartCoroutine(_FadeObjectIn(obj, dur, true, co));
            else
                StartCoroutine(_FadeObjectIn(obj, dur, true));
        }
    }

    /// /// <summary>
    /// Fades the object alpha in to 1 over time.
    /// </summary>
    /// <param name="obj">The object to fade in.</param>
    /// <param name="dur">The fade in time.</param>
    /// <param name="doFadeInAfter">Whether the object should fade back out afterwards.</param>
    /// <param name="co">An existing FadeOut/FadeIn coroutine. Used to track and remotely stop the coroutine when needed.</param>
    /// <returns></returns>
    IEnumerator _FadeObjectIn(GameObject obj, float dur, bool doFadeOutAfter = false, Coroutine co = null)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        float startVal = canvasGroup.alpha;
        float time = 0;

        while (time < dur)
        {
            canvasGroup.alpha = Mathf.Lerp(startVal, 1, time / dur);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
        if (doFadeOutAfter)
        {
            if (co != null)
                StartCoroutine(_FadeObjectOut(obj, dur, true, co));
            else
                StartCoroutine(_FadeObjectOut(obj, dur, true));
        }
    }
}

/// <summary>
/// A type of lobby Setting to be changed. Created for AYSTTC.
/// </summary>
[System.Serializable]
public enum SettingType { MaxRounds, RoundTime, Difficulty, HostAnswers }
