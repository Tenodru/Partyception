using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QAObject", menuName = "AYSTTC/Question-Answer Object")]
public class QAObject : ScriptableObject
{
    [Tooltip("A question.")]
    public string question = "Enter question here.";
    [Tooltip("A list of answer choices.")]
    public List<string> answerList = new List<string> { "answer 1", "answer 2", "answer 3", "answer 4" };
}
