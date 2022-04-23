using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCanvas : MonoBehaviour
{
    public static TransitionCanvas current;

    // Start is called before the first frame update
    void Awake()
    {
        current = this;
    }

    public void Animate(string type)
    {
        GetComponent<Animator>().SetTrigger(type);
    }
}
