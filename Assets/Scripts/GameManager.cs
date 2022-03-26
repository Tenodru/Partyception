using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public string gameDatabaseLink = "";
    public string playerName;
    public string currentLobby;
    public PlayerStatus playerStatus;

    public List<string> players;

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            current = this;
        }
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

    /// <summary>
    /// Deletes the current lobby from the website database.
    /// </summary>
    /// <returns></returns>
    public IEnumerator _DeleteLobby()
    {
        Debug.Log("Running deletion coroutine.");

        WWWForm form = new WWWForm();
        form.AddField("function", "deleteLobby");
        form.AddField("lobbyNumber", currentLobby);

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
}

/// <summary>
/// Whether the player is Host (creator of lobby) or Participant (joined lobby).
/// </summary>
public enum PlayerStatus { Host, Participant }

/// <summary>
/// The type of outcome that can occur after a question in AYSTTC.
/// </summary>
public enum OutcomeType { Correct, Wrong, TimeOut }
