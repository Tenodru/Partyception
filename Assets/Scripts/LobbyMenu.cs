using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Networking;

public class LobbyMenu : MonoBehaviour
{
    public bool lobbyLeader = false;
    public Transform playerCardPool;
    public List<GameObject> playerCards;
    public List<string> playerNames;
    public Button startButton;
    private float timer = 0;

    [TextArea(1, 5)]
    public string gameDatabaseLink;
    public string mainScene;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < playerCardPool.childCount; i++)
        {
            playerCards.Add(playerCardPool.GetChild(i).gameObject);
        }
        StartCoroutine("_GetPlayerList");
        StartCoroutine("_CheckForStart");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    public IEnumerator _GetPlayerList()
    {
        while (true)
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
                    foreach (string data in splitData)
                    {
                        if (data != "")
                        {
                            if (!GameManager.current.players.Contains(data))
                            {
                                GameManager.current.AddPlayer(data);
                            }
                            foreach (GameObject playerCard in playerCards)
                            {
                                if (!playerCard.activeInHierarchy && !playerNames.Contains(data))
                                {
                                    playerCard.SetActive(true);
                                    playerCard.GetComponent<PlayerCard>().AssignPlayerName(data);
                                    playerNames.Add(data);
                                    if (data.Contains("Leader: "))
                                    {
                                        //do something special to indicate they are leader
                                    }
                                    if (data == GameManager.current.playerName || data == "Leader: " + GameManager.current.playerName)
                                    {
                                        playerCard.GetComponent<PlayerCard>().MakeCurrent();
                                        if (data.Contains("Leader: "))
                                        {
                                            lobbyLeader = true;
                                            startButton.interactable = true;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public IEnumerator _StartGame()
    {
        LoadingPanel.current.ToggleLoadingPanel(true);

        WWWForm form = new WWWForm();
        form.AddField("function", "startGame");
        form.AddField("lobbyNumber", GameManager.current.currentLobby);
        form.AddField("playerName", GameManager.current.playerName);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "lobby.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                LoadingPanel.current.ToggleLoadingPanel(false);
                AlertText.current.ToggleAlertText(www.error, Color.red);
            }
            else
            {
                string receivedData = www.downloadHandler.text;
                if (receivedData == "successfully started game")
                {
                    LoadingPanel.current.ToggleLoadingPanel(false);
                    GameManager.current.LoadScene(mainScene);
                }
                else
                {
                    LoadingPanel.current.ToggleLoadingPanel(false);
                    AlertText.current.ToggleAlertText(receivedData, Color.red);
                }
            }
        }
    }

    public void StartGame()
    {
        StartCoroutine("_StartGame");
    }

    public IEnumerator _CheckForStart()
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
                    if (receivedData != "" && !lobbyLeader)
                    {
                        GameManager.current.playerStatus = PlayerStatus.Participant;
                        GameManager.current.LoadScene(mainScene);
                        yield break;
                    }
                }

                AlertText.current.ToggleAlertText(timer.ToString(), Color.green);
                timer = 0;
            }
        }
    }
}
