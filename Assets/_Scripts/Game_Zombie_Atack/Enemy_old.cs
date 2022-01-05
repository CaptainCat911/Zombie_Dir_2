using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_old : Mover
{

    // Logic
    public float triggerLenght = 5;   // радиус приследования (в пределах игрок)
    public float chaseLenght = 10;   // радиус приследования (вся зона)
    public float findPlayerRange = 1;  // на каком расстоянии остановится перед целью
    public float faceingTargetSpeed = 5f;   // скорость поворота возле цели
    public float timeAfterDeath = 10f;
    private bool chasing;
    //private bool returning = false;
    private bool collidingWithPlayer;

    private Transform playerTransform;
    private Vector3 startingPosition;   
    public LayerMask layerPlayer;
    

    public Animator anim;
    public NavMeshAgent agent;    
    public EnemyHitbox hitbox;      // ссылка на хитбокс
    public CapsuleCollider capsuleCollider;
    public CapsuleCollider capsuleColliderLeftARm;
    public CapsuleCollider capsuleColliderRightArm;
    //CapsuleCollider[] allCapsCol;
    private Enemy_old selfScript;


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
                    chasing = true;

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
                            agent.SetDestination(playerTransform.position);
                        
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


        //---------------------------- Проверяем столкновение с игроком ----------------------------\\

        collidingWithPlayer = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, findPlayerRange, layerPlayer);
        foreach (Collider enObject in colliders)
        {            
            if (enObject == null)
            {                
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

        if (agent.velocity.magnitude > 0.05)
            anim.SetBool("Run", true);
        else
            anim.SetBool("Run", false);

        //-------------------ПушДирекшн---------------------\\

        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);
        transform.Translate(pushDirection * Time.deltaTime, Space.World);      
    }


//---------------------------------------------------------------------------------------------------------------------------------------------------------\\
    void FaceTarget()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * faceingTargetSpeed);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\

    protected override void Death()
    {
        anim.SetTrigger("Death");
        capsuleCollider.enabled = false;       
        capsuleColliderLeftARm.enabled = false;
        capsuleColliderRightArm.enabled = false;
        selfScript.enabled = false;
        agent.ResetPath();
        Invoke("NavMeshDisable", 1);
        Destroy(gameObject, timeAfterDeath);        

        // Добавить шанс выпадение предмета
    }

    public void NavMeshDisable()
    {
        agent.enabled = false;
    }
}