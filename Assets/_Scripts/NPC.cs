using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPC : Mover
{
    // Logic
    public float triggerLenght = 10f;       // радиус тригера преследования (в пределах игрок)
    //public float chaseLenght = 10f;       // радиус преследования (вся зона)
    public float findPlayerRange = 1f;      // на каком расстоянии остановится перед целью
    public float grabPlayerRange = 2f;      // на каком расстоянии начать хватать
    public float faceingTargetSpeed = 5f;   // скорость поворота возле цели
    public float timeAfterDeath = 10f;      // сколько лежит труп



    public float cooldownSlow = 0.5f;       // кулдаун замедления
    public float lastSlow;                  // для замедления
    public bool slowed = false;             // замедлен

    private float maxSpeed = 4f;            // максимальная скорость (не нужна наверное)
    //private float speed;
    private bool chasing;                   // преследование
    //private bool returning = false;
    private bool collidingWithPlayer;       // столкновение с игроком

    private Transform playerTransform;              // ссылка на трансформ игрока
    private Vector3 startingPosition;               // стартовая позиция
    public LayerMask layerPlayer;                   // маска для игрока

    public Animator anim;
    public NavMeshAgent agent;
    public EnemyHitbox hitbox;                      // ссылка на хитбокс
    public CapsuleCollider capsuleCollider;         // коллайдеры для попадания
    //public CapsuleCollider capsuleColliderLeftARm;  // коллайдеры рук
    //public CapsuleCollider capsuleColliderRightArm; 
    
    private Enemy_old selfScript;           // ссылка на свой скрипт (вроде можно убрать)

    public GameObject tempCapColl;          // временный коллайдер для жрущих зомби (пока что отключил)

    public float tempAgentSpeed = 6;        // скорость к которой вернуться после замедления     

    public ParticleSystem hitEffectBlood;   // кровь для финала
    public GameObject chest;                // для крови

    public bool dead = false;               // если убили

    [HideInInspector]
    public AudioSourses audioSourses;       // ссылка на объект с аудиоисточниками
    bool audioPlayingIdle;                  // для остановки idle звука
    public float audioPitch;                // установка питч звука

    public GameObject mapIcon;              // иконка для карты 

    public GameObject magazine;             // ссылка на магазин (UI)
    public bool magazineOpen;               // магазин открыт




    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    protected override void Start()
    {
        base.Start();

        mapIcon.SetActive(true);

        audioSourses = GetComponentInChildren<AudioSourses>();
        audioPitch = Random.Range(0.8f, 1.2f);
        audioSourses.death.pitch = audioPitch;
        audioSourses.idle.pitch = audioPitch;

        //hitbox = transform.GetChild(18).GetComponent<BoxCollider>();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;        
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        hitbox = GetComponentInChildren<EnemyHitbox>();
        //capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        selfScript = GetComponent<Enemy_old>();      
        tempAgentSpeed = agent.speed; 

    }




    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\




    private void FixedUpdate()
    {
        if (dead)
            return;

/*        if (playerTransform == null)
        {            
            agent.ResetPath();
        }*/

        // Is the player in range?
        else
        {           
            if (Vector3.Distance(playerTransform.position, transform.position) < triggerLenght)       // если дистанция до игрока < тригер дистанции
            {                    
                chasing = true;                                                                     // преследование включено 
                //direction = false;
               
            }

            if (chasing)                                                                            // если преследуем
            {
                if (collidingWithPlayer == false)                                                   // если не достаем до игрока 
                {
                    if (hitbox.attacking == true)                                                   // если идёт анимация атаки
                    {
                        agent.ResetPath();                                                          // стоим на месте
                        FaceTarget();
                    }
                                                            
                    else                                                                            // если анимация атаки не идёт 
                    {
                        agent.SetDestination(playerTransform.position);                             // следовать к игроку
                    }
                        
                }

                else                                                                                // если достаем до игрока
                {                                                   
                    agent.ResetPath();
                    FaceTarget();
                    if (GameManager.instance.player.isAlive)
                    {
                        //hitbox.Attack();                                                            // АТАКУЕМ                      
                        
                    }                               
                }
            }            
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

        //Debug.Log(collidingWithPlayer);
       
        anim.SetFloat("Speed", agent.velocity.magnitude / maxSpeed);
    }


    public void SetDestinationNPC(Transform transform)
    {
        agent.SetDestination(transform.position);
    }


    // Магазин
    public void OpenMagazine()
    {        
        if (!magazineOpen)
        {
            magazine.SetActive(true);
            GameManager.instance.Pause();
        }
        if (magazineOpen)
        {
            magazine.SetActive(false);
            GameManager.instance.UnPause();
        }
        magazineOpen = !magazineOpen;
    }


    protected override void ReceiveDamage(Damage dmg)
    {        
        base.ReceiveDamage(dmg);        
        //TakeHit();
    }


    public void TakeHit()
    {
        audioSourses.takeDamage.Play();
        if (!dead)
            anim.SetTrigger("Take_Hit");        
        //lastSlow = Time.time;
        //slowed = true;
    }

    public void TakeHitAxeBlood()
    {
        hitEffectBlood.transform.position = chest.transform.position;
        hitEffectBlood.transform.forward = transform.forward;
        hitEffectBlood.Emit(1);
    }






    void FaceTarget()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * faceingTargetSpeed);
    }



    protected override void Death()
    {
        if (dead)
            return;   

                
        int random = Random.Range(0, 2);
        if (random == 1)
            anim.SetFloat("Death_type", 1);
        anim.SetTrigger("Death");
        //tempCapColl.SetActive(false);
       
        agent.ResetPath();
        Invoke("NavMeshDisable", 2);
        Destroy(gameObject, timeAfterDeath);

        audioSourses.idle.Stop();
        
        audioSourses.death.Play();                  // звук

        mapIcon.SetActive(false);                   // убираем иконку

        //selfScript.enabled = false;        

        capsuleCollider.enabled = false;
        //capsuleColliderLeftARm.enabled = false;
        //capsuleColliderRightArm.enabled = false;

        dead = true;
    }


    public void NavMeshDisable()
    {
        agent.enabled = false;
    }
}