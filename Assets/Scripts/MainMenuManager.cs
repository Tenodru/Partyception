using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject newGM;

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

    [Header("Info Panel")]
    public GameObject infoPanel;
    public Text infoPanelText;
    public Button createLobbyButton;
    public Button joinLobbyButton;
    public InputField nameInputField;
    public Text charCountDisplay;
    public Text charLimitDisplay;

    //[Header("Opening Animation")]
    //public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.current == null)
        {
            //videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "SDP_Intro_Progress_04182022.mp4");
            Instantiate(newGM, transform.position, transform.rotation);
        }
        else
        {
            GameManager.current.players.Clear();
        }
        AudioManager.current.PlayMusic("mainMenuMusic");
        StartCoroutine(_GetLobbyList());
    }

    private void Update()
    {
        if (nameInputField.enabled)
        {
            charCountDisplay.text = nameInputField.text.Length.ToString();
            if (nameInputField.text.Length < nameInputField.characterLimit / 2)
            {
                charCountDisplay.color = new Color(0.2509804f, 1f, 0.4862745f);
            }
            else if (nameInputField.text.Length == nameInputField.characterLimit)
            {
                charCountDisplay.color = new Color(1f, 0f, 0f);
            }
            else
            {
                charCountDisplay.color = new Color(1f, 0.6705883f, 0.2509804f);
            }
        }
    }

    public void ShowInfoPanel(string type)
    {
        infoPanel.SetActive(true);
        if (type == "create")
        {
            infoPanelText.text = "Enter your name:";
            nameField.gameObject.SetActive(true);
            lobbyField.gameObject.SetActive(false);
            createLobbyButton.gameObject.SetActive(true);
            joinLobbyButton.gameObject.SetActive(false);
        }
        else if (type == "join")
        {
            infoPanelText.text = "Enter your name and lobby number:";
            nameField.gameObject.SetActive(true);
            lobbyField.gameObject.SetActive(true);
            createLobbyButton.gameObject.SetActive(false);
            joinLobbyButton.gameObject.SetActive(true);
        }
    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
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

    public void CreateLobby()
    {
        if (validName(nameField.text))
        {
            StartCoroutine(EnterLobby("createLobby", nameField.text, lobbyCode));
        }
        else
        {
            AlertText.current.ToggleAlertText("Invalid name. Please choose a different name.", Color.red);
        }
    }

    public void JoinLobby()
    {
        if (validName(nameField.text))
        {
            StartCoroutine(EnterLobby("joinLobby", nameField.text, lobbyField.text));
        }
        else
        {
            AlertText.current.ToggleAlertText("Invalid name. Please choose a different name.", Color.red);
        }
    }

    public bool validName(string name)
    {
        if (name.Contains("prestart") || name.Contains("Leader:") || name.Contains("answering") || name.Contains("eliminated") || name.Contains("prestart") || name.Contains("awaiting"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public IEnumerator EnterLobby(string functionType, string playerName, string lobbyNumber)
    {
        LoadingPanel.current.ToggleLoadingPanel(true);

        WWWForm checkForm = new WWWForm();
        checkForm.AddField("function", "getPlayerList");
        checkForm.AddField("lobbyNumber", lobbyField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(gameDatabaseLink + "lobby.php", checkForm))
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
                List<string> splitDataList = new List<string>();
                foreach (string data in splitData)
                {
                    splitDataList.Add(data);
                }
                if (splitDataList.Contains(playerName))
                {
                    LoadingPanel.current.ToggleLoadingPanel(false);
                    AlertText.current.ToggleAlertText("That name already exists in this lobby.", Color.red);
                    yield break;
                }
            }
        }

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
