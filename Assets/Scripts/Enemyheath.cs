using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemyheath : MonoBehaviour
{
    public int health =100;
    public void takedamage(int damage)
    {
       health -= damage;
       Debug.Log(name + "took damage: "+ damage); 
       if(health <= 0)
        {
            Die();
            
        }
    }
    public void Die()
    {
        Debug.Log(name + "has died");
        Destroy(gameObject);
        
    }
}
