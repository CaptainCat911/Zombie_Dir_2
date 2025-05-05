using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Collidable
{
    // Damage    
    public int damage;                      // Урон
    private float pushForce = 0.95f;        // замедление
    public float cooldown = 0.5f;           // перезардяка атаки
    public float attackSpeed = 1;           // скорость атаки       
    public float attackRadiusHitBox = 1;    // радиус хитбокса
    float grabSpeed = 5f;                   // скорость передвижения при захвате

    private float lastSwing;                // время последнего удара (для перезарядки удара)
    private float lastGrab;                 // для перезарядки захвата
    public bool grabChardge = false;        // заряд захвата
    public bool attacking = false;          // состояние атаки (чтобы стоял на месте, когда бьет)
    public bool grabReady = false;          // готовность сделать захват (когда игрок в радиусе)

        // Ссылки
    public Enemy_old enemy;
    public WeaponAnimationEvents animationEvents;
    Player player;


    protected override void Start()
    {
        base.Start();

        player = GameManager.instance.player;
        enemy = GetComponentInParent<Enemy_old>();

        animationEvents.ZombieAnimationEvent.AddListener(EventsZombieAttack);       // получаем ивенты от анимации атаки              
        animationEvents.ZombieAnimationGrabEvent.AddListener(EventsZombieGrab);     // получаем ивенты от анимации захвата                
        //damage = enemy.damage;
    }


    public void LateUpdate()
    {
        //Debug.Log("Ready" + grabReady);
        //Debug.Log("Chardge" + grabChardge);
            // Перезарядка захвата (пока что не работает)
        if (grabReady && grabChardge)       // если готов делать захват и есть заряд
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
            enemy.anim.SetFloat("AttackSpeed", attackSpeed);    // ставим скорость атаки
            int random = Random.Range(0, 2);                    // меняем тип атаки (анимацию)
            if (random == 0)
                enemy.anim.SetFloat("Attack_type", 0);
            if (random == 1)
                enemy.anim.SetFloat("Attack_type", 1);
/*            if (random == 2)
                enemy.anim.SetFloat("Attack_type", 2);
            if (random == 3)
                enemy.anim.SetFloat("Attack_type", 3);*/
            enemy.anim.SetTrigger("Swing");                     // бьем

            enemy.audioSourses.attack.pitch = enemy.audioPitch;
            enemy.audioSourses.attack.Play();                   // звук атаки
        }
    }

    public void Grab()                          // захват зомби 
    {
        //Debug.Log("GrabF !");
        int random = Random.Range(0, 2);        // захват левой или правой рукой
        if (random == 1)
            enemy.anim.SetFloat("Grab_side", 1);
        enemy.anim.SetTrigger("Grab");
        StartCoroutine(GrabCor());
        grabChardge = false;                    // заряд захвата зомби
    }


    IEnumerator GrabCor()                       // ускорение при захвате зомби 
    {
        enemy.agent.speed = grabSpeed;
        //do
        //{
            //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1f);
        //}
        //while (enemy.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        enemy.agent.speed = 0.5f;       
    }


    void EventsZombieAttack(string eventName)       // по ивенту атакукем (ивент в анимации атаки)
    {
        //Debug.Log(eventName);
        switch (eventName)
        {
            case "hit":                             

                Collider[] collidersHitbox = Physics.OverlapSphere(this.transform.position, attackRadiusHitBox, enemy.layerPlayer);
                foreach (Collider enObjectBox in collidersHitbox)
                {                    
                    if (enObjectBox == null)
                    {
                        continue;
                    }

                    if (enObjectBox.tag == "Fighter")
                    {
                        if (player)
                        {

                            // тут звук удара по игроку

                            GrabPlayerWalk();           // замедление игрока
                        }

                        Damage dmg = new Damage()
                        {
                            damageAmount = damage,
                            origin = transform.position,
                            stopForce = pushForce
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
                            GrabPlayerWalk();         // замедление игрока                   
                        }

                        Damage dmg = new Damage()
                        {
                            damageAmount = 3,
                            origin = transform.position,
                            stopForce = pushForce
                        };
                        enObjectBox.SendMessage("ReceiveDamage", dmg);
                    }
                    collidersHitbox = null;
                }                
                break;
        }
    }

    public void GrabPlayerWalk()                // замедление игрока
    {
        player.lastSlow = Time.time;
        player.slowed = true;
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
