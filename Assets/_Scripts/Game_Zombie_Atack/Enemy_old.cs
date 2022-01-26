using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_old : Mover
{
    // Logic
    public float triggerLenght = 5f;   // радиус тригера преследования (в пределах игрок)
    public float chaseLenght = 10f;   // радиус преследования (вся зона)
    public float findPlayerRange = 1f;  // на каком расстоянии остановится перед целью
    public float grabPlayerRange = 2f;  // на каком расстоянии начать хватать
    public float faceingTargetSpeed = 5f;   // скорость поворота возле цели
    public float timeAfterDeath = 10f;      // сколько лежит труп
    public bool weak = false;               // слабый зомби 
    public bool strong = false;             // сильный зомби
    public bool test = false;               // режим тестового зомби
    public bool biting = false;

    public float cooldownSlow = 0.5f;     // кулдаун замедления
    public float lastSlow;              // для замедления
    public bool slowed = false;

    private float maxSpeed = 4f;        // максимальная скорость (не нужна наверное)
    //private float speed;
    private bool chasing;           // преследование
    //private bool returning = false;
    private bool collidingWithPlayer;   // столкновение с игроком

    private Transform playerTransform;  // ссылка на трансформ игрока
    private Vector3 startingPosition;   // стартовая позиция
    public LayerMask layerPlayer;       // маска для игрока

    public Animator anim;
    public NavMeshAgent agent;
    public EnemyHitbox hitbox;      // ссылка на хитбокс
    public CapsuleCollider capsuleCollider;         // коллайдеры для попадания
    public CapsuleCollider capsuleColliderLeftARm;  // коллайдеры рук
    public CapsuleCollider capsuleColliderRightArm; //
    //CapsuleCollider[] allCapsCol;
    private Enemy_old selfScript;    // ссылка на свой скрипт (вроде можно убрать)

    public GameObject tempCapColl;      // временный коллайдер для жрущих зомби

    public float tempAgentSpeed = 6;    // скорость к которой вернуться после замедления 
    float stopForce = 0;                // сила замедления

    


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    protected override void Start()
    {
        base.Start();        

        //hitbox = transform.GetChild(18).GetComponent<BoxCollider>();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;        
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        hitbox = GetComponentInChildren<EnemyHitbox>();
        //capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        selfScript = GetComponent<Enemy_old>();
        if (test)
            return;

        int random = Random.Range(0, 100);          // разные типы зомби
        if (random <= 79)
        {
            weak = true;
            int random3 = Random.Range(0, 3);
            if (random3 == 0)
                hitbox.grabChardge = false;
            if (random3 == 1 || random3 == 2)
                hitbox.grabChardge = true;

            hitbox.cooldown = 2.5f;
            hitbox.attackSpeed = 1.3f;
            agent.speed = 0.5f;
            int random2 = Random.Range(1,4);
            //Debug.Log(random2);
            if (random2 == 1)            
                anim.SetFloat("Walk_number", 0);
            if (random2 == 2)
                anim.SetFloat("Walk_number", 0.5f);
            if (random2 == 3)
                anim.SetFloat("Walk_number", 1);
            tempCapColl.SetActive(false);
        }

        if (random >= 80 && random < 90)
        {
            int random3 = Random.Range(0, 3);
            if (random3 == 0 || random3 == 2)
                hitbox.grabChardge = false;
            if (random3 == 1)
                hitbox.grabChardge = true;

            hitbox.cooldown = 2f;
            hitbox.attackSpeed = 1.6f;
            agent.speed = 2f;
            maxHealth = 150;
            currentHealth = maxHealth;
            tempCapColl.SetActive(false);
        }

        if (random >= 90)
        {
            strong = true;
            hitbox.grabChardge = false;
            hitbox.cooldown = 1.5f;
            hitbox.attackSpeed = 2f;
            agent.speed = 6f;
            maxHealth = 250;
            currentHealth = maxHealth;            
            triggerLenght = 6;
            anim.SetTrigger("Biting");
            biting = true;
        }

        tempAgentSpeed = agent.speed;
    }







    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            
        }
    }




    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\




    private void FixedUpdate()
    {
        // Is the player in range?
        if (playerTransform == null)
        {            
            agent.ResetPath();
        }

        else
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLenght)
            {
                if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLenght)
                {
                    
                    chasing = true;
                    if (strong && biting)
                    {                       
                        anim.SetTrigger("Stop_biting");
                        FaceTarget();
                        if (currentHealth == maxHealth)
                        {
                            StartCoroutine(ScreamDelay());
                            biting = false;
                            tempCapColl.SetActive(false);
                        }
                        if (currentHealth != maxHealth)
                        {                            
                            biting = false;
                            tempCapColl.SetActive(false);
                            anim.SetTrigger("Bite_Go");
                        }

                    }
                }
                

                if (chasing == true)
                {
                    if (collidingWithPlayer == false)
                    {
                        if (hitbox.attacking == true)
                        {
                            agent.ResetPath();
                            FaceTarget();
                        }

                        else
                        {
                            agent.SetDestination(playerTransform.position);
                        }
                        
                    }
                    else
                    {                                                   
                        agent.ResetPath();
                        FaceTarget();
                        hitbox.Attack();
                    }
                }

                else
                {

                }
            }

            else
            {
                agent.SetDestination(startingPosition);
                chasing = false;
            }
        }


        if (currentHealth < maxHealth)
        {
            triggerLenght = 1000;
        }


        //---------------------------- Проверяем столкновение с игроком ----------------------------\\
        
        collidingWithPlayer = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, findPlayerRange, layerPlayer);
        
        foreach (Collider enObject in colliders)
        {
            //Debug.Log(enObject);
            if (enObject == null)                   // НЕ РАБОТАЕТ !
            {
                Debug.Log("NULL");
                collidingWithPlayer = false;
                continue;
            }      

            if (enObject.tag == "Fighter")
            {
                collidingWithPlayer = true;
            }
            colliders = null;
        }
        
        
        Collider[] collidersToGrab = Physics.OverlapSphere(transform.position, grabPlayerRange, layerPlayer);
        foreach (Collider enObject in collidersToGrab)
        {
            if (enObject == null)
            {
                hitbox.grabReady = false;
                hitbox.grabChardge = true;
                continue;
            }

            if (enObject.tag == "Fighter")
            {
                hitbox.grabReady = true;
                //Debug.Log("GrabTrue !");
            }
            colliders = null;
        }
        //Debug.Log(collidingWithPlayer);


        //------------------ Смотреть в сторону игрока ------------------------\\

        if (chasing == true)
        {
                                //LookAtPlayer(); не нужен тут
        }

        /*        else
                {
                    Vector3 lookDir = startingPosition - transform.position;
                    float angle = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, angle, 0);
                }*/

        //if (agent.velocity.magnitude > 0.05)
        //anim.SetBool("Run", true);
        //else
        //anim.SetBool("Run", false);
        anim.SetFloat("Speed", agent.velocity.magnitude / maxSpeed);

        //Debug.Log(agent.velocity.magnitude);

        //-------------------ПушДирекшн---------------------\\

        //pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);
        //transform.Translate(pushDirection * Time.deltaTime, Space.World);



        
        // Перезарядка замедления
        if (Time.time - lastSlow > cooldownSlow)
        {
            slowed = false;
        }

        if (slowed)         
        {
            agent.speed *= stopForce;           // скорость замедления 
        }

        if (!slowed)        
        {
            if (agent.speed < tempAgentSpeed)
            {
                agent.speed *= 1.01f;           // скорость восстановления после замедления
            }            
            //agent.speed = tempAgentSpeed;
        }

        if (agent.speed < 0.2f && !test)        
        {
            agent.speed = 0.2f;            // минимальная скорость
        }        
    }



    protected override void ReceiveDamage(Damage dmg)
    {
        base.ReceiveDamage(dmg);
        stopForce = dmg.pushForce;
        TakeHit();
    }



    public void TakeHit()
    {
        anim.SetTrigger("Take_Hit");
        lastSlow = Time.time;
        slowed = true;
    }



    IEnumerator ScreamDelay()
    {
        float tempSpeed = agent.speed;
        agent.speed = 0;           
        yield return new WaitForSeconds(2f);       
        agent.speed = tempSpeed;
        anim.SetTrigger("Bite_Go");
    }






    void FaceTarget()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * faceingTargetSpeed);
    }







    protected override void Death()
    {
        GameManager.instance.enemyCount -= 1;
        int random = Random.Range(0, 2);
        if (random == 1)
            anim.SetFloat("Death_type", 1);
        anim.SetTrigger("Death");
        capsuleCollider.enabled = false;       
        capsuleColliderLeftARm.enabled = false;
        capsuleColliderRightArm.enabled = false;
        selfScript.enabled = false;
        agent.ResetPath();
        Invoke("NavMeshDisable", 2);
        Destroy(gameObject, timeAfterDeath);        

        // Добавить шанс выпадение предмета
    }

    public void NavMeshDisable()
    {
        agent.enabled = false;
    }

    public void Kill()
    {
        GameManager.instance.enemyCount -= 1;
        Destroy(gameObject);
    }
}