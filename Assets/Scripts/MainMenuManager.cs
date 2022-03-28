using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public InputField nameField;
    public InputField lobbyField;
    public TextMeshProUGUI lobbyCodeDisplay;

    [TextArea(1, 5)]
    public string gameDatabaseLink;
    public string lobbyScene;

    [Header("Lobby Code Parameters")]
    public int minCharCount = 5;
    public int maxCharCount = 8;

    [HideInNormalInspector] public string lobbyCode = "";
    [HideInNormalInspector] public string lobbyListStr = "";
    [HideInNormalInspector] public List<string> lobbyList = new List<string>();
    [HideInNormalInspector] public const string codeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(_GetLobbyList());
    }

    /// <summary>
    /// Generates a lobby code from the valid characters in codeChars.
    /// </summary>
    /// <returns>A lobby code as a string.</returns>
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

    public void Lobby(string functionName)
    {
        //StartCoroutine(EnterLobby(functionName, nameField.text, lobbyField.text));
        StartCoroutine(EnterLobby(functionName, nameField.text, lobbyField.text));
    }

    public void CreateLobby ()
    {
        StartCoroutine(EnterLobby("createLobby", nameField.text, lobbyCode));
    }

    public void JoinLobby()
    {
        StartCoroutine(EnterLobby("joinLobby", nameField.text, lobbyField.text));
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
                        GameManager.current.JoinLobby(playerName, lobbyNumber, lobbyScene, PlayerStatus.Host);
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
                        GameManager.current.JoinLobby(playerName, lobbyNumber, lobbyScene, PlayerStatus.Participant);
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
}
