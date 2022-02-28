using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private float timer;
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.01f)
        {
            GetComponent<RectTransform>().Rotate(0, 0, rotateSpeed);
            timer = 0;
        }
    }
}
