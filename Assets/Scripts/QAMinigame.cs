using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class QAMinigame : MonoBehaviour
{
    public Text questionText;
    public List<Button> answerChoices;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetUpMinigame(string minigameData)
    {
        CanvasSwitcher.current.SwitchCanvas("QAMinigameCanvas");
        int lineNumber = 0;
        string correctAnswer = "";
        List<string> possibleAnswers = new List<string>();
        string[] splitData = minigameData.Split('\n');
        foreach (string line in splitData)
        {
            if (splitData[lineNumber] != "")
            {
                string[] minigameLine = splitData[lineNumber].Split(':');
                if (lineNumber == 1)
                {
                    Debug.Log(minigameLine[1]);
                    questionText.text = minigameLine[1];
                }
                else if (lineNumber == 2)
                {
                    correctAnswer = minigameLine[1];
                    possibleAnswers.Add(minigameLine[1]);
                }
                else if (lineNumber > 2)
                {
                    possibleAnswers.Add(minigameLine[1]);
                }
                lineNumber += 1;
            }
        }

        foreach (Button button in answerChoices)
        {
            button.gameObject.SetActive(false);
        }

        for (int i = 0; i < possibleAnswers.Count; i++)
        {
            string temp = possibleAnswers[i];
            int randomIndex = Random.Range(i, possibleAnswers.Count);
            possibleAnswers[i] = possibleAnswers[randomIndex];
            possibleAnswers[randomIndex] = temp;
            answerChoices[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < possibleAnswers.Count; i++)
        {
            answerChoices[i].GetComponentInChildren<Text>().text = possibleAnswers[i];
            if (possibleAnswers[i] == correctAnswer)
            {
                answerChoices[i].onClick.RemoveAllListeners();
                answerChoices[i].onClick.AddListener(() => AnswerChoice(true));
            }
            else
            {
                answerChoices[i].onClick.RemoveAllListeners();
                answerChoices[i].onClick.AddListener(() => AnswerChoice(false));
            }
        }
    }

    public void AnswerChoice(bool correct)
    {
        if (correct)
        {
            Debug.Log("correct");
            StartCoroutine(_UpdateStatus("correct"));

        }
        else
        {
            Debug.Log("incorrect");
            StartCoroutine(_UpdateStatus("incorrect"));
        }
    }

    public IEnumerator _UpdateStatus(string newStatus)
    {
        WWWForm form = new WWWForm();
        form.AddField("function", "update");
        form.AddField("lobbyNumber", GameManager.current.currentLobby);
        form.AddField("newStatus", newStatus);

        using (UnityWebRequest www = UnityWebRequest.Post("https://tenodrucreative.com/tracker/updatePlayerStatus.php", form))
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
                if (receivedData == "success")
                {
                    CanvasSwitcher.current.SwitchCanvas("HubCanvas");
                }
                else
                {
                    AlertText.current.ToggleAlertText(receivedData, Color.red);
                }
            }
        }
    }
}
