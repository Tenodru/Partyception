using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public Text playerName;
    public Image leaderIcon;
    public Sprite mrPuzzlesIcon;

    public void AssignPlayerName(string name)
    {
        playerName.text = name;
    }

    public void MakeLeader()
    {
        leaderIcon.sprite = mrPuzzlesIcon;
        leaderIcon.color = Color.white;
    }

    public void MakeCurrent()
    {
        GetComponent<Image>().color = Color.yellow;
    }
}
