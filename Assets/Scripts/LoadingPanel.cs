using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    public static LoadingPanel current;
    public GameObject loadingScreen;

    // Start is called before the first frame update
    void Awake()
    {
        current = this;
    }

    public void ToggleLoadingPanel(bool active)
    {
        loadingScreen.SetActive(active);
    }
}
