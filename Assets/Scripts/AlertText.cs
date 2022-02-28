using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertText : MonoBehaviour
{
    public static AlertText current;

    // Start is called before the first frame update
    void Awake()
    {
        current = this;
    }

    public void ToggleAlertText(string alertText, Color alertColor)
    {
        StopAllCoroutines();
        GetComponent<Text>().text = alertText;
        GetComponent<Text>().color = alertColor;
        StartCoroutine(DisableAlertText(3f));
    }

    IEnumerator DisableAlertText(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Text>().text = "";
    }
}
