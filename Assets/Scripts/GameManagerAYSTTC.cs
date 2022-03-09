using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

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
    [TextArea(1, 5)]
    [Tooltip("The game database URL.")]
    public string gameDatabaseLink;

    [HideInNormalInspector] public float timeRemaining = 15f;
    [HideInNormalInspector] public bool timerBegun = false;
    [HideInNormalInspector] public bool isTimerRunning = false;
    [HideInNormalInspector] public bool roundInProgress = false;
    [HideInNormalInspector] public bool isRoundOver = false;
    [HideInNormalInspector] public bool runningEOR = false;
    [HideInNormalInspector] public int currentRound = 0;
    [HideInNormalInspector] public QuestionCategory chosenCategory;
    [HideInNormalInspector] public int catIndex = 0;
    [HideInNormalInspector] public int quesIndex = 0;
    [HideInNormalInspector] public int currentTier = 0;
    [HideInNormalInspector] public bool questionChosen = false;
    [HideInNormalInspector] public bool questionSent = false;
    [HideInNormalInspector] public bool questionReceived = false;
    [HideInNormalInspector] public bool roundComplete = false;
    [HideInNormalInspector] public Question currentQuestion;
    [HideInNormalInspector] public string questionID;
    [HideInNormalInspector] public Answer selectedAnswer = null;

    public static GameManagerAYSTTC current;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Host must choose a category.
        if (GameManager.current.playerStatus == PlayerStatus.Host)
        {
            UIManagerAYSTTC.current.SetSelectionStageH();
        }
        else if (GameManager.current.playerStatus == PlayerStatus.Participant)
        {
            UIManagerAYSTTC.current.SetSelectionStageP();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Grab random question from questions list.
        //TODO: Initialize round.
        //TODO: Do pre-timer stuff (like a "ready-set-go").
        //TODO: Initialize timer.
        //TODO: Display question and answer choices.
        //TODO: Send category and question through UnityWebRequest as an ID
        //      ID = [category index] + [question index]

        //TODO: When timer ends, check if player's chosen answer matches correct answer.
        //TODO: Reward correct players.
        //TODO: Eliminate incorrect players.
        //TODO: Repeat loop until end.

        if (roundInProgress)
        {
            if (!timerBegun && !isRoundOver)
            {
                // Select and show question.
                if (GameManager.current.playerStatus == PlayerStatus.Host)
                {
                    if (!questionChosen)
                    {
                        currentQuestion = ChooseQuestion();
                        questionID = GetQuestionID(catIndex, quesIndex);
                        StartCoroutine(SendQuestion(GameManager.current.currentLobby, questionID));
                        questionChosen = true;
                    }
                    if (questionSent)
                    {
                        isTimerRunning = true;
                        StartCoroutine(Timer(timerDuration));
                        timerBegun = true;
                        StartCoroutine(GetQuestion(GameManager.current.currentLobby));

                        UIManagerAYSTTC.current.SetGameStageP(currentQuestion);
                    }
                }
                if (GameManager.current.playerStatus == PlayerStatus.Participant)
                {
                    StartCoroutine(GetQuestion(GameManager.current.currentLobby));
                    if (questionReceived) { timerBegun = true; }
                    UIManagerAYSTTC.current.SetGameStageP(currentQuestion);
                }
            }
            else if (isRoundOver && !runningEOR)
            {
                // Run end-of-round activities.
                questionSent = false;
                runningEOR = true;
                if (GameManager.current.playerStatus == PlayerStatus.Host)
                {
                    StartCoroutine(CompleteRound(GameManager.current.currentLobby));

                    timeRemaining = 5f;
                    isTimerRunning = true;
                    Debug.Log("Time Set: " + timeRemaining);
                    StartCoroutine(Timer(5f, TimerPurpose.EndOfRound));
                    if (selectedAnswer == null)
                    {
                        UIManagerAYSTTC.current.DisplayOutcomeScreen(OutcomeType.TimeOut);
                    }
                    else if (selectedAnswer.isCorrectAnswer)
                    {
                        UIManagerAYSTTC.current.DisplayOutcomeScreen(OutcomeType.Correct);
                    } else
                    {
                        UIManagerAYSTTC.current.DisplayOutcomeScreen(OutcomeType.Wrong);
                    }
                }
            }
        }
        

        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(GetQuestion(GameManager.current.currentLobby));
            if (questionReceived) { timerBegun = true; }
            UIManagerAYSTTC.current.SetGameStageP(currentQuestion);
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
        catIndex = Random.Range(0, categories.Count);
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
        UIManagerAYSTTC.current.timerSlider.maxValue = timerDuration;
        currentRound = 1;
        currentTier = 1;

        roundInProgress = true;
    }

    /// <summary>
    /// Selects a random question of the current tier in the chosen category.
    /// </summary>
    /// <returns></returns>
    public Question ChooseQuestion()
    {
        List<Question> tierQuestions = new List<Question>();
        foreach (Question q in chosenCategory.questions)
        {
            tierQuestions = chosenCategory.questions.Where(qu => qu.difficulty == currentTier).ToList();
        }
        int qIndex = Random.Range(0, tierQuestions.Count);
        quesIndex = chosenCategory.questions.FindIndex(x => x.Equals(tierQuestions[qIndex]));
        return chosenCategory.questions[quesIndex];
    }

    /// <summary>
    /// Creates and returns the ID for the specified Category and Question.
    /// </summary>
    /// <param name="cIndex"></param>
    /// <param name="qIndex"></param>
    /// <returns></returns>
    public string GetQuestionID(int cIndex, int qIndex)
    {
        return ("." + cIndex.ToString() + "Q" + qIndex.ToString());
    }

    /// <summary>
    /// Select an Answer and record into database.
    /// </summary>
    /// <param name="answer"></param>
    public void ChooseAnswer(Answer answer)
    {
        selectedAnswer = answer;
        string outcome = "";
        if (selectedAnswer.isCorrectAnswer)
        {
            outcome = "correct";
        } else
        {
            outcome = "incorrect";
        }
        StartCoroutine(Answer(GameManager.current.currentLobby, GameManager.current.playerName, outcome));
    }
    
    IEnumerator Timer(float maxVal, TimerPurpose purpose = TimerPurpose.DuringRound)
    {
        Debug.Log("Timer begun with purpose: " + purpose);
        Debug.Log("Timer Set To: " + timeRemaining);
        while (isTimerRunning)
        {
            UIManagerAYSTTC.current.ShowTimer(timeRemaining, maxVal);
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                if (purpose == TimerPurpose.DuringRound)
                {
                    isRoundOver = true;
                }
                else if (purpose == TimerPurpose.EndOfRound)
                {
                    isRoundOver = false;
                    runningEOR = false;
                }
            }

            yield return null;
        }
    }

    public IEnumerator SendQuestion(string lobbyNumber, string questionID)
    {
        Debug.Log("Sending question.");
        WWWForm form = new WWWForm();
        form.AddField("function", "startQuestion");
        form.AddField("lobbyNumber", lobbyNumber);
        form.AddField("questionID", questionID);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "question.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                AlertText.current.ToggleAlertText(www.error, Color.red);
            }
            else
            {
                string receivedData = www.downloadHandler.text;
                Debug.Log(receivedData);
                if (receivedData == "successfully started question")
                {
                    questionSent = true;
                }
            }
        }
    }

    public IEnumerator GetQuestion(string lobbyNumber)
    {
        Debug.Log("Getting question.");
        WWWForm form = new WWWForm();
        form.AddField("function", "getQuestion");
        form.AddField("lobbyNumber", lobbyNumber);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "question.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                AlertText.current.ToggleAlertText(www.error, Color.red);
            }
            else
            {
                questionReceived = true;
                string receivedData = www.downloadHandler.text;
                Debug.Log(receivedData);
                string[] splitData = receivedData.Split('.');
                string dataID = splitData[splitData.Length - 1];
                int dataIDCategory = int.Parse(dataID.Split('Q')[0]);
                int dataIDQuestion = int.Parse(dataID.Split('Q')[1]);
                Debug.Log("Grabbed category: " + dataIDCategory);
                Debug.Log("Grabbed question: " + dataIDQuestion);
                chosenCategory = categories[dataIDCategory];
                currentQuestion = chosenCategory.questions[dataIDQuestion];
            }
        }
    }

    public IEnumerator Answer(string lobbyNumber, string playerName, string outcome)
    {
        WWWForm form = new WWWForm();
        form.AddField("function", "answerQuestion");
        form.AddField("lobbyNumber", lobbyNumber);
        form.AddField("playerName", playerName);
        form.AddField("outcome", outcome);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "updatePlayerStatus.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                AlertText.current.ToggleAlertText(www.error, Color.red);
            }
            else
            {
                string receivedData = www.downloadHandler.text;
                Debug.Log(receivedData);
                if (receivedData == "player successfully answered")
                {
                    // Player successfully answered.
                }
            }
        }
    }

    public IEnumerator CompleteRound(string lobbyNumber)
    {
        WWWForm form = new WWWForm();
        form.AddField("function", "completeRound");
        form.AddField("lobbyNumber", lobbyNumber);
        form.AddField("roundNumber", currentRound);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "question.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                AlertText.current.ToggleAlertText(www.error, Color.red);
            }
            else
            {
                string receivedData = www.downloadHandler.text;
                Debug.Log(receivedData);
                if (receivedData == "successfully completed round")
                {
                    roundComplete = true;
                }
            }
        }
    }
}

/// <summary>
/// When the Timer coroutine is going to be used.
/// </summary>
public enum TimerPurpose { DuringRound, EndOfRound }
