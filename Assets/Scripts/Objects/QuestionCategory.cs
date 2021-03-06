using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Category", menuName = "AYSTTC/Question-Category Object")]
public class QuestionCategory : ScriptableObject
{
    [Tooltip("The category name.")]
    public string categoryName = "category name";

    [Tooltip("The questions associated with this category.")]
    public List<Question> questions;
}
