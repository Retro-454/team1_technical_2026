using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class dash : MonoBehaviour
{
    public float dashspeed;
    public float dashduration;
    PlayerLocalmotion moveScript;
    public int dashDamage = 20;          // Damage dealt while dashing
    public float dashHitRange = 2f;      // Distance in front of player
    public float dashHitRadius = 0.75f;  // Radius of attack area
  

    public bool isShieldUp = true;

    

    private Rigidbody rb;
 

    void Start(){

        moveScript = GetComponent<PlayerLocalmotion>();
      
    }
    void Update(){
         isShieldUp = moveScript.isShieldup;

        if(Mouse.current.rightButton.wasPressedThisFrame)
            TryDash();

        
        }
    
     void TryDash()
    {
        if (!isShieldUp)return;

        StartCoroutine(Dash());
    }

    IEnumerator Dash()
{
    float startTime=Time.time;

   

    while (Time.time < startTime + dashduration)
    {
        moveScript.HandleDash(dashspeed);

        //attack
        DealDashDamage();
        yield return null;
        
    }

    

  
}
void DealDashDamage()
    {
        //player direction
        Vector3 direction = transform.forward;
        direction.y = 0;
        direction.Normalize();

        //put it infront of player
        Vector3 attackOrigin = transform.position + direction * dashHitRadius;

        Collider [] hits =  Physics.OverlapSphere(attackOrigin, dashHitRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("enemy"))
            {
                hit.GetComponent<Enemyheath>()?.takedamage(dashDamage);
            }
        }
        
    }

     
}
