using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granate : MonoBehaviour
{
    public float delay = 3f;                // задержка перед взрывом
    public float radius = 4f;               // радиус взрыва
    public float radiusBig = 8f;            // радиус дополнительного взрыва
    public int damage = 300;                // основной урон
    public int damageBigRadius = 76;        // дополнительный урон
    public float pushForce = 0.95f;         // останавливающая сила
    public LayerMask layerEnemy;            // маска для нанесения урона
    public GameObject explEffect;           // эффект взрыва
    public ActiveWeapon activeWeapon;       // ссылка на скрипт (для звука)

    //public float force = 700f;

    float countdown;
    bool hasExploded = false;
    
    void Start()
    {
        countdown = delay;

        activeWeapon = GameManager.instance.player.GetComponent<ActiveWeapon>();

/*        explEffect.transform.position = transform.position;
        explEffect.transform.forward = transform.forward;
        explEffect.Emit(1);*/
    }

    
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        //Debug.Log("BOOM!");



        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerEnemy);
        foreach (Collider nearbyObject in colliders)
        {
            activeWeapon.audioSourses.explosion.Play();                         // хлопок 

            if (nearbyObject == null)
            {
                continue;
            }

            if (nearbyObject.tag == "Enemy")
            {
                Enemy_old enemy = nearbyObject.GetComponentInParent<Enemy_old>();
                if (enemy)
                {
                    Damage dmg = new Damage()
                    {
                        damageAmount = damage,
                        origin = transform.position,
                        pushForce = pushForce
                    };
                    enemy.TakeHitAxeBlood();
                    enemy.SendMessage("ReceiveDamage", dmg);
                }

                NPC npc = nearbyObject.GetComponentInParent<NPC>();
                if (npc)
                {
                    Damage dmg = new Damage()
                    {
                        damageAmount = damage,
                        origin = transform.position,
                        pushForce = pushForce
                    };
                    npc.TakeHitAxeBlood();
                    npc.SendMessage("ReceiveDamage", dmg);
                }
            }

            if (nearbyObject.tag == "Fighter")
            {
                Player player = nearbyObject.GetComponentInParent<Player>();
                Damage dmg = new Damage()
                {
                    damageAmount = 52,
                    origin = transform.position,
                    pushForce = pushForce
                };
                //enemy.TakeHitAxeBlood();
                player.SendMessage("ReceiveDamage", dmg);
            }

            colliders = null;


/*            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }*/
        }

        Collider[] collidersBig = Physics.OverlapSphere(transform.position, radiusBig, layerEnemy);
        foreach (Collider nearbyObject in collidersBig)
        {
            if (nearbyObject == null)
            {
                continue;
            }

            if (nearbyObject.tag == "Enemy")
            {
                Enemy_old enemy = nearbyObject.GetComponentInParent<Enemy_old>();
                if (enemy)
                {
                    Damage dmg = new Damage()
                    {
                        damageAmount = damageBigRadius,
                        origin = transform.position,
                        pushForce = pushForce
                    };
                    enemy.TakeHitAxeBlood();
                    enemy.SendMessage("ReceiveDamage", dmg);
                }

                NPC npc = nearbyObject.GetComponentInParent<NPC>();
                if (npc)
                {
                    Damage dmg = new Damage()
                    {
                        damageAmount = damage,
                        origin = transform.position,
                        pushForce = pushForce
                    };
                    npc.TakeHitAxeBlood();
                    npc.SendMessage("ReceiveDamage", dmg);
                }
            }

            if (nearbyObject.tag == "Fighter")
            {
                Player player = nearbyObject.GetComponentInParent<Player>();
                Damage dmg = new Damage()
                {
                    damageAmount = 26,
                    origin = transform.position,
                    pushForce = pushForce
                };
                //enemy.TakeHitAxeBlood();
                player.SendMessage("ReceiveDamage", dmg);
            }

            colliders = null;
        }

        Instantiate(explEffect, transform.position, transform.rotation);
/*        explEffect.transform.position = transform.position;
        explEffect.transform.forward = transform.forward;
        explEffect.Emit(2);*/

        Destroy(gameObject);



    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusBig);
    }
}



