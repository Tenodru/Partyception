using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class QASetup : MonoBehaviour
{
    public InputField question;
    public List<InputField> answerChoices;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddAnswerChoice()
    {
        int numChoices = 0;
        foreach (InputField choice in answerChoices)
        {
            if (choice.gameObject.activeInHierarchy)
            {
                numChoices += 1;
            }
        }
        if (numChoices <= 3)
        {
            answerChoices[numChoices].gameObject.SetActive(true);
        }
        else
        {
            AlertText.current.ToggleAlertText("Max answer choices.", Color.red);
        }
    }

    public void RemoveAnswerChoice()
    {
        int numChoices = 0;
        foreach (InputField choice in answerChoices)
        {
            if (choice.gameObject.activeInHierarchy)
            {
                numChoices += 1;
            }
        }
        if (numChoices > 2)
        {
            answerChoices[numChoices - 1].gameObject.SetActive(false);
        }
        else
        {
            AlertText.current.ToggleAlertText("Need at least 2 answer choices.", Color.red);
        }
    }

    public void StartQAPuzzle()
    {
        foreach (InputField choice in answerChoices)
        {
            if (choice.gameObject.activeInHierarchy)
            {
                if (choice.text == "")
                {
                    AlertText.current.ToggleAlertText("You have an empty answer choice.", Color.red);
                    return;
                }
            }
        }
        if (question.text == "")
        {
            AlertText.current.ToggleAlertText("Your question is empty.", Color.red);
            return;
        }

        CanvasSwitcher.current.SwitchCanvas("PlayerStatusCanvas");

        PlayerStatusTracker.current.StartTracking();

        StartCoroutine("_StartMinigame");
    }

    public IEnumerator _StartMinigame()
    {
        //LoadingPanel.current.ToggleLoadingPanel(true);

        string minigameData = "minigame:QA\n" +
            "question:" + question.text + "\n" +
            "correctChoice:" + answerChoices[0].text + "\n";

        foreach (InputField choice in answerChoices)
        {
            if (choice.gameObject.activeInHierarchy && answerChoices.IndexOf(choice) != 0)
            {
                minigameData += "wrongChoice" + answerChoices.IndexOf(choice) + ":" + choice.text + "\n";
            }
        }

        WWWForm form = new WWWForm();
        Debug.Log(minigameData);
        form.AddField("lobbyNumber", GameManager.current.currentLobby);
        form.AddField("minigameData", minigameData);

        using (UnityWebRequest www = UnityWebRequest.Post("https://tenodrucreative.com/tracker/minigame.php", form))
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
                if (receivedData == "successfully started minigame")
                {
                    LoadingPanel.current.ToggleLoadingPanel(false);
                    CanvasSwitcher.current.SwitchCanvas("HubCanvas");
                }
                else
                {
                    LoadingPanel.current.ToggleLoadingPanel(false);
                    Debug.Log(receivedData);
                    AlertText.current.ToggleAlertText(receivedData, Color.red);
                }
            }
        }
    }
}
