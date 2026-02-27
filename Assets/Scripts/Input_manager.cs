using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_manager : MonoBehaviour
{
    // Start is called before the first frame update
    Player_controls player_Controls;
    public Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;


    private void OnEnable()
    {
        if (player_Controls == null)
        {
            player_Controls = new Player_controls();

            player_Controls.Player_movement.Movement.performed += i => movementInput
            = i.ReadValue<Vector2>();

        }
        player_Controls.Enable();

    }
    private void Update()
    {
        HandleAllInputs();
        HandleShieldInput();
    }
    private void OnDisable()
    {
        player_Controls.Disable();
    }
    public void HandleAllInputs()
    {
        HandleMovementInput();

    }
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }
    private void HandleShieldInput()
    {
          // Toggle shield on K press
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            PlayerLocalmotion playerMotion = GetComponent<PlayerLocalmotion>();
            if (playerMotion != null)
            {
                playerMotion.ToggleShield();
            }
        }
        
    }


}
