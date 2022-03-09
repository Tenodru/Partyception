using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnswerButton : Button
{
    [Tooltip ("The Answer associated with this Button.")]
    public Answer answer;
}
