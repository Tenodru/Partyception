using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor (typeof(AnswerButton))]
public class AnswerButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        AnswerButton button = (AnswerButton)target;

        base.OnInspectorGUI();
    }
}
