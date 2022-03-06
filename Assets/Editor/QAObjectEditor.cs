using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(QAObject))]
public class QAObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        QAObject script = target as QAObject;

        Answer correctAnswer = null;

        /*
        foreach (Answer a in script.answerList)
        {
            //a.isCorrectAnswer = GUILayout.Toggle(a.isCorrectAnswer, "Correct Answer?");
            if (a.isCorrectAnswer)
            {
                //GUILayout.Toggle(a.isCorrectAnswer, "Correct Answer Chosen?");
                correctAnswer = a;
                break;
            }
        }

        foreach (Answer a in script.answerList)
        {
            if (correctAnswer != null && a != correctAnswer)
            {
                //a.isCorrectAnswer;
            }
        }*/
    }
}
