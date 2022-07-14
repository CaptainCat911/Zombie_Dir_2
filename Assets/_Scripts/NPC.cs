using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

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
    public bool chasing;                   // преследование
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
    bool playerInRange;                     // игрок в ренже нпс
    public bool meatWithPlayer;             // встреча с игроком
    public bool meatWithPlayerDone;         // встретился с игроком

    public VisualEffect effectSmoke;        // эффект дыма
    float xSize = 1.7f;                     // размер дыма
    public GameObject closes;               // одежда
    bool effectIncrise;                     // увеличивать дым
    bool effectDecrise;                     // уменьшать дым

    private bool timerStart;                // таймер стартовал
    private bool timerGone;                 // таймер завершён
    private float timerStarted;             // начало отсчёта
    public float timerTimeDelay = 5;        // сколько ждать
    private bool magazineOpened;
    




    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    protected override void Start()
    {
        base.Start();

        //mapIcon.SetActive(true);

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
            if (timerGone)       // если волна не запущена и уровень остановлен    (!GameManager.instance.diffManager.waveStarted && !GameManager.instance.diffManager.levelStop && timerGone) ||
            {
                GameManager.instance.diffManager.start = true;              // стартуем волну
                GameManager.instance.diffManager.waveStarted = true;        // волна запущена
                timerGone = false;
            }

            if (Vector3.Distance(playerTransform.position, transform.position) < triggerLenght && !GameManager.instance.diffManager.testDiff)     // если дистанция до игрока < тригер дистанции
            {
                //chasing = true;                                                                   // преследование включено 
                playerInRange = true;                       // игрок в ренже
                if (!meatWithPlayerDone)                    // встреча с игроком
                {
                    meatWithPlayer = true;                  // встретили игрока
                    meatWithPlayerDone = true;              // встреча произошла
                    GameManager.instance.dialogueTrig.TriggerDialogue(1);       // вызываем диалог торговца
                    GameManager.instance.PauseWithDelay();                      // пауза с задержкой
                }
                //direction = false;

            }
            else
            {
                playerInRange = false;
            }

            if (playerInRange && !GameManager.instance.diffManager.levelGo)     // если игрок в ренже и волна не идёт
            {
                FaceTarget();
            }
            else
            {
                //GameManager.instance.diffManager.start = false;
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
                        chasing = false;
                    }                               
                }
            }            
        }

        // Таймер
        if (timerStart && !magazineOpened)
        {            
            if (Time.time - timerStarted > timerTimeDelay)
            {
                timerStart = false;
                timerGone = true;
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
       
        anim.SetFloat("Speed", agent.velocity.magnitude / maxSpeed);        // установка параметра скорости в аниматоре



        // Эффект дыма
        if (effectIncrise)
        {
            xSize += 0.02f;
            effectSmoke.SetFloat("EffectSize", xSize);
            if (xSize > 6)
            {
                effectIncrise = false;
                closes.SetActive(false);
                effectDecrise = true;
            }
        }

        if (effectDecrise)
        {
            xSize -= 0.04f;
            effectSmoke.SetFloat("EffectSize", xSize);
            if (xSize < 0.1f)
            {
                effectDecrise = false;
            }
        }

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SetDestinationNPC(playerTransform.position, false);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            xSize += 0.1f;
            effectSmoke.SetFloat("EffectSize", xSize);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            xSize -= 0.1f;
            effectSmoke.SetFloat("EffectSize", xSize);
        }
    }


    public void SetDestinationNPC(Vector3 position, bool warp)
    {
        if (warp)
        {
            agent.Warp(position);
            effectSmoke.SetFloat("EffectSize", 1.7f);
            closes.SetActive(true);
        }
        else
        {
            agent.SetDestination(position);
            effectIncrise = true;
        }
    }   


    // Магазин
    public void OpenMagazine()
    {                
        if (!magazineOpen)
        {
            magazine.SetActive(true);
            magazineOpened = true;
            GameManager.instance.playerStop = true;

            //GameManager.instance.Pause();
        }
        if (magazineOpen)
        {
            CloseMagazine();

            //GameManager.instance.UnPause();
        }
        magazineOpen = !magazineOpen;
    }

    public void CloseMagazine()
    {        
        magazine.SetActive(false);
        magazineOpened = false;
        GameManager.instance.playerStop = false;
        timerStarted = Time.time;
        timerStart = true;
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