using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
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

    //Heavy attack
    public int heavyAttackDamage = 25;
    public float heavyAttackCooldown = 1.2f;
    public float heavyAttackChargeTime = 0.8f;

    //spin att
    public float spinAttackRadius = 2f;
    public float spinAttackDuration = 0.8f;

    private int heavyAttackCounter = 0;
    private bool isHeavyAttacking = false;

    PlayerLocalmotion playerMovement;
    Rigidbody playerRigidBody;
    public float heavyComboResetTime = 3f;
    private float lastHeavyTime;
    public float heavyAttackRadius = 1.2f;

    void Awake()
    {
    playerMovement = GetComponent<PlayerLocalmotion>();
     playerRigidBody = GetComponent<Rigidbody>();
    }   

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
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            HeavyAttack();
        }
    } 
    void HeavyAttack()
    {
        if (Time.time < lastAttackTime + heavyAttackCooldown || isHeavyAttacking)
        {
            return;
        }
            

        StartCoroutine(HeavyAttackRoutine());

        
    }
    IEnumerator HeavyAttackRoutine()
    {
        isHeavyAttacking=true;
        playerMovement.canMove = false;
        

        Debug.Log("Charging Heavy Attack");
        yield return new WaitForSeconds(heavyAttackChargeTime);
        heavyAttackCounter++;

        if (heavyAttackCounter%3==0)
        {
           yield return StartCoroutine(SpinAttackRoutine());
        }
        else
        {
            PerformAttack(heavyAttackDamage,heavyAttackRadius);
        }

        if (Time.time > lastHeavyTime + heavyComboResetTime)
        {
         heavyAttackCounter = 0;
        }
        lastHeavyTime=Time.time;
        heavyAttackCounter++;
        playerMovement.canMove = true;
        isHeavyAttacking=false;
    }
   
   IEnumerator SpinAttackRoutine()
{
    playerRigidBody.linearVelocity = Vector3.zero;

    float elapsed = 0;
    GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    debugSphere.transform.position = transform.position;
    debugSphere.transform.localScale = Vector3.one * spinAttackRadius * 2;

    Material mat = new Material(Shader.Find("Standard"));
    mat.color = Color.blue;
    debugSphere.GetComponent<MeshRenderer>().material = mat;

        // disable physics
    debugSphere.GetComponent<Collider>().enabled = false;

    Destroy(debugSphere, spinAttackDuration);       

    while (elapsed < spinAttackDuration)
    {
        playerRigidBody.linearVelocity = Vector3.zero;
        transform.Rotate(Vector3.up * 720 * Time.deltaTime);

        elapsed += Time.deltaTime;
        yield return null;
    }

    // Damage happens ONCE at the end
    Collider[] hits = Physics.OverlapSphere(transform.position, spinAttackRadius);

    foreach (Collider hit in hits)
    {
        if (hit.CompareTag("enemy"))
        {
            hit.GetComponent<Enemyheath>()?.takedamage(heavyAttackDamage);
        }
    }

    Debug.Log("Spin Attack!");
}
    void LightAttack()
    {
        // Prevent spamming attacks
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        PerformAttack(lightAttackDamage,attackRange);
    }

    void PerformAttack(int damage,float radius)
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
        Collider[] hits = Physics.OverlapSphere(attackOrigin, radius);

        foreach (Collider hit in hits) { if (hit.CompareTag("enemy")) { hit.GetComponent<Enemyheath>()?.takedamage(damage); } } 

        Debug.Log("Attack performed at " + attackOrigin);
    }
}
