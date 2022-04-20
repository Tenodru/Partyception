using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public Text playerName;
    public Image playerIcon;
    public Sprite leaderIcon;

    public void AssignPlayerName(string name)
    {
        playerName.text = name;
    }

    public void MakeCurrent()
    {
        GetComponent<Image>().color = Color.yellow;
    }

    public void MakeLeader()
    {
        playerIcon.gameObject.SetActive(true);
        playerIcon.sprite = leaderIcon;
    }
}
