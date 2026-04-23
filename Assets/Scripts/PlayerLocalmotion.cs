using UnityEngine;

public class PlayerLocalmotion : MonoBehaviour
{
    Vector3 moveDirection;

    InputManager inputManager;
    
    Transform cameraTransform;
    Rigidbody rb;


    [Header("Movement")]
    public float movementSpeed = 7f;
    public float rotationSpeed = 15f;

    [Header("Shield")]
    public GameObject shieldObject;
    public bool isShieldup = false;
    public float shieldMovementMultiplier = 0.5f;

    [Header("State")]
    public bool canMove = true;

    void Awake()
    {
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    // ------------------------
    // MAIN MOVEMENT LOOP
    // ------------------------

    public void HandleAllMovement()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        CalculateMoveDirection();
        HandleMovement();
        HandleRotation();
    }

    // ------------------------
    // MOVEMENT
    // ------------------------

    void CalculateMoveDirection()
    {
        moveDirection =
            cameraTransform.forward * inputManager.verticalInput +
            cameraTransform.right * inputManager.horizontalInput;

        moveDirection.y = 0;
        moveDirection.Normalize();
    }

    void HandleMovement()
    {
        float speed = isShieldup
            ? movementSpeed * shieldMovementMultiplier
            : movementSpeed;

        rb.linearVelocity = moveDirection * speed;
    }

    // ------------------------
    // ROTATION
    // ------------------------

    void HandleRotation()
    {
        if (moveDirection == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // ------------------------
    // DASH
    // ------------------------

    public void HandleDash(float dashSpeed)
    {
        if (!canMove)
            return;

        Vector3 dashDirection =
            moveDirection != Vector3.zero
            ? moveDirection
            : transform.forward;

        rb.AddForce(dashDirection.normalized * dashSpeed, ForceMode.Impulse);
    }

    // ------------------------
    // SHIELD
    // ------------------------

    public void ToggleShield()
    {
        isShieldup = !isShieldup;

        if (shieldObject != null)
            shieldObject.SetActive(isShieldup);
    }
}