# Partyception
 The code repository for the Partyception party game.
 The repository for Partyception's serverside code can be found here: https://github.com/Tenodru/Partyception-Serverside

## Overview
This repository hosts all of the code and assets used in the Partyception game.
These scripts are primarily written in C#, and handle all of the game's clientside functionality.
This code was developed by Alex Kong and Victor Do. Below are the main contributions each of us made to the game, and a breakdown of how the code works to provide the clientside functionality needed for the game to function.

## Main Directories
### Editor
- All Unity custom editor scripts are located in this directory. This includes custom editor scripts for the AnswerButton and the QAObject.
### Scenes
- All Unity scenes are located in this folder. The scenes are listed below.
- `AYSTTC Main Menu` : The game's main menu. Players can create or join lobbies here.
- `AYSTTC Lobby Menu` : The game's waiting lobby here. Players are sent here after creating or joining a lobby.
- `AYSTTC Game Menu` : The main game scene. Players answer questions and play through the game loop here.
### ScriptableObjects
- All scriptable objects are located here.
- This is where we store our question objects and categories. 
### Scripts
- All other scripts are located here.



## Contributions and Breakdown - Alex

### Task 1 - The Game Loop
We wanted to create a game that was straightforward and easy to grasp. From the start, we knew our main game loop was gonna be pretty simple - you enter a game, and try to answer a number of questions until you reach the end.
![Main Game Loop Flowchart](https://github.com/Tenodru/Partyception/blob/25e4aa505c1dd881a2f8a55a98c0d6696e805cb6/Other/Readme%20Resources/Alex/Game%20Loop%20Flowchart%201.png)

The "Answer Question" step could then be expanded into a Round Loop, illustrated below.
![Round Loop Flowchart](https://github.com/Tenodru/Partyception/blob/0e76fbd66557594684a1fb64e7fdd183fab46c6e/Other/Readme%20Resources/Alex/Round%20Loop.png)

My first primary task was to build this Round Loop.


#### The Question-Answer Object System
To begin, we needed a framework for our questions and answers. I wrote a `Question` scriptable object class that would take in several parameters:
- `question` : String. The question text.
- `difficulty` : Int. The question difficulty, from 1-5.
- `answerList` : List. A list of the question's Answers.

To hold answer choices, I wrote an `Answer` scriptable object with the following parameters:
- `answer` : String. The answer text.
- `isCorrectAnswer` : Bool. Whether this answer is the correct one for its associated question. `False` by default.

Finally, we needed a Category object to hold a list of questions for our various categories. The `QuestionCategory` takes in these parameters:
- `categoryName` : String. A name for the category.
- `question` : List. A list of all Questions in this category.

This setup would allow our writers to easily add and change questions with ease using Unity's inspector, instead of having to mess around with code.

Editing a Question            |  Editing a Category
:-------------------------:|:-------------------------:
![Inspector 1](https://github.com/Tenodru/Partyception/blob/9603e1fa1ee0bb9d937300aa42ed1c63997ad948/Other/Readme%20Resources/Alex/ScriptableObject%20Games.PNG) | ![Inspector 2](https://github.com/Tenodru/Partyception/blob/9603e1fa1ee0bb9d937300aa42ed1c63997ad948/Other/Readme%20Resources/Alex/ScriptableObject%20Category.PNG)

Once this was done, I could begin building the actual loop in `GameManagerAYSTTC.cs`.


#### Step 1 - Choosing a Question
The first step in the round loop was choosing a question. To do this, we would need to first identify the correct category, then randomly choose a question from that category.

`GameManagerAYSTTC.cs` : 
```csharp
public void ChooseCategory(QuestionCategory category)
{
    chosenCategory = category;
    catIndex = categories.FindIndex(x => x.Equals(chosenCategory));
    ShowStartButton();
}

public void ChooseRandomCategory(bool changeUI = false)
{
    // Choose a category at random.
    catIndex = UnityEngine.Random.Range(0, categories.Count);
    chosenCategory = categories[catIndex];

    if (changeUI)
        ShowStartButton();
}

public void ChooseAllCategories()
{
    doAllCategories = true;
    ShowStartButton();
}
```

These three functions above identify and save the right category based on the user's selection in the lobby settings screen. Below, the `ChooseQuestion()` function chooses a question by first getting the right category, grabbing the question list from that category, then filtering the list to leave a list of only questions that match the round's current difficulty tier. Note that all of this category and question selection only happens in the host client - other players only retrieve and display the question.

`GameManagerAYSTTC.cs` : 
```csharp
public Question ChooseQuestion()
{
    if (doAllCategories)
    {
        ChooseRandomCategory();
    }
    if (unlimitedRounds)
    {
        List<Question> tierQuestions = chosenCategory.questions.Where(qu => qu.difficulty == currentTier).ToList();

        int qIndex = UnityEngine.Random.Range(0, tierQuestions.Count);
        quesIndex = chosenCategory.questions.FindIndex(x => x.Equals(tierQuestions[qIndex]));
        catIndex = categories.FindIndex(x => x.Equals(chosenCategory));
        return chosenCategory.questions[quesIndex];
    }
    else
    {
        // If not unlimited rounds, avoid using used questions.
        List<Question> tierQuestions = chosenCategory.questions.Where(qu => qu.difficulty == currentTier).ToList();

        if (usedQuestions.Count > 0)
        {
            foreach (Question q in usedQuestions)
            {
                if (tierQuestions.Contains(q))
                {
                    tierQuestions.Remove(q);
                }
            }
            // If all questions of this difficulty tier have already been used, we can re-use the old questions.
            if (tierQuestions.Count < 1)
            {
                tierQuestions = chosenCategory.questions.Where(qu => qu.difficulty == currentTier).ToList();
            }
        }

        int qIndex = UnityEngine.Random.Range(0, tierQuestions.Count);
        quesIndex = chosenCategory.questions.FindIndex(x => x.Equals(tierQuestions[qIndex]));
        catIndex = categories.FindIndex(x => x.Equals(chosenCategory));
        usedQuestions.Add(chosenCategory.questions[quesIndex]);

        return chosenCategory.questions[quesIndex];
    }

}
```


#### Step 2 - Sending the Question

Once a question was selected, we needed to send this question to the server so the other players in the lobby would retrieve and display it in their own clients. To do so, we needed to first serialize a question as a "question ID" that would specify what question was chosen what what category.

`GameManagerAYSTTC.cs` : 
```csharp
public string GetQuestionID(int cIndex, int qIndex)
{
    return ("." + cIndex.ToString() + "Q" + qIndex.ToString());
}
```

Then, the `_SendQuestion()` coroutine sends the question ID via a POST request using Unity's Networking library. We specify the PHP script we want to send the request to, and include the necessary parameters. The web server then stores this question.

`GameManagerAYSTTC.cs` : 
```csharp
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
```

If the request is successful, the PHP script will echo back "successfully started question". This lets the host client know that it's time to actially initialize the round; it starts the Timer then moves to the Answering screen.

Of course, for this to function, we needed a timer function that would keep track of time, then return something once the timer hits 0.

`GameManagerAYSTTC.cs` : 
```csharp
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
            // Do end-of-timer things.
        }
```

We used a coroutine for this Timer. It will constantly count down from the specified time to 0, then when it hits 0 (or a negative number), it stops the timer and initiates whatever should be done after the timer ends.
To determine what should be done on timer end, I created a `TimerPurpose` enum. When `_Timer()` is called, the timer will begin with a `TimerPurpose` in mind - we could then specify these timer-end actions based on what the timer is being used for (during a round, during the round recap screen, etc.)

`GameManagerAYSTTC.cs` : 
```csharp
public enum TimerPurpose { DuringRound, EndOfRoundSafe, EndOfRoundEliminated, PreStart, EndOfGame }
```

#### Step 3 - Receiving the Question

Up until this point, the other players' clients will be periodically sending requests to the web server checking for the game start via the `_CheckForGameStart()` coroutine. 

`GameManagerAYSTTC.cs` : 
```csharp
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
                            //REMOVE THIS IF BROKEN
                            StartCoroutine(_UpdatePlayerStatus("prestart"));
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
```

When the server receives its first question, it will set the lobby status in the database to "prestart". When this coroutine receives this "prestart" status from its web request, it will tell the client that the round is ready to begin. The coroutine will then call the `_GetQuestion()` coroutine to retrieve the question ID from the server.

`GameManagerAYSTTC.cs` : 
```csharp
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
                usedQuestions.Add(currentQuestion);
            }

            yield break;
        }
    }
}
```

This coroutine will "decode" the question ID and grab the right category and question. It will then move the player to the Answering screen via the `UIManagerAYSTTC.cs` script (and start the round timer). Host and participants will do this at the same time to ensure that the gameplay of all players remains synced up. 

#### Step 4 - Answering the Question
On the answering screen, players will be given the question and its four answer choices. I created the `AnswerButton.cs` script to give the default Button additional functionality - storing the answer associated with the Button.

`AnswerButton.cs` : 
```csharp
public class AnswerButton : Button
{
    [Tooltip ("The Answer associated with this Button.")]
    public Answer answer;
}
```

`AnswerButtonEditor.cs` : 
```csharp
[CustomEditor (typeof(AnswerButton))]
public class AnswerButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        AnswerButton button = (AnswerButton)target;

        base.OnInspectorGUI();
    }
}
```

By extending the default Button functionality, we could "assign" answers to each of the four buttons in the UI without needing to attach another script to these elements.

![Answer UI](https://github.com/Tenodru/Partyception/blob/43733d8c381b66176ed619e6c69a4c04b055039b/Other/Readme%20Resources/Alex/Answer%20UI.PNG)


The UIManager script will take the answer list from the GameManager script, then randomly assign each answer to each of the buttons. This means that while everyone will see the same answers, the buttons that each answer is assigned to will be different.

`UIManagerAYSTTC.cs` : 
```csharp
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
```

Then, when a player clicks a button, that button will call `SelectAnswerChoice()` function in the UIManager, passing in itself as the AnswerButton parameter. This function will then record the player's selected answer via a call to the GameManager. Clean and simple!

`UIManagerAYSTTC.cs` : 
```csharp
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
```

Furthermore, by clicking any other answer choice, `SelectAnswerChoice()` will "deselect" all other answer buttons by resetting their colors to normal.

The GameManager script will also keep identify whether the player's answer choice is correct or incorrect, allowing the game to eliminate players once the round is over. This elimination system was implemented by Victor, and is covered in his section further below in this readme.

#### Step 5 - Ending the Round
Once the timer runs out, the `_GetRoundStatus()` coroutine will be called, which will check whether or not the player answered incorrectly; the coroutine will then call the `DisplayOutcomeScreen()` function in UIManager, telling the player whether or not they were eliminated. The intricacies of this step - progressing from answering questions to the outcome screen - was mostly handled by Victor.

#### Step 6 - Repeating the Round
Repeating the round is very simple - the above steps are just repeated in the same order. At this point in the development process, Victor began working on the frontend, adding to the existing functionality of the game.



### Task 2 - The End Screen and Closing the Loop Back to Main Menu
With the basic functionality of the game loop completed, my next task was to implement an end to the game that would then allow players to return to the main menu. This would allow us to "complete the loop" of the game, so to speak, so the game could be experienced in repeat from main menu, lobby, game, and main menu again.

![End Loop](https://github.com/Tenodru/Partyception/blob/382bc4835c2c678987352c7077e146a28f3583d1/Other/Readme%20Resources/Alex/Game%20End%20Screen.png)


To begin, I added a `roundCount` variable and a `currentRound` variable to the GameManager; with each round, `currentRound` will increment by 1. Each time the timer is run during a round, when the time runs out, it will check if the `currentRound == roundCount`. If so, it will move to the game end screen with a call to the UIManager. This screen includes a button that will move the client back to the main menu if clicked by the player.



### Task 3 - Expanding The Game Lobby System
With the basic functionality of the game loop completed, my next task was to begin working on expanding the lobby system, where each lobby would be assigned a "lobby code," and players could enter lobbies with this code. Victor had already developed a basic lobby system that I augmented with a random code generator.

`MainMenuManager.cs` : 
```csharp
public string GenerateLobbyCode()
{
    string code = "";
    int charCount = Random.Range(minCharCount, maxCharCount);
    for (int i = 0; i < charCount; i++)
    {
        code += codeChars[Random.Range(0, codeChars.Length)];
    }
    return code;
}
```

Having random code generation allowed us to "control" lobby codes, so players wouldn't try to enter a code, get stopped because the code was already used for another lobby, and have to try again...over and over. However, we still needed to check if the current generated code was used for another lobby (otherwise the lobby files would be merged), so I created the `_GetLobbyList()` coroutine to do so.

`MainMenuManager.cs` : 
```csharp
public IEnumerator _GetLobbyList()
{
    WWWForm form = new WWWForm();
    form.AddField("function", "getLobbyList");

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
            lobbyListStr = www.downloadHandler.text;
            string[] splitData = lobbyListStr.Split('/');
            lobbyList = new List<string>(splitData);

            bool foundValidCode = false;
            while (!foundValidCode)
            {
                lobbyCode = GenerateLobbyCode();
                if (!lobbyList.Contains(lobbyCode))
                {
                    lobbyCodeDisplay.text = lobbyCode;
                    foundValidCode = true;
                }
            }
        }
    }
}
```

This coroutine requests the server for a list of all lobbies currently in the server's lobby directory - it then parses the returned list, checking for the current generated code. If the code already exists, it re-runs the generator, and this loops until a valid lobby code is found.



### Other Tasks and Code Contribution
Alongside these major tasks, I worked closely in tandem with Victor to edit, tweak, add to, and debug other parts of the game code that would be too numerous to detail here. For insight on other parts of the code, see Victor's breakdown section below.

## Contributions and Breakdown - Victor
