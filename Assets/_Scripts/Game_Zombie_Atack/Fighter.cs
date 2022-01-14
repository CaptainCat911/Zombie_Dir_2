using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    // Health
    public int currentHealth;
    public int maxHealth = 100;

    //public float attackRange = 1f;    
    public float pushRecoverySpeed = 0.2f;

    //Immunity
    public float immuneTime = 0f;
    protected float lastImmune;

    //Push
    protected Vector3 pushDirection;
       

    void Awake()
    {
        currentHealth = maxHealth;
    }



    // All fighters can ReceiveDamage / Die
    protected virtual void ReceiveDamage(Damage dmg)
    {
 /*       if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;*/

                // Damage             
            dmg.damageAmount = Mathf.Clamp(dmg.damageAmount, 0, int.MaxValue);
            //Debug.Log(dmg.damageAmount);
            currentHealth -= dmg.damageAmount;

                // Push
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;


             /*   // Text 
            if (this.gameObject.name == "Player")
            {
                GameManager.instance.ShowText(dmg.damageAmount.ToString(), 15, Color.red, transform.position, Vector3.up * 30, 0.5f);
            }
            else
            {
                GameManager.instance.ShowText(dmg.damageAmount.ToString(), 15, Color.white, transform.position, Vector3.up * 30, 0.5f);
            }*/

                //Death
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Death();
            }
        /*}*/
    }

    protected virtual void Death()
    {
        // Die in some way
        // This method is meant to be overwritten
        Debug.Log(transform.name + " died.");
    }
}