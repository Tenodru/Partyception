using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Question-Answer object. Holds a string and a list of Answers.
/// </summary>
[CreateAssetMenu(fileName = "Question", menuName = "AYSTTC/Question-Answer Object")]
public class Question : ScriptableObject
{
    [Tooltip("A question.")]
    public string question = "Enter question here.";

    [Tooltip("The question difficulty.")]
    [Range (1, 5)]
    public int difficulty = 1;

    [Tooltip("A list of answer choices.")]
    public List<Answer> answerList = new List<Answer> { new Answer(), new Answer(), new Answer(), new Answer() };
}

/// <summary>
/// An Answer object. Holds a string and a boolean.
/// </summary>
[System.Serializable]
public class Answer
{
    [Tooltip("An answer.")]
    public string answer = "enter answer here";

    [Tooltip("Whether this answer is the correct answer.")]
    public bool isCorrectAnswer = false;
}
