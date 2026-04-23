using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemyheath : MonoBehaviour
{
    public int health =100;
    Rigidbody rb;
    UnityEngine.AI.NavMeshAgent agent;
    bool isKnockedBack = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>(); // null-safe if no agent present
    }
    public void takedamage(int damage)
    
    {
       health -= damage;
       Debug.Log(name + "took damage: "+ damage); 
       if(health <= 0)
        {
            Die();
            
        }
    }
    // Called by PlayerDash — pushDir should already be normalized
    public void TakePush(Vector3 pushDir, float pushForce)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackRoutine(pushDir, pushForce));
    }

    IEnumerator KnockbackRoutine(Vector3 pushDir, float pushForce)
    {
        isKnockedBack = true;

        // Disable NavMeshAgent so it doesn't fight the physics push
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Warp the Rigidbody off the NavMesh so physics takes over cleanly
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }

        // Wait for the knockback to settle (~0.5s is enough for repositioning)
        yield return new WaitForSeconds(0.5f);

        // Re-enable NavMesh and snap back onto it
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        isKnockedBack = false;
    }
    public void Die()
    {
        Debug.Log(name + "has died");
        Destroy(gameObject);
        
    }
}
