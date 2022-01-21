using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Collidable
{  
    // Damage
    public int damage = 1;  
    public float pushForce = 2;
    public float cooldown = 0.5f;
    public float attackSpeed = 1;
    //public float hitBoxDelay = 1;  // задержка перед атакой (для совпадения с анимацией), переделать на ивент анимации     
    public float attackRadiusHitBox = 1;    // радиус хитбокса

    private float lastSwing;
    private float lastGrab;
    public bool grabChardge = true;
    public bool attacking = false;
    public bool grabReady = false;

    public Enemy_old enemy;

    public WeaponAnimationEvents animationEvents;

    Player player;


    protected override void Start()
    {
        base.Start();

        player = GameManager.instance.player;

        enemy = GetComponentInParent<Enemy_old>();

        animationEvents.ZombieAnimationEvent.AddListener(EventsZombieAttack);     // получаем ивенты от анимации атаки              
        animationEvents.ZombieAnimationGrabEvent.AddListener(EventsZombieGrab);     // получаем ивенты от анимации захвата                
    }


    public void LateUpdate()
    {
        //Debug.Log("Ready" + grabReady);
        //Debug.Log("Chardge" + grabChardge);
        if (grabReady && grabChardge)
        {
            lastGrab = Time.time;
            //Debug.Log("GrabUpdate !");
            Grab();
        }
    }


    public void Attack()
    {
        if (Time.time - lastSwing > cooldown)
        {
            attacking = true;
            lastSwing = Time.time;
            //StartCoroutine(AttackWithDelay(hitBoxDelay));
            enemy.anim.SetFloat("AttackSpeed", attackSpeed);
            enemy.anim.SetTrigger("Swing");
        }
    }

    public void Grab()      // захват зомби 
    {
        //Debug.Log("GrabF !");
        enemy.anim.SetTrigger("Grab");
        StartCoroutine(GrabCor());
        grabChardge = false;        // заряд захвата зомби
    }


    IEnumerator GrabCor()       // ускорение при захвате зомби 
    {
        enemy.agent.speed = 3;
        //do
        //{
            //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1f);
        //}
        //while (enemy.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        enemy.agent.speed = 0.5f;


        //yield return new WaitForSeconds(delay_1);

    }


    void EventsZombieAttack(string eventName)       // по ивенту атакукем
    {
        //Debug.Log(eventName);
        switch (eventName)
        {
            case "hit":
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
                        if (player)
                        {
                            StartCoroutine(GrabPlayerWalk());
                        }

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
                attacking = false;
                break;
        }
    }


    void EventsZombieGrab(string eventName)     // по ивенту делаем завхат
    {
        switch (eventName)
        {
            case "grab":
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
                        if (player)
                        {
                            StartCoroutine(GrabPlayerWalk());                            
                        }

                        Damage dmg = new Damage()
                        {
                            damageAmount = 7,
                            origin = transform.position,
                            pushForce = pushForce
                        };
                        enObjectBox.SendMessage("ReceiveDamage", dmg);
                    }
                    collidersHitbox = null;
                }                
                break;
        }
    }

    IEnumerator GrabPlayerWalk()
    {
        player.walking = true;
        yield return new WaitForSeconds(2f);
        player.walking = false;
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
