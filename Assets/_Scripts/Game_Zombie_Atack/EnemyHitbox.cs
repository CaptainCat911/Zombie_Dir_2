using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Collidable
{
 
    

    // Damage
    public int damage = 1;  
    public float pushForce = 2;
    public float cooldown = 0.5f;
    public float hitBoxDelay = 1;  // задержка перед атакой (для совпадения с анимацией), переделать на ивент анимации     
    public float attackRadiusHitBox = 1;    // радиус хитбокса

    private float lastSwing;
    public bool attacking = false;

    public Enemy_old enemy;


    protected override void Start()
    {
        base.Start();

        enemy = GetComponentInParent<Enemy_old>();
    }

    public void Attack()
    {
        if (Time.time - lastSwing > cooldown)
        {
            attacking = true;
            lastSwing = Time.time;
            StartCoroutine(AttackWithDelay(hitBoxDelay));            
            enemy.anim.SetTrigger("Swing");            
        }
    }


    IEnumerator AttackWithDelay(float delay_1)
    {
        yield return new WaitForSeconds(delay_1);
        Collider[] collidersHitbox = Physics.OverlapSphere(this.transform.position, attackRadiusHitBox, enemy.layerPlayer);    
        foreach (Collider enObjectBox in collidersHitbox)
        {
            //Debug.Log(enObject.name);
            if (enObjectBox == null)
            {
                continue;
            }

            if (enObjectBox.tag == "Fighter")
            {
                Damage dmg = new Damage()
                {
                    damageAmount = damage,
                    origin = transform.position,
                    pushForce = pushForce
                };
                enObjectBox.SendMessage("ReceiveDamage", dmg);
            }
            collidersHitbox = null;
        }
        yield return new WaitForSeconds(0.7f);
        attacking = false;
    }






    /*
    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Fighter")
        {


            Damage dmg = new Damage()
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);

            //Debug.Log(coll.name);            
        }
    }
    */

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadiusHitBox);
    }

}
