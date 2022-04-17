using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCard : MonoBehaviour
{
    public bool animatingQuestion = false;
    public Text questionNum;
    public Text questionText;
    public Image[] answers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateQuestionCard(int _questionNum, Question question)
    {
        questionNum.text = _questionNum.ToString();
        questionText.text = question.question;
        for(int i = 0; i < answers.Length; i++)
        {
            answers[i].GetComponentInChildren<Text>().text = question.answerList[i].answer;
            if (question.answerList[i].isCorrectAnswer)
            {
                answers[i].color = Color.green;
            }
            else
            {
                answers[i].color = Color.red;
            }
        }
    }

    public void ToggleQuestionCard()
    {
        if (!animatingQuestion)
        {
            if (GetComponent<RectTransform>().sizeDelta.y == 60)
            {
                animatingQuestion = true;
                StartCoroutine(_ToggleQuestionCard("expand"));
            }
            else
            {
                animatingQuestion = true;
                StartCoroutine(_ToggleQuestionCard("collapse"));
            }
        }
    }

    IEnumerator _ToggleQuestionCard(string type)
    {
        while (true)
        {
            if (type == "expand")
            {
                while (GetComponent<RectTransform>().sizeDelta.y < 180)
                {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y + 4f);
                    yield return new WaitForSeconds(0.01f);
                }
                GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 180);
                animatingQuestion = false;
                yield break;
            }

            if (type == "collapse")
            {
                while (GetComponent<RectTransform>().sizeDelta.y > 60)
                {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y - 4f);
                    yield return new WaitForSeconds(0.01f);
                }
                GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 60);
                animatingQuestion = false;
                yield break;
            }
        }
    }
}
