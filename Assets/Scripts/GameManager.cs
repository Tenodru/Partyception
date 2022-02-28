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

    public void JoinLobby(string name, string lobbyName)
    {
        playerName = name;
        currentLobby = lobbyName;
        LoadScene("Lobby Menu");
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
