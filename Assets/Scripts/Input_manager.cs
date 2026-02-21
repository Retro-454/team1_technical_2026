using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
