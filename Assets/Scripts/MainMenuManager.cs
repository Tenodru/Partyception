using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenuManager : MonoBehaviour
{
    public InputField nameField;
    public InputField lobbyField;

    [TextArea(1,5)]
    public string gameDatabaseLink;
    public string lobbyScene;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Lobby(string functionName)
    {
        StartCoroutine(EnterLobby(functionName, nameField.text, lobbyField.text));
    }

    public IEnumerator EnterLobby(string functionType, string playerName, string lobbyNumber)
    {
        LoadingPanel.current.ToggleLoadingPanel(true);

        WWWForm form = new WWWForm();
        form.AddField("function", functionType);
        form.AddField("lobbyNumber", lobbyNumber);
        form.AddField("playerName", playerName);

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
                Debug.Log(receivedData);
                if (functionType == "createLobby")
                {
                    if (receivedData == "lobby created")
                    {
                        LoadingPanel.current.ToggleLoadingPanel(false);
                        GameManager.current.JoinLobby(playerName, lobbyNumber, lobbyScene);
                    }
                    else if (receivedData == "lobby already exists")
                    {
                        LoadingPanel.current.ToggleLoadingPanel(false);
                        AlertText.current.ToggleAlertText("Lobby already exists.", Color.red);
                    }
                    else
                    {
                        LoadingPanel.current.ToggleLoadingPanel(false);
                        AlertText.current.ToggleAlertText(receivedData, Color.red);
                    }
                }
                else if (functionType == "joinLobby")
                {
                    Debug.Log("trying to join");
                    if (receivedData == "lobby successfully joined")
                    {
                        LoadingPanel.current.ToggleLoadingPanel(false);
                        GameManager.current.JoinLobby(playerName, lobbyNumber, lobbyScene);
                    }
                    else if (receivedData == "lobby does not exist")
                    {
                        LoadingPanel.current.ToggleLoadingPanel(false);
                        AlertText.current.ToggleAlertText("Lobby does not exist.", Color.red);
                    }
                    else
                    {
                        LoadingPanel.current.ToggleLoadingPanel(false);
                        AlertText.current.ToggleAlertText(receivedData, Color.red);
                    }
                }
            }
        }
    }
}
