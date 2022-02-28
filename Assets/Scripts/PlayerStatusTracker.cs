using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerStatusTracker : MonoBehaviour
{
    public static PlayerStatusTracker current;
    public Transform playerTrackersPool;
    public List<PlayerTracker> playerTrackers;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerTrackersPool.childCount; i++)
        {
            playerTrackers.Add(playerTrackersPool.GetChild(i).GetComponent<PlayerTracker>());
        }
    }

    public void StartTracking()
    {
        foreach (PlayerTracker obj in playerTrackers)
        {
            obj.gameObject.SetActive(false);
        }

        foreach (string player in GameManager.current.players)
        {
            foreach(PlayerTracker obj in playerTrackers)
            {
                if (!obj.gameObject.activeInHierarchy)
                {
                    obj.gameObject.SetActive(true);
                    obj.playerName = player;
                    obj.playerNameText.text = player;
                    break;
                }
            }
        }
        StartCoroutine("_UpdatePlayerTrackers");
    }

    public IEnumerator _UpdatePlayerTrackers()
    {
        while (true)
        {
            WWWForm form = new WWWForm();
            form.AddField("function", "retrieve");
            form.AddField("lobbyNumber", GameManager.current.currentLobby);

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
                    string[] splitData = receivedData.Split('\n');
                    foreach (string data in splitData)
                    {
                        if (data != "")
                        {
                            string[] playerStatus = data.Split(':');
                            if (GameManager.current.players.Contains(playerStatus[0]))
                            {
                                foreach (PlayerTracker playerTracker in playerTrackers)
                                {
                                    if (playerTracker.playerName == playerStatus[0])
                                    {
                                        playerTracker.UpdateStatus(playerStatus[1]);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
