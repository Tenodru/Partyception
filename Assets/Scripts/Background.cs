using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public static Background current;
    public ParticleSystem answerParticles;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
    }

    public void PlayParticleEffect()
    {
        GetComponent<Animator>().SetTrigger("answer");
        answerParticles.Play();
    }
}
