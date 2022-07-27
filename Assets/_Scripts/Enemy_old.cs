using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Enemy_old : Mover
{
    public int damage = 6;
    // Logic
    public float triggerLenght = 10f;       // радиус тригера преследования (в пределах игрок)
    //public float chaseLenght = 10f;       // радиус преследования (вся зона)
    public float findPlayerRange = 1f;      // на каком расстоянии остановится перед целью
    public float grabPlayerRange = 2f;      // на каком расстоянии начать хватать
    public float faceingTargetSpeed = 5f;   // скорость поворота возле цели
    public float timeAfterDeath = 10f;      // сколько лежит труп

    public bool weakZombie = false;         // слабый зомби 
    public bool runZombie = false;          // бегущий зомби 
    public bool strongZombie = false;       // сильный зомби
    public bool agony = true;               // сильный зомби безумен
    public bool darkZombie;                 // темный (усилиненный) зомби

    public bool simpleZombie = false;       // обычный зомби (пока нигде не использую)
    public bool mediumZombie = false;       // средний зомби (пока нигде не использую)
    public bool agonyZombie = false;        // зомби в агонии

    public bool test = false;               // режим тестового зомби
    public bool dontCount = false;          // если не нужно считать убитым
    public bool biting = false;             // жрёт

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
    public CapsuleCollider capsuleColliderLeftARm;  // коллайдеры рук
    public CapsuleCollider capsuleColliderRightArm; 
    //CapsuleCollider[] allCapsCol;
    private Enemy_old selfScript;           // ссылка на свой скрипт (вроде можно убрать)
    public GameObject tempCapColl;          // временный коллайдер для жрущих зомби (пока что отключил)

    public float tempAgentSpeed = 6;        // скорость к которой вернуться после замедления 
    float stopForce = 0;                    // сила замедления

    public ParticleSystem hitEffectBlood;   // кровь для финала
    public GameObject chest;                // для крови
    public GameObject[] ammos;              // выпадение патронов, массив
    public GameObject medHP;                // выпадение аптечки

    public GameObject ammoBack;             // патроны за спиной
    public GameObject medhpBack;            // аптечка за спиной
    private int randomAmmo;                 // для вероятности выпадения патронов и аптечек
    int ndx;                                // индекс для выбора типа патронов
    public int ammoChanse = 98;             // шанс выпадения патронов

    public bool dead = false;               // если убили

    [HideInInspector]
    public AudioSourses audioSourses;       // ссылка на объект с аудиоисточниками
    bool audioPlayingIdle;                  // для остановки idle звука
    public float audioPitch;                // установка питч звука

    public GameObject mapIcon;              // иконка для карты 
    bool direction;                         // для рандомного направления зомби
    public GameObject darkEffect;           // эффект тьмы
    public bool granateInRange;             // для следования за гранатой (пока не используется)

    public VisualEffect effectSmoke;        // эффект дыма
    float xSize = 1.7f;                     // размер дыма
    public bool effectDecrise;              // для уменьшения дыма

    //public SphereCollider weaponPickUpCollider; // ccылка на колайдер оружия


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
        //currentHealth = maxHealth;
        if (test)
            return;


        if (GameManager.instance.survZombie)        // если зомби для режима выживания
        {
            triggerLenght = 1000;
        }
        
        int random = Random.Range(0, 100);          // разные типы зомби
        random = 0;

        if ((random <= 84 || weakZombie) && !strongZombie && !runZombie)          // шанс на слабого зомби или сами устанавливаем слабого зомби
        {

            simpleZombie = true;
            int random3 = Random.Range(0, 2);       // 50% шанс на то, что зомби будет с захватом
            if (random3 == 0)
                hitbox.grabChardge = false;
            if (random3 == 1)                       // || random3 == 2
                hitbox.grabChardge = true;

            hitbox.damage = damage;                 // урон
            hitbox.cooldown = 2.5f;                 // кд атаки
            hitbox.attackSpeed = 1.3f;              // скорость атаки
            
            if (darkZombie)
            {
                hitbox.damage *= 2;
                maxHealth *= 8;
                cooldownSlow = 0;
                darkEffect.SetActive(true);
            }

            currentHealth = maxHealth;

            agent.speed = 0.5f;                     // скорость передвижения
            int random2 = Random.Range(1,4);        // случайный выбор типа передвижения зомби
            //Debug.Log(random2);
            if (random2 == 1)            
                anim.SetFloat("Walk_number", 0);
            if (random2 == 2)
                anim.SetFloat("Walk_number", 0.5f);
            if (random2 == 3)
                anim.SetFloat("Walk_number", 1);
            //tempCapColl.SetActive(false);           // временный коллайдер для жрущих зомби (отключаем)
        }

        else if ((random >= 85 && random < 90) && !strongZombie && !weakZombie || runZombie)
        {
            int random3 = Random.Range(0, 3);
            if (random3 == 0 || random3 == 2)
                hitbox.grabChardge = false;
            if (random3 == 1)
                hitbox.grabChardge = true;

            hitbox.damage = damage + 2;
            hitbox.cooldown = 2f;
            hitbox.attackSpeed = 1.6f;
            agent.speed = 2f;
            maxHealth = maxHealth + 40;
            if (darkZombie)
            {
                hitbox.damage *= 2;
                maxHealth *= 8;
                cooldownSlow = 0.1f;
                darkEffect.SetActive(true);
            }
            currentHealth = maxHealth;
            //tempCapColl.SetActive(false);
        }

        else if ((random >= 90 || strongZombie) && !GameManager.instance.final)
        {
            agonyZombie = true;
            hitbox.grabChardge = false;
            hitbox.damage = damage + 4;
            hitbox.cooldown = 1.5f;
            hitbox.attackSpeed = 2f;
            //int randomSpeedStrong = Random.Range(1, 8);
            agent.speed = 5f;
            maxHealth = maxHealth + 120;
            if (darkZombie)
            {
                hitbox.damage *= 2;
                maxHealth *= 6;
                cooldownSlow = 0.2f;
                darkEffect.SetActive(true);
            }
            currentHealth = maxHealth;            
            if (agony)                                      // они агонируют?
            {
                triggerLenght = 6;
                anim.SetTrigger("Agony");                   // агония
                biting = true;

                int randomAgony = Random.Range(0, 2);       // меняем тип агонии (анимацию)
                if (randomAgony == 0)
                    anim.SetFloat("Agony_type", 0);
                if (randomAgony == 1)
                    anim.SetFloat("Agony_type", 1);
                //tempCapColl.SetActive(true);
            }
        }        

        tempAgentSpeed = agent.speed;

        // выпадение патронов
        randomAmmo = Random.Range(0, 100);
        if (randomAmmo >= ammoChanse)
        {
            ammoBack.SetActive(true);
        }
        if (randomAmmo == 0)                                // шанс выпадения аптечки 1% 
        {
            if (Random.Range(0, 2) > 0)                     // шанс выпадения аптечки 0.5% 
            {
                medhpBack.SetActive(true);
            }
        }

        tempCapColl.SetActive(false);          
    }



    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Death();
        }


        if (effectDecrise)
        {            
            xSize -= 0.04f;
            effectSmoke.SetFloat("EffectSize", xSize);
            if (xSize < 0.1f)
            {
                effectDecrise = false;
                darkEffect.SetActive(false);
            }
        }

        if (currentHealth < 50)
        {
            hitbox.grabChardge = false;
        }
    }




    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\




    private void FixedUpdate()
    {
        if (dead)
            return;

        // Is the player in range?
        if (playerTransform == null)
        {            
            agent.ResetPath();
        }

        else
        {
            if (Vector3.Distance(playerTransform.position, transform.position) < triggerLenght)       // если дистанция до игрока < тригер дистанции
            {                    
                chasing = true;                                                              // преследование включено 
                direction = false;
                if (biting && GameManager.instance.player.isAlive)                           // для безумных зомби
                {                       
                    anim.SetTrigger("Stop_biting");                                          // если игрок в тригере - останавливаем безумие
                    FaceTarget();
                    if (currentHealth == maxHealth)
                    {
                        biting = false;
                        StartCoroutine(ScreamDelay());                                      // запускаем анимацию крика
                        //tempCapColl.SetActive(false);
                    }
                    if (currentHealth != maxHealth)
                    {                                                        
                        //tempCapColl.SetActive(false);
                        anim.SetTrigger("Bite_Go");                                         // если получили урон - сразу преследование игрока
                    }

                }
            }

/*                else
            {
                    agent.SetDestination(new Vector3(0,0,0));
            }*/


            if (chasing)                                // если преследуем
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
                        hitbox.Attack();                                                            // АТАКУЕМ
                        audioSourses.idle.Stop();
                        audioPlayingIdle = false;
                    }
                        
                    if (!GameManager.instance.player.isAlive)
                        StartCoroutine(BitingDelay());                            
                }
            }

            // зомби идут в рандомном направлении
            if (!chasing && !direction && !agony && GameManager.instance.zombieFreeWalk)          
            {
                //agent.SetDestination(startingPosition);
                agent.SetDestination(transform.position + new Vector3 (Random.Range(-100,100), 0, Random.Range(-100, 100)));
                //chasing = false;
                direction = true;
            }
        }

        if (chasing && !audioPlayingIdle && !dead)
        {
            
            audioSourses.idle.Play();
            audioPlayingIdle = true;
            //Debug.Log("Idle playing");
        }

        if (currentHealth < maxHealth)                                  // если получили урон - преследуем
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


    public void SetDestinationZombie(Vector3 position)
    {
        agent.SetDestination(position);
    }

    IEnumerator BitingDelay()
    {
        yield return new WaitForSeconds(1);
        anim.SetTrigger("Biting");
    }


    protected override void ReceiveDamage(Damage dmg)
    {        
        base.ReceiveDamage(dmg);
        stopForce = dmg.stopForce;
        TakeHit();
    }



    public void TakeHit()
    {
        audioSourses.takeDamage.Play();
        if (!dead)
            anim.SetTrigger("Take_Hit");
        if (dead)
            anim.SetTrigger("Take_Hit_Dead");
        lastSlow = Time.time;
        slowed = true;
    }

    public void TakeHitAxeBlood()
    {
        hitEffectBlood.transform.position = chest.transform.position;
        hitEffectBlood.transform.forward = transform.forward;
        hitEffectBlood.Emit(1);
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
        if (dead)
            return;
        

        GameManager.instance.enemyKilledStatistic += 1;         // в счётчик убитых зомби для статистики
        GameManager.instance.enemyKilledCount += 1;             // в счётчик убитых зомби

        if (!dontCount)
            GameManager.instance.enemyCount -= 1;               // из счётчика зомби на карте

        GameManager.instance.player.ammoPack.souls += 1;        // + души
        if (darkZombie)
        {            
            effectDecrise = true;
            GameManager.instance.player.ammoPack.souls += 1;    // + ещё душа, если зомби тёмный
        }

        // выпадение патронов        
        if (randomAmmo >= ammoChanse)
        {            
            if (!GameManager.instance.questAmmo)                // если не начали квест
            {
                ndx = Random.Range(0, 2);                       // патроны только пистолет и ар 
            }

            if (GameManager.instance.questAmmo || GameManager.instance.enemyAllAmmo)    // есил взяли квест или все патроны у зомби
            {
                ndx = Random.Range(0, ammos.Length);            // все патроны
            }

            GameObject go = Instantiate(ammos[ndx]);        // Создаём префаб патронов
            //go.transform.SetParent(transform, false);     // Назначаем этот спавнер родителем
            go.transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
            ammoBack.SetActive(false);
        }

        if (randomAmmo == 0)                                // шанс выпадения аптечки 1% 
        {
            if (Random.Range(0, 2) > 0)                     // шанс выпадения аптечки 0.5% 
            {
                GameObject go = Instantiate(medHP);             // Создаём префаб аптечки
                //go.transform.SetParent(transform, false);     // Назначаем этот спавнер родителем
                go.transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
                medhpBack.SetActive(false);
            }
        }

        int random = Random.Range(0, 2);
        if (random == 1)
            anim.SetFloat("Death_type", 1);
        anim.SetTrigger("Death");
        //tempCapColl.SetActive(false);
       
        agent.ResetPath();
        Invoke("NavMeshDisable", 2);
        Destroy(gameObject, timeAfterDeath);

        
        audioSourses.idle.Stop();                   // отключаем шипение зомби        
        audioSourses.death.Play();                  // звук
        //darkEffect.SetActive(false);                // отключаем темный эффект
        mapIcon.SetActive(false);                   // убираем иконку
        //selfScript.enabled = false;

        //weaponPickUpCollider.enabled = true;

        capsuleCollider.enabled = false;
        capsuleColliderLeftARm.enabled = false;
        capsuleColliderRightArm.enabled = false;

        dead = true;
    }



    public void NavMeshDisable()
    {
        agent.enabled = false;
    }




    public void Kill()
    {
        if (!dontCount)
            GameManager.instance.enemyCount -= 1;        
        Destroy(gameObject);
    }




    public void DeathFinal()
    {
        if (!dontCount)
            GameManager.instance.enemyCount -= 1;

        hitEffectBlood.transform.position = chest.transform.position;
        hitEffectBlood.transform.forward = transform.forward;
        hitEffectBlood.Emit(1);

        int random = Random.Range(0, 2);
        if (random == 1)
            anim.SetFloat("Death_type", 1);
        anim.SetTrigger("Death");
        //tempCapColl.SetActive(false);
        capsuleCollider.enabled = false;
        capsuleColliderLeftARm.enabled = false;
        capsuleColliderRightArm.enabled = false;
        //darkEffect.SetActive(false);                // отключаем темный эффект
        mapIcon.SetActive(false);                   // убираем иконку
        agent.ResetPath();
        Invoke("NavMeshDisable", 2);
        audioSourses.idle.Stop();                   // отключаем шипение зомби        
        audioSourses.death.Play();                  // звук
        Destroy(gameObject, timeAfterDeath);
        selfScript.enabled = false;        
    }
}