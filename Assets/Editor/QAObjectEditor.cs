using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Question))]
public class QAObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Question script = target as Question;

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
