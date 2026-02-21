using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int lightAttackDamage = 10;
    public float attackCooldown = 0.6f;

    [Header("Hitbox Settings")]
    public float attackRange = 2f;
    public float attackRadius = 0.75f;

    private float lastAttackTime;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Spacebar triggers light attack for testing
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            LightAttack();
        }
    } 

    void LightAttack()
    {
        // Prevent spamming attacks
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        PerformAttack(lightAttackDamage);
    }

    void PerformAttack(int damage)
    {
        // Player forward direction
        Vector3 direction = transform.forward;
        direction.y = 0;
        direction.Normalize();

        // Attack position slightly above player base
        Vector3 attackOrigin = transform.position + transform.forward.normalized * attackRange;

        
        // --- DEBUG HITBOX VISUAL ---
        GameObject debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debugCube.transform.position = attackOrigin;
        debugCube.transform.localScale = new Vector3(attackRadius * 2, 0.5f, attackRadius * 2);

        // Make it red so you can see it
       Material mat = new Material(Shader.Find("Standard"));
       mat.color = Color.red;
        debugCube.GetComponent<MeshRenderer>().material = mat;
      Destroy(debugCube, 0.5f); // disappear after 0.5 seconds */

        // --- DAMAGE LOGIC ---
        Collider[] hits = Physics.OverlapSphere(attackOrigin, attackRadius);

        foreach (Collider hit in hits) { if (hit.CompareTag("enemy")) { hit.GetComponent<Enemyheath>()?.takedamage(damage); } } 

        Debug.Log("Attack performed at " + attackOrigin);
    }
}
