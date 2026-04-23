using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermanager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocalmotion playerlocalmotion;//old
    PlayerTankMotion playerTankMotion;     // new


    public bool useTankControls = true;    // flip, right now for testing
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerlocalmotion = GetComponent<PlayerLocalmotion>();
        playerTankMotion = GetComponent<PlayerTankMotion>();
    }
    private void Update()
    {
        inputManager.HandleAllInputs();
    }
    private void FixedUpdate()
    {
      if (useTankControls)
            playerTankMotion.HandleAllMovement();
        else
            playerlocalmotion.HandleAllMovement();
        
    }
}
