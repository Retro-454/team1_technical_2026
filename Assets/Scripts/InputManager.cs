using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    Player_controls controls;
    PlayerLocalmotion playerMotion;

    [Header("Movement Input")]
    public Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;

    void Awake()
    {
        playerMotion = GetComponent<PlayerLocalmotion>();
    }

    void OnEnable()
    {
        if (controls == null)
        {
            controls = new Player_controls();

            controls.Player_movement.Movement.performed += ctx =>
                movementInput = ctx.ReadValue<Vector2>();

            controls.Player_movement.Movement.canceled += ctx =>
                movementInput = Vector2.zero;
        }

        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
    }
    public void HandleAllInputs()
{
    UpdateMovementValues();
    HandleShieldInput();
}

    // ------------------------
    // MOVEMENT INPUT
    // ------------------------

    void UpdateMovementValues()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }

    // ------------------------
    // SHIELD INPUT
    // ------------------------

    void HandleShieldInput()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame && playerMotion != null)
        {
            playerMotion.ToggleShield();
        }
    }
}