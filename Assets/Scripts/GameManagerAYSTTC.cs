using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
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
    [Tooltip("The current difficulty of the game.")]
    public int difficulty = 1;
    [Tooltip ("The number of rounds, or questions, to play.")]
    public int roundCount = 5;
    [Tooltip ("Round timer duration in seconds.")]
    public float timerDuration = 5f;
    [Tooltip("The question categories.")]
    public List<QuestionCategory> categories;
    public int tierInc = 1;
    public int maxTier = 5;
    [Header ("UI")]
    [Tooltip("The start button.")]
    public Button startButton;
    [TextArea(1, 5)]
    [Tooltip("The game database URL.")]
    public string gameDatabaseLink;

    [HideInNormalInspector] public float timeRemaining = 5f;
    [HideInNormalInspector] public int currentRound = 0;
    [HideInNormalInspector] public QuestionCategory chosenCategory;
    [HideInNormalInspector] public int catIndex = 0;
    [HideInNormalInspector] public int quesIndex = 0;
    [HideInNormalInspector] public int currentTier = 0;
    [HideInNormalInspector] public Question currentQuestion;
    [HideInNormalInspector] public string questionID;
    [HideInNormalInspector] public Answer selectedAnswer = null;
    [HideInNormalInspector] public int remainingPlayerCount = 0;
    [HideInNormalInspector] public bool doAllCategories = false;
    [HideInNormalInspector] public bool updatedPlayerCount = false;

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
            UIManagerAYSTTC.current.DisplayInstructionsScreen(GameManager.current.playerStatus);
            StartCoroutine(_CheckForGameStart());
        }
    }

    /// <summary>
    /// Selects the specified Category as the chosen question Category.
    /// </summary>
    /// <param name="category"></param>
    public void ChooseCategory(QuestionCategory category)
    {
        chosenCategory = category;
        catIndex = categories.FindIndex(x => x.Equals(chosenCategory));
        ShowStartButton();
    }

    /// <summary>
    /// Selects a random category.
    /// </summary>
    /// <param name="changeUI">Set to true if used for Random category button.</param>
    public void ChooseRandomCategory(bool changeUI = false)
    {
        // Choose a category at random.
        catIndex = UnityEngine.Random.Range(0, categories.Count);
        chosenCategory = categories[catIndex];

        if (changeUI)
            ShowStartButton();
    }

    /// <summary>
    /// Enables all categories for play. Not currently used since we already have an ALL QuestionCategory.
    /// </summary>
    public void ChooseAllCategories()
    {
        doAllCategories = true;
        ShowStartButton();
    }

    /// <summary>
    /// Shows or de-greyifies the Start Game button.
    /// </summary>
    public void ShowStartButton()
    {
        startButton.interactable = true;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        AudioManager.current.PlayMusic("inGameMusic");
        currentRound = 0;
        currentTier = 0;

        //set max rounds
        roundCount = (int)UIManagerAYSTTC.current.maxRoundsSlider.value;

        //set max tier
        if ((int)UIManagerAYSTTC.current.difficultySlider.value == 0)
        {
            maxTier = 2;
        }
        else if ((int)UIManagerAYSTTC.current.difficultySlider.value == 1)
        {
            maxTier = 3;
        }
        else if ((int)UIManagerAYSTTC.current.difficultySlider.value == 2)
        {
            maxTier = 4;
        }
        else if ((int)UIManagerAYSTTC.current.difficultySlider.value == 3)
        {
            maxTier = 5;
        }

        //set round time
        timerDuration = (int)UIManagerAYSTTC.current.roundTimeSlider.value;

        StartCoroutine(_StartGame(GameManager.current.currentLobby));
        StartCoroutine(_GetPlayerCount());
        //StartRound();
    }

    /// <summary>
    /// Starts a new round by choosing a new question and sending it to the server. The host should only be able to access this function.
    /// </summary>
    public void StartRound()
    {
        timeRemaining = timerDuration;
        UIManagerAYSTTC.current.timerSlider.maxValue = timerDuration;
        currentRound += 1;
        if (currentTier + tierInc <= maxTier)
        {
            currentTier += tierInc;
        }
        currentQuestion = ChooseQuestion();
        questionID = GetQuestionID(catIndex, quesIndex);
        StartCoroutine(_SendQuestion(GameManager.current.currentLobby, questionID));
    }

    /// <summary>
    /// Selects a random question of the current tier in the chosen category.
    /// </summary>
    /// <returns></returns>
    public Question ChooseQuestion()
    {
        if (doAllCategories)
        {
            ChooseRandomCategory();
        }
        List<Question> tierQuestions = new List<Question>();
        foreach (Question q in chosenCategory.questions)
        {
            tierQuestions = chosenCategory.questions.Where(qu => qu.difficulty == currentTier).ToList();
        }
        int qIndex = UnityEngine.Random.Range(0, tierQuestions.Count);
        quesIndex = chosenCategory.questions.FindIndex(x => x.Equals(tierQuestions[qIndex]));
        catIndex = categories.FindIndex(x => x.Equals(chosenCategory));
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
        StartCoroutine(_Answer(GameManager.current.currentLobby, GameManager.current.playerName, outcome));
    }
    
    /// <summary>
    /// A timer. Accepts a maximum (starting) value, and a TimerPurpose (when the timer is going to be run).
    /// </summary>
    /// <param name="maxVal">The max/starting value.</param>
    /// <param name="purpose">When the timer is going to be run.</param>
    /// <returns></returns>
    IEnumerator _Timer(float maxVal, TimerPurpose purpose = TimerPurpose.DuringRound)
    {
        timeRemaining = maxVal;
        Debug.Log("Timer begun with purpose: " + purpose);
        while (true)
        {
            UIManagerAYSTTC.current.ShowTimer(timeRemaining, maxVal);                                               // Keeps the timer slider display updated.
            if (timeRemaining > 0)
            {
                if (purpose == TimerPurpose.DuringRound)
                {
                    UIManagerAYSTTC.current.bgBrightness.color = new Color(0, 0, 0, 0.8f - ((timeRemaining / timerDuration * 0.6f) + 0.1f));
                }
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                if (purpose == TimerPurpose.DuringRound)
                {
                    if (selectedAnswer == null)
                    {
                        StartCoroutine(_UpdatePlayerStatus("eliminated"));
                    }
                    else if (selectedAnswer.isCorrectAnswer)
                    {
                        StartCoroutine(_UpdatePlayerStatus("awaiting"));
                    }
                    else
                    {
                        StartCoroutine(_UpdatePlayerStatus("eliminated"));
                    }

                    if (GameManager.current.playerStatus == PlayerStatus.Host)
                    {
                        //StartCoroutine(_CompleteRound(GameManager.current.currentLobby));
                        StartCoroutine(_ReadyCheck());
                        yield break;
                    }
                    else if (GameManager.current.playerStatus == PlayerStatus.Participant)
                    {
                        StartCoroutine(_GetRoundStatus(GameManager.current.currentLobby));
                        yield break;
                    }
                }
                else if (purpose == TimerPurpose.EndOfRoundSafe)
                {
                    selectedAnswer = null;
                    if (GameManager.current.playerStatus == PlayerStatus.Host)
                    {
                        // Final round was reached. Go to end screen.
                        if (currentRound == roundCount)
                        {
                            StartCoroutine(_GetPlayerCount(func: () => StartCoroutine(_EndGame(GameManager.current.currentLobby))));
                            //StartCoroutine(_EndGame(GameManager.current.currentLobby));
                            yield break;
                        }
                        else
                        {
                            StartRound();
                        }
                    }
                    else if (GameManager.current.playerStatus == PlayerStatus.Participant)
                    {
                        // Final round was reached. Go to end screen.
                        if (currentRound == roundCount)
                        {
                            StartCoroutine(_GetPlayerCount(func: () => UIManagerAYSTTC.current.DisplayGameEndScreen(PlayerStatus.Participant)));
                            //UIManagerAYSTTC.current.DisplayGameEndScreen(PlayerStatus.Participant);
                            yield break;
                        }
                        else
                        {
                            StartCoroutine(_CheckForRoundStart());
                        }
                    }
                    Debug.Log("End of round has ended.");
                    yield break;
                }
                else if (purpose == TimerPurpose.EndOfRoundEliminated)
                {
                    selectedAnswer = null;
                    if (GameManager.current.playerStatus == PlayerStatus.Host)
                    {
                        // Final round was reached. Go to end screen.
                        if (currentRound == roundCount)
                        {
                            StartCoroutine(_GetPlayerCount(co: _EndGame(GameManager.current.currentLobby)));
                            //StartCoroutine(_EndGame(GameManager.current.currentLobby));
                            yield break;
                        }
                        else
                        {
                            StartRound();
                        }
                    }
                    else if (GameManager.current.playerStatus == PlayerStatus.Participant)
                    {
                        yield break;
                    }
                    yield break;
                }
                else if (purpose == TimerPurpose.PreStart)
                {
                    if (GameManager.current.playerStatus == PlayerStatus.Host)
                    {
                        StartRound();
                    }
                    else if (GameManager.current.playerStatus == PlayerStatus.Participant)
                    {
                        StartCoroutine(_CheckForRoundStart());
                    }
                    Debug.Log("First round has started.");
                    yield break;
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// Sends a Question, using a questionID, to the server.
    /// </summary>
    /// <param name="lobbyNumber">The current lobby number.</param>
    /// <param name="questionID">The question ID. Use GetQuestionID() to create one.</param>
    /// <returns></returns>
    public IEnumerator _SendQuestion(string lobbyNumber, string questionID)
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
                    //timeRemaining = timerDuration;
                    StartCoroutine(_Timer(timerDuration));
                    StartCoroutine(_GetQuestion(GameManager.current.currentLobby));
                    UIManagerAYSTTC.current.SetGameStageP(currentQuestion);
                }
            }
        }
    }

    /// <summary>
    /// Checks for and grabs the latest questionID from the server, then converts it to a Category and Question.
    /// </summary>
    /// <param name="lobbyNumber">The current lobby number.</param>
    /// <returns></returns>
    public IEnumerator _GetQuestion(string lobbyNumber)
    {
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
                // questionID received. Split the ID into appropriate Category index and Question index data.
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

                if (GameManager.current.playerStatus == PlayerStatus.Participant)
                {
                    //timeRemaining = timerDuration;
                    StartCoroutine(_Timer(timerDuration));
                    UIManagerAYSTTC.current.SetGameStageP(currentQuestion);
                }

                yield break;
            }
        }
    }

    /// <summary>
    /// Sends an outcome (whether the player got the question correct or not) to the server.
    /// </summary>
    /// <param name="lobbyNumber">The current lobby number.</param>
    /// <param name="playerName">The player's name.</param>
    /// <param name="outcome">'incorrect,' 'correct,' or 'timeOut.'</param>
    /// <returns></returns>
    public IEnumerator _Answer(string lobbyNumber, string playerName, string outcome)
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

    /// <summary>
    /// Tells the server that the round has ended. Should only be accessible by host.
    /// </summary>
    /// <param name="lobbyNumber">The current lobby number.</param>
    /// <returns></returns>
    public IEnumerator _CompleteRound(string lobbyNumber)
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
                    StartCoroutine(_GetRoundStatus(GameManager.current.currentLobby));
                }
            }
        }
    }

    /// <summary>
    /// Checks for and grabs round completion status from the server.
    /// </summary>
    /// <param name="lobbyNumber">The current lobby number.</param>
    /// <returns></returns>
    public IEnumerator _GetRoundStatus(string lobbyNumber)
    {
        while (true)
        {
            WWWForm form = new WWWForm();
            form.AddField("function", "getStatus");
            form.AddField("lobbyNumber", lobbyNumber);

            using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "lobby.php", form))
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
                    if (receivedData == "completing")
                    {
                        UIManagerAYSTTC.current.bgBrightness.color = new Color(0, 0, 0, 0.1f);
                        timeRemaining = 5f;
                        Debug.Log("Time Set: " + timeRemaining);
                        if (selectedAnswer == null)
                        {
                            UIManagerAYSTTC.current.DisplayOutcomeScreen(OutcomeType.TimeOut);
                            StartCoroutine(_Timer(5f, TimerPurpose.EndOfRoundEliminated));
                        }
                        else if (selectedAnswer.isCorrectAnswer)
                        {
                            UIManagerAYSTTC.current.DisplayOutcomeScreen(OutcomeType.Correct);
                            StartCoroutine(_Timer(5f, TimerPurpose.EndOfRoundSafe));
                        }
                        else
                        {
                            UIManagerAYSTTC.current.DisplayOutcomeScreen(OutcomeType.Wrong);
                            StartCoroutine(_Timer(5f, TimerPurpose.EndOfRoundEliminated));
                        }
                        yield break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if next round has started yet. Should only be accessible by the participants.
    /// </summary>
    /// <returns></returns>
    public IEnumerator _CheckForRoundStart()
    {
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(gameDatabaseLink + "lobbies/" + GameManager.current.currentLobby + "/lobbyStatus.txt"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string receivedData = www.downloadHandler.text;
                    if (receivedData == "questioning")
                    {
                        if (GameManager.current.playerStatus == PlayerStatus.Participant)
                        {
                            currentRound += 1;
                            if (currentTier + tierInc <= maxTier)
                            {
                                currentTier += tierInc;
                            }
                            StartCoroutine(_GetQuestion(GameManager.current.currentLobby));
                        }
                        //StartGame();
                        //Debug.Log("Host started game.");
                        yield break;
                    }
                    else if (receivedData == "gameEnd")
                    {
                        UIManagerAYSTTC.current.DisplayGameEndScreen(PlayerStatus.Participant);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if game has started yet. Should only be accessible by the participants.
    /// </summary>
    /// <returns></returns>
    public IEnumerator _CheckForGameStart()
    {
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(gameDatabaseLink + "lobbies/" + GameManager.current.currentLobby + "/lobbyStatus.txt"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string receivedData = www.downloadHandler.text;
                    if (receivedData.Contains("prestart"))
                    {
                        if (GameManager.current.playerStatus == PlayerStatus.Participant)
                        {
                            AudioManager.current.PlayMusic("inGameMusic");
                            StartCoroutine(_CheckForRoundStart());
                            string category = receivedData.Split('/')[1];
                            timerDuration = int.Parse(receivedData.Split('/')[2]);
                            roundCount = int.Parse(receivedData.Split('/')[3]);
                            UIManagerAYSTTC.current.DisplayPreStartScreen(category);
                            timeRemaining = timerDuration;
                            Debug.Log("host starts game, new prestart timer");
                        }
                        yield break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tells the server that the game has started and participants should run pre-start functions. Should only be accessible by host.
    /// </summary>
    /// <param name="lobbyNumber">The current lobby number.</param>
    /// <returns></returns>
    public IEnumerator _StartGame(string lobbyNumber)
    {
        WWWForm form = new WWWForm();
        form.AddField("function", "changeStatus");
        form.AddField("lobbyNumber", lobbyNumber);
        form.AddField("newStatus", "prestart" + "/" + catIndex + "/" + timerDuration + "/" + roundCount);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "lobby.php", form))
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
                if (receivedData == "successfully changed status")
                {
                    UIManagerAYSTTC.current.DisplayInstructionsScreen(GameManager.current.playerStatus);
                    StartCoroutine(_Timer(5, TimerPurpose.PreStart));
                }
            }
        }
    }

    /// <summary>
    /// Tells the server that the game has ended. Should only be called by the host.
    /// </summary>
    /// <param name="lobbyNumber"></param>
    /// <returns></returns>
    public IEnumerator _EndGame(string lobbyNumber)
    {
        WWWForm form = new WWWForm();
        form.AddField("function", "changeStatus");
        form.AddField("lobbyNumber", lobbyNumber);
        form.AddField("newStatus", "gameEnd");

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "lobby.php", form))
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
                if (receivedData == "successfully changed status")
                {
                    UIManagerAYSTTC.current.DisplayGameEndScreen(GameManager.current.playerStatus);
                }
            }
        }
    }

    /// <summary>
    /// Sets remainingPlayerCount to the count of players grabbed from the database.
    /// </summary>
    /// <param name="func">A method to run after player count is updated.</param>
    /// <returns></returns>
    public IEnumerator _GetPlayerCount(Action func = null, IEnumerator co = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("function", "getPlayerList");
        form.AddField("lobbyNumber", GameManager.current.currentLobby);
        form.AddField("playerName", GameManager.current.playerName);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "lobby.php", form))
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
                string[] splitData = receivedData.Split('\n');
                List<string> playerList = new List<string>();
                foreach (string str in splitData)
                {
                    if (str != "") { playerList.Add(str); }
                }
                Debug.Log("Player List: " + receivedData + ", " + splitData.Length);
                Debug.Log("New Player List: " + playerList + ", " + playerList.Count);
                remainingPlayerCount = playerList.Count;
                updatedPlayerCount = true;

                if (func != null)
                {
                    func.Invoke();
                }
            }
        }
        
        if (co != null)
        {
            StartCoroutine(co);
        }
    }

    /// <summary>
    /// Deletes the current lobby from the website database.
    /// </summary>
    /// <returns></returns>
    public IEnumerator _DeleteLobby()
    {
        Debug.Log("Running deletion coroutine.");

        WWWForm form = new WWWForm();
        form.AddField("function", "deleteLobby");
        form.AddField("lobbyNumber", GameManager.current.currentLobby);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "lobby.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                AlertText.current.ToggleAlertText(www.error, Color.red);
                Debug.Log("Could not delete lobby.");
            }
            else
            {
                Debug.Log("In the process of deleting lobby!");
                string receivedData = www.downloadHandler.text;
                Debug.Log("Current Lobby List: " + receivedData);
                Debug.Log("Deleted lobby!");
            }
        }
    }

    public IEnumerator _ReadyCheck()
    {
        while (true)
        {
            WWWForm form = new WWWForm();
            form.AddField("function", "retrieve");
            form.AddField("lobbyNumber", GameManager.current.currentLobby);

            using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "updatePlayerStatus.php", form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string receivedData = www.downloadHandler.text;
                    Debug.Log(receivedData);
                    if (receivedData == "everyone is ready")
                    {
                        if (GameManager.current.playerStatus == PlayerStatus.Host)
                        {
                            StartCoroutine(_CompleteRound(GameManager.current.currentLobby));
                        }
                        yield break;
                    }
                }
            }
        }
    }

    public IEnumerator _UpdatePlayerStatus(string newStatus)
    {
        Debug.Log(GameManager.current.playerName + "'s new status: " + newStatus);

        WWWForm form = new WWWForm();
        form.AddField("function", "update");
        form.AddField("lobbyNumber", GameManager.current.currentLobby);
        form.AddField("playerName", GameManager.current.playerName);
        form.AddField("newStatus", newStatus);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "updatePlayerStatus.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string receivedData = www.downloadHandler.text;
                Debug.Log(receivedData);
                if (receivedData == "successfully updated player status")
                {
                    if (newStatus == "eliminated")
                    {
                        GameManager.current.LoadScene("AYSTTC Main Menu");
                        // Eventually we want to send them to an eliminated screen.
                        yield break;
                    }
                    else if (newStatus == "awaiting")
                    {

                    }
                }
            }
        }
    }
}

/// <summary>
/// When the Timer coroutine is going to be used.
/// </summary>
public enum TimerPurpose { DuringRound, EndOfRoundSafe, EndOfRoundEliminated, PreStart, EndOfGame }
