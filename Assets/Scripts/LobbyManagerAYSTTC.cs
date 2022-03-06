using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles gameplay and question looping for AYSTTC.
/// </summary>
public class LobbyManagerAYSTTC : MonoBehaviour
{
    [Tooltip ("The number of rounds, or questions, to play.")]
    public int roundCount = 5;

    /// <summary>
    /// The current round.
    /// </summary>
    int currentRound = 0;

    public static LobbyManagerAYSTTC current;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //TODO: Host must choose question category.
        // Once category is chosen, start the round loop.
        StartRoundLoop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartRoundLoop()
    {
        for (int x = 0; x < roundCount; x++)
        {
            currentRound = x;
            //TODO: Grab random question from questions list.
            //TODO: Initialize timer.
            //TODO: Display question and answer choices.

            //TODO: When timer ends, check if player's chosen answer matches correct answer.
        }
    }

    
}
