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
We wanted to create a game that was straightforward and easy to grasp. From the start, we knew our main game loop was gonna be pretty simple - you enter a game, and try to answer a number of questions until you reach the end.
![Main Game Loop Flowchart](https://github.com/Tenodru/Partyception/blob/25e4aa505c1dd881a2f8a55a98c0d6696e805cb6/Other/Readme%20Resources/Alex/Game%20Loop%20Flowchart%201.png)
