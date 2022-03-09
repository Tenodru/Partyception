using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager current;
    public string playerName;
    public string currentLobby;
    public PlayerStatus playerStatus;

    public List<string> players;

    private void Awake()
    {
        current = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void JoinLobby(string name, string lobbyName, string targetLobbyScene, PlayerStatus status)
    {
        playerName = name;
        currentLobby = lobbyName;
        LoadScene(targetLobbyScene);
        playerStatus = status;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void AddPlayer(string playerName)
    {
        players.Add(playerName);
    }
}

/// <summary>
/// Whether the player is Host (creator of lobby) or Participant (joined lobby).
/// </summary>
public enum PlayerStatus { Host, Participant }

/// <summary>
/// The type of outcome that can occur after a question in AYSTTC.
/// </summary>
public enum OutcomeType { Correct, Wrong, TimeOut }
