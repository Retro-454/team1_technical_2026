using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Light Attack")]
    public int lightDamage = 10;
    public float lightCooldown = 0.6f;
    public float lightRange = 2f;
    public float lightRadius = 0.75f;

    [Header("Heavy Attack")]
    public int heavyDamage = 25;
    public float heavyCooldown = 1.2f;
    public float heavyChargeTime = 0.8f;
    public float heavyRadius = 1.2f;

    [Header("Spin Attack")]
    public float spinRadius = 2f;
    public float spinDuration = 0.8f;

    [Header("Combo Settings")]
    public float comboResetTime = 3f;

    private float lastLightTime;
    private float lastHeavyTime;

    private int heavyComboCount;
    private bool isHeavyAttacking;

    PlayerLocalmotion movement;
    Rigidbody rb;

    void Awake()
    {
        movement = GetComponent<PlayerLocalmotion>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            TryLightAttack();

        if (Keyboard.current.hKey.wasPressedThisFrame)
            TryHeavyAttack();
    }

    // ------------------------
    // LIGHT ATTACK
    // ------------------------

    void TryLightAttack()
    {
        if (Time.time < lastLightTime + lightCooldown)
            return;

        lastLightTime = Time.time;

        Vector3 origin = GetForwardAttackOrigin(lightRange);
        DealDamage(origin, lightRadius, lightDamage);

        Debug.Log("Light Attack");
    }

    // ------------------------
    // HEAVY ATTACK
    // ------------------------

    void TryHeavyAttack()
    {
        if (Time.time < lastHeavyTime + heavyCooldown || isHeavyAttacking)
            return;

        StartCoroutine(HeavyAttackRoutine());
    }

    IEnumerator HeavyAttackRoutine()
    {
        isHeavyAttacking = true;
        movement.canMove = false;

        Debug.Log("Charging Heavy Attack...");
        yield return new WaitForSeconds(heavyChargeTime);

        HandleComboReset();

        heavyComboCount++;

        if (heavyComboCount % 3 == 0)
            yield return SpinAttackRoutine();
        else
            PerformHeavyAttack();

        lastHeavyTime = Time.time;

        movement.canMove = true;
        isHeavyAttacking = false;
    }

    void PerformHeavyAttack()
    {
        Vector3 origin = GetForwardAttackOrigin(lightRange);
        DealDamage(origin, heavyRadius, heavyDamage);

        Debug.Log("Heavy Attack");
    }

    // ------------------------
    // SPIN ATTACK
    // ------------------------

    IEnumerator SpinAttackRoutine()
    {
        float elapsed = 0;

        while (elapsed < spinDuration)
        {
            rb.linearVelocity = Vector3.zero;
            transform.Rotate(Vector3.up * 720 * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        DealDamage(transform.position, spinRadius, heavyDamage);

        Debug.Log("Spin Attack!");
    }

    // ------------------------
    // DAMAGE SYSTEM
    // ------------------------

    void DealDamage(Vector3 origin, float radius, int damage)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("enemy"))
            {
                hit.GetComponent<Enemyheath>()?.takedamage(damage);
            }
        }
    }

    Vector3 GetForwardAttackOrigin(float distance)
    {
        Vector3 dir = transform.forward;
        dir.y = 0;
        dir.Normalize();

        return transform.position + dir * distance;
    }

    void HandleComboReset()
    {
        if (Time.time > lastHeavyTime + comboResetTime)
            heavyComboCount = 0;
    }

    // ------------------------
    // DEBUG GIZMOS
    // ------------------------

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetForwardAttackOrigin(lightRange), lightRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetForwardAttackOrigin(lightRange), heavyRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spinRadius);
    }
}