using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.4f;

    [Header("Dash Damage")]
    public int dashDamage = 20;
    public float dashRange = 2f;
    public float dashRadius = 0.75f;

    [Header("Push Settings")]
    [Tooltip("Base force applied to an enemy with weight = 1. Heavier enemies receive proportionally less.")]
    public float basePushForce = 8f;


    PlayerLocalmotion movement;

    bool isDashing;

    // Tracks enemies already pushed this dash — cleared each new dash
    HashSet<Collider> pushedThisDash = new HashSet<Collider>();

    void Start()
    {
        movement = GetComponent<PlayerLocalmotion>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
            TryDash();
    }

    // ------------------------
    // DASH LOGIC
    // ------------------------

    void TryDash()
    {
        if (!movement.isShieldup || isDashing)
            return;

        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        pushedThisDash.Clear();

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            movement.HandleDash(dashSpeed);
            DealDashDamage();

            yield return null;
        }

        isDashing = false;
    }

    // ------------------------
    // DAMAGE + PUSH LOGIC
    // ------------------------

void DealDashDamage()
{
    Vector3 origin = GetDashOrigin();
    Debug.Log("Dash origin: " + origin + " | Player pos: " + transform.position);

    Collider[] hits = Physics.OverlapSphere(origin, dashRadius);
    Debug.Log("Hits detected: " + hits.Length);

    foreach (Collider hit in hits)
    {
        if (hit.transform.root == transform.root) continue;

        if (hit.CompareTag("enemy"))
        {
            hit.GetComponentInParent<Enemyheath>()?.takedamage(dashDamage);
            Debug.Log("Hit: " + hit.gameObject.name + " | Tag: " + hit.tag);

            // Push only once per enemy per dash
            if (!pushedThisDash.Contains(hit))
            {
                pushedThisDash.Add(hit);
                ApplyPush(hit);
            }
        }
    }
}
    void ApplyPush(Collider enemyCollider)
    {
        Enemyheath health = enemyCollider.GetComponent<Enemyheath>();
        EnemyStats stats = enemyCollider.GetComponent<EnemyStats>();

        if (health == null) return;

        // Fallback weight of 3 if EnemyStats is missing
        float weight = stats != null ? Mathf.Max(stats.weight, 0.1f) : 3f;

        // Heavier enemies get pushed less — force scales inversely with weight
        float pushForce = basePushForce / weight;

        // Push direction: away from player, horizontal only
        Vector3 pushDir = (enemyCollider.transform.position - transform.position);
        pushDir.y = 0;
        pushDir.Normalize();

        health.TakePush(pushDir, pushForce);
    }

    Vector3 GetDashOrigin()
    {
        Vector3 direction = transform.forward;
        direction.y = 0;
        direction.Normalize();

        return transform.position + direction * dashRange;
    }
}