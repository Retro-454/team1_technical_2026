using UnityEngine;


public class PlayerTankMotion : MonoBehaviour
{
    InputManager input;
    Rigidbody rb;

    [Header("Movement")]
    public float movementSpeed = 7f; //foward/backward speed
    public float rotationSpeed = 120f; //degrees rotated per sec

    [Header("Shield")]
    public GameObject shieldObject;
    public bool isShieldup = false;
    public float shieldMovementMultiplier = 0.5f;

     [Header("State")]
    public bool canMove = true;

    void Awake()
    {
        input = GetComponent<InputManager>();
           rb = GetComponent<Rigidbody>();
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

       
        HandleTankMovement();
        HandleTankRotation();
    }
     // ────────────────────────────────────────────────────────────────
    // ROTATION — horizontal input spins the player left or right
    // ────────────────────────────────────────────────────────────────
    void HandleTankRotation()
    {
        float turnAmount = input.horizontalInput*rotationSpeed*Time.deltaTime;
        Quaternion turnDelta = Quaternion.Euler(0f, turnAmount, 0f);
         rb.MoveRotation(rb.rotation * turnDelta);
    }
      // ────────────────────────────────────────────────────────────────
    // MOVEMENT — vertical input pushes forward/back along facing dir
    // ────────────────────────────────────────────────────────────────
    void HandleTankMovement()
    {
        float speed = isShieldup ? movementSpeed*shieldMovementMultiplier : movementSpeed;

        Vector3 moveDir=transform.forward * input.verticalInput;

        rb.linearVelocity = new Vector3(
            moveDir.x * speed,
            rb.linearVelocity.y,   // preserve gravity / vertical momentum
            moveDir.z * speed
        );
    }
     // ------------------------
    // DASH
    // ------------------------

    public void HandleDash(float dashSpeed)
    {
        if (!canMove)
            return;

         rb.AddForce(transform.forward * dashSpeed, ForceMode.Impulse);
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
