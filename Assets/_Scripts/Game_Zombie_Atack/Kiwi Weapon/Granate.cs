using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granate : MonoBehaviour
{
    public float delay = 3f;                // задержка перед взрывом
    public float radius = 4f;               // радиус взрыва
    public float radiusBig = 8f;            // радиус дополнительного взрыва
    public int damage = 300;                // основной урон
    public int damageBig = 76;              // дополнительный урон
    public float pushForce = 0.95f;         // останавливающая сила
    public LayerMask layerEnemy;            // маска для нанесения урона
    public ParticleSystem explEffect;       // эффект взрыва

    //public float force = 700f;

    float countdown;
    bool hasExploded = false;
    
    void Start()
    {
        countdown = delay;

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
            if (nearbyObject == null)
            {
                continue;
            }

            if (nearbyObject.tag == "Enemy")
            {
                Enemy_old enemy = nearbyObject.GetComponentInParent<Enemy_old>();
                Damage dmg = new Damage()
                {
                    damageAmount = damage,
                    origin = transform.position,
                    pushForce = pushForce
                };
                enemy.TakeHitAxeBlood();
                enemy.SendMessage("ReceiveDamage", dmg);
            }

            if (nearbyObject.tag == "Fighter")
            {
                Player player = nearbyObject.GetComponentInParent<Player>();
                Damage dmg = new Damage()
                {
                    damageAmount = damage,
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
                Damage dmg = new Damage()
                {
                    damageAmount = damageBig,
                    origin = transform.position,
                    pushForce = pushForce
                };
                enemy.TakeHitAxeBlood();
                enemy.SendMessage("ReceiveDamage", dmg);
            }

            if (nearbyObject.tag == "Fighter")
            {
                Player player = nearbyObject.GetComponentInParent<Player>();
                Damage dmg = new Damage()
                {
                    damageAmount = damageBig,
                    origin = transform.position,
                    pushForce = pushForce
                };
                //enemy.TakeHitAxeBlood();
                player.SendMessage("ReceiveDamage", dmg);
            }

            colliders = null;
        }

        explEffect.transform.position = transform.position;
        //explEffect.transform.forward = transform.forward;
        explEffect.Emit(1);

        Destroy(gameObject, 2);



    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusBig);
    }
}



