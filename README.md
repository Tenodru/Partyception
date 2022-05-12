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
![Inspector 1](https://github.com/Tenodru/Partyception/blob/9603e1fa1ee0bb9d937300aa42ed1c63997ad948/Other/Readme%20Resources/Alex/ScriptableObject%20Games.PNG)
![Inspector 2](https://github.com/Tenodru/Partyception/blob/9603e1fa1ee0bb9d937300aa42ed1c63997ad948/Other/Readme%20Resources/Alex/ScriptableObject%20Category.PNG)



