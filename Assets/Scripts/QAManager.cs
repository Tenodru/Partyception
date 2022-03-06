using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAManager : MonoBehaviour
{
    [Tooltip ("The question categories.")]
    public List<QuestionCategory> categories;


    public static QAManager current;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
