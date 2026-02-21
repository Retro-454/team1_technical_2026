using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermanager : MonoBehaviour
{
    Input_manager inputManager;
    PlayerLocalmotion playerlocalmotion;

    private void Awake()
    {
        inputManager = GetComponent<Input_manager>();
        playerlocalmotion = GetComponent<PlayerLocalmotion>();
    }
    private void Update()
    {
        inputManager.HandleAllInputs();
    }
    private void FixedUpdate()
    {
        playerlocalmotion.HandleAllMovement();
        
    }
}
