using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    public static CanvasSwitcher current;
    public List<Canvas> canvases;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Canvas[] temp = Canvas.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in temp)
        {
            if (canvas.name != "MainCanvas")
            {
                canvases.Add(canvas);
            }
        }
        SwitchCanvas("HubCanvas");
    }

    public void SwitchCanvas(string canvasName)
    {
        foreach (Canvas canvas in canvases)
        {
            if (canvas.name == canvasName)
            {
                canvas.enabled = true;
            }
            else
            {
                canvas.enabled = false;
            }
        }
    }
}
