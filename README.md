# Partyception
 The code repository for the Partyception party game.

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

### The Game Loop
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

```csharp
public string GetQuestionID(int cIndex, int qIndex)
{
    return ("." + cIndex.ToString() + "Q" + qIndex.ToString());
}
```

Then, the `_SendQuestion()` coroutine sends the question ID via a POST request using Unity's Networking library. We specify the PHP script we want to send the request to, and include the necessary parameters. The web server then stores this question.

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

```csharp
public enum TimerPurpose { DuringRound, EndOfRoundSafe, EndOfRoundEliminated, PreStart, EndOfGame }
```

Up until this point, the other players' clients will be periodically sending requests to the web server checking for the game start via the `_CheckForGameStart()` coroutine. 

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
