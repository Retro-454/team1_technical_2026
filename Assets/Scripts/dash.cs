using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class dash : MonoBehaviour
{
    public float dashspeed;
    public float dashduration;
    PlayerLocalmotion moveScript;
  

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
        yield return null;
        
    }

    

  
}

     
}
