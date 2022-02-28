using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTracker : MonoBehaviour
{
    public string playerName;
    public Text playerNameText;
    public Text statusText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateStatus(string status)
    {
        statusText.text = status;
    }
}
