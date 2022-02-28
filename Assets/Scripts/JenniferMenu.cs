using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JenniferMenu : MonoBehaviour
{
    public bool inMinigame = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("_CheckMinigameStatus");
    }


    public IEnumerator _CheckMinigameStatus()
    {
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Get("https://tenodrucreative.com/tracker/lobbies/" + GameManager.current.currentLobby + "/minigameStatus.txt"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string receivedData = www.downloadHandler.text;
                    if (receivedData != "none" && !inMinigame)
                    {
                        ReadMinigameData(receivedData);
                        inMinigame = true;
                        StopAllCoroutines();
                    }
                }
            }
        }
    }

    public void ReadMinigameData(string minigameData)
    {
        string[] splitData = minigameData.Split('\n');
        string[] minigameLine = splitData[0].Split(':');
        if (minigameLine[1] == "QA")
        {
            QAMinigame.FindObjectOfType<QAMinigame>().SetUpMinigame(minigameData);
        }
    }
}
