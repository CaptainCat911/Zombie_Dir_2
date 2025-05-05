using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    // Health
    public int currentHealth;
    public int maxHealth = 100;
    public int armor;                           // броня
    public float armorProtection = 1;           // уменьшение урона
    public bool armored;                        // есть ли броня

    //public float attackRange = 1f;    
    public float pushRecoverySpeed = 0.2f;

    //Push
    protected Vector3 pushDirection;
    


    void Awake()
    {
        //currentHealth = maxHealth;        
    }



    // All fighters can ReceiveDamage / Die
    protected virtual void ReceiveDamage(Damage dmg)
    {
            // Damage             
        dmg.damageAmount = Mathf.Clamp(dmg.damageAmount, 0, int.MaxValue);
        //Debug.Log(dmg.damageAmount);
        int takenDamage = Mathf.CeilToInt( dmg.damageAmount / armorProtection);
        //Debug.Log(takenDamage);
        currentHealth -= takenDamage;
        armor -= Mathf.CeilToInt(takenDamage / 2);

        // Push (убрал пока что (заменил))
        //pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;


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
        if (armor <= 0)
        {
            armor = 0;
            armorProtection = 1;
            armored = false;
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