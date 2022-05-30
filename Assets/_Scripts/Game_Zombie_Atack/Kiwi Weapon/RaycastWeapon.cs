using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RaycastWeapon : MonoBehaviour
{

/*    public Rig animLayer;
    float animDuration = 0.3f;
*/
    
    [Header("Weapon Settings")]     // ??? Почему-то не работает

        // Rig
    public string weaponName; // название оружия для анимаций 

    // Damage struct
    public string textNameWeapon;                   // название оружия для GUI
    public ActiveWeapon.WeaponSlot weaponSlot;  // внутри перечисление слотов (основное, вторичное оружия) и выбирается в инспекторе префаба
    public string ammoType;     // тип патронов, указать в инспекторе (потом поменять на список)
    public int indexNumberWeapon;   // Индекс оружия для переключения между оружиями (Потом поменять на список или использовать weaponSlot[index])

    public float cooldown = 0.1f;  // 600 выстрелов в секунду 
    //public float bulletSpeed = 30f;     // скорость пули (не используется)
    public int rayDamage = 1;   // урон рейкаста
    public float pushForce; 

    private float lastSwing;        // для кд

        // Перезарядка
    public int allAmmo;     // всего патронов
    public int clipSize;    // размер магазина 
    public int ammoCount;   // счетчик в магазине

        // Ссылки 
    Player player;          // ссылка на игрока
    AmmoPack playerAmmo;    // ссылка на патроны игрока
    ActiveWeapon activeWeapon;
    //public GameObject projPrefab;   // префаб снаряда
    public Transform PROJECTILE_ANCHOR;   // якорь для оружия
    public Transform EFFECT_ANCHOR;   // якорь для оружия
    //public GameObject magazine;     // магазин

        // Эффекты
    public ParticleSystem[] muzzleFlash;    // вспышка оружия
    public ParticleSystem hitEffect;        // попадание в стену
    public ParticleSystem hitEffectBlood;   // попадание в зомби
    public TrailRenderer tracerEffect;      // трасер
    public Light lightEffect;               // вспышка света

    public LayerMask layerIgnore;           // маска 

    RaycastHit hit;
    Ray ray;
    Quaternion qua1;
    Quaternion qua2;

    public RaycastHit[] m_Results = new RaycastHit[3];      // для прострелов 

    AudioSource audioSource;                              // аудио источник

    public float recoilX;                                   // разброс по X
    public float recoilY;                                   // разброс по Y

    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\

    public void Start()
    {
        player = GameManager.instance.player;
        playerAmmo = GameManager.instance.player.GetComponent<AmmoPack>();        
        TakeAmmo();                                                            // Берем патроны из АммоПак игрока
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        layerIgnore = ~layerIgnore;

        audioSource = GetComponent<AudioSource>();
    }


    public void Update()
    {
        //Debug.Log(PROJECTILE_ANCHOR.forward);
    }


    public void TakeAmmo()
    {
        switch (ammoType)
        {
            case "9":
                allAmmo = playerAmmo.allAmmo_9;
                break;
            case "0.357":
                allAmmo = playerAmmo.allAmmo_0_357;
                break;
            case "5.56":
                allAmmo = playerAmmo.allAmmo_5_56;
                break;
            case "0.12":
                allAmmo = playerAmmo.allAmmo_0_12;
                break;
            case "7.62":
                allAmmo = playerAmmo.allAmmo_7_62;
                break;
            case "0.50":
                allAmmo = playerAmmo.allAmmo_0_50;
                break;
        }
    }





    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    public void UpdateWeapon()
    {      
        if (activeWeapon.attackActive && Time.time - lastSwing > cooldown)
        {
            //startAttack = true;

            AttackRange();
        }
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\




    public void AttackRange()
    {
        if (ammoCount <= 0)         // Если нет патронов
        {
            return;
        }

        ammoCount--;    // - патрон за каждый выстрел

        lastSwing = Time.time;

        //switch (type)
        //{
        //case WeaponType.blaster:
        //p = MakeProjectile();
        //p.rigid.AddForce(vel, ForceMode.Impulse);
        //}



            //----- Raycast стрельба и Эффекты -----\\        
        float dist = Vector3.Distance(PROJECTILE_ANCHOR.position, player.pointer.position);         // ТУТ ПЕРЕДЕЛАТЬ !                                                                                                    
        if (dist > 2)                                                                               // (меняет угол, если прицел близко)
        {
              // Поворачиваем якорь стрельбы, чтобы точно попадать рейкастами
            Vector3 lookDir = player.pointer.position - PROJECTILE_ANCHOR.position;
            float angle = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
            qua1 = Quaternion.Euler(0, angle, 0);
            PROJECTILE_ANCHOR.rotation = Quaternion.Lerp(transform.rotation, qua1, 1);
        }
        else
        {
              // Если прицел ближе dist делаем угол
            qua1 = Quaternion.Euler(0, 180, 0);        
            PROJECTILE_ANCHOR.localRotation = Quaternion.Lerp(transform.rotation, qua1, 1);
        }

        //Debug.DrawRay(PROJECTILE_ANCHOR.position, PROJECTILE_ANCHOR.transform.forward * 100f, Color.yellow);


          // Вспышки при стрельбе
        foreach (var flash in muzzleFlash)
        {
            flash.Emit(1);
        }

            // Свет
        StartCoroutine(LightDelay());   // делаем задержку для вспышек


        if (weaponName == "shotgun")   // Дробовик
        {

            for (int i = 0; i < 7; i++)
            {
                // Рейкаст
                //Vector3 vector3 = new Vector3(0f, 0, 0.2f - (0.1f * i));
                //PROJECTILE_ANCHOR.position = new Vector3(0, 0, 1);
                qua2 = Quaternion.Euler(0, 178f + (1.5f * i), 0);
                PROJECTILE_ANCHOR.localRotation = Quaternion.Lerp(transform.rotation, qua2, 1);
                MakeRayCast();
                //yield return new WaitForSeconds(0.000f);
            }
        }

/*        if (weaponName == "sniper")   // СВД
        {
            MakeRayCastAll();
        }*/

        else
        {
            MakeRayCast();
        }
    }



    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\
    


    public void MakeRayCast()
    {
        // Звуки
        audioSource.Play();


            // Настройки для трасеров
        TrailRenderer tracer = Instantiate(tracerEffect, EFFECT_ANCHOR.position, Quaternion.identity);          // создаем трасер
        tracer.AddPosition(EFFECT_ANCHOR.position);                                                             // начальная позиция 
        
            // Рейкаст
        float randomBulletX = Random.Range(-recoilX, recoilX);
        float randomBulletY = Random.Range(-recoilY, recoilY);

        ray.origin = PROJECTILE_ANCHOR.position;        // луч из позиции якоря
        ray.direction = PROJECTILE_ANCHOR.forward + new Vector3(randomBulletX, randomBulletY, 0);      // луч с направлением вперед
        //Debug.DrawRay(PROJECTILE_ANCHOR.position, PROJECTILE_ANCHOR.transform.forward * 100f, Color.yellow);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerIgnore))
        {
            //Debug.DrawLine(ray.origin, hit.point, Color.red, 1.0f);                                   // дебаг, красные линии

            tracer.transform.position = hit.point;                                                      // конечная позиция трасера пули 

            Enemy_old enemy = hit.collider.GetComponentInParent<Enemy_old>();
            if (enemy)
            {
                Damage dmg = new Damage()
                {
                    damageAmount = rayDamage,
                    origin = transform.position,
                    pushForce = pushForce
                };
                enemy.SendMessage("ReceiveDamage", dmg);


                // Эффекты попадания (кровь)
                hitEffectBlood.transform.position = hit.point;
                hitEffectBlood.transform.forward = hit.normal;
                hitEffectBlood.Emit(1);


            }
            else
            {
                // Эффекты попадания (искры)
                hitEffect.transform.position = hit.point;
                hitEffect.transform.forward = hit.normal;
                hitEffect.Emit(1);
            }
        }
    }



    public void MakeRayCastAll()                // для прострелов (пока не смог сделать)
    {
        // Настройки для трасеров
        TrailRenderer tracer = Instantiate(tracerEffect, EFFECT_ANCHOR.position, Quaternion.identity);
        tracer.AddPosition(EFFECT_ANCHOR.position);

        // Рейкаст
        ray.origin = PROJECTILE_ANCHOR.position;        // луч из позиции якоря
        ray.direction = PROJECTILE_ANCHOR.forward;      // луч с направлением вперед
        //Debug.DrawRay(PROJECTILE_ANCHOR.position, PROJECTILE_ANCHOR.transform.forward * 100f, Color.yellow);
        //Debug.DrawLine(ray.origin, hit.point, Color.red, 1.0f);
        int hits = Physics.RaycastNonAlloc(ray, m_Results, Mathf.Infinity);
        for (int i = 0; i < hits; i++)
        {
            tracer.transform.position = m_Results[i].point;   // трасеры пуль             

            Enemy_old enemy = m_Results[i].collider.GetComponentInParent<Enemy_old>();
            if (enemy)
            {
                Damage dmg = new Damage()
                {
                    damageAmount = rayDamage,
                    origin = transform.position,
                    pushForce = pushForce
                };
                enemy.SendMessage("ReceiveDamage", dmg);


                // Эффекты попадания (кровь)
                hitEffectBlood.transform.position = m_Results[i].point;
                hitEffectBlood.transform.forward = m_Results[i].normal;
                hitEffectBlood.Emit(1);


            }
            else
            {
                // Эффекты попадания (искры)
                hitEffect.transform.position = m_Results[i].point;
                hitEffect.transform.forward = m_Results[i].normal;
                hitEffect.Emit(1);
            }
        }
    
        if (hits == 0)
        {
            Debug.Log("Did not hit");
        }
    }


/*        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            

            
    }*/


    /*    IEnumerator ShotDelay()
        {
            for (int i = 0; i < 5; i++)
            {
                // Рейкаст
                //Vector3 vector3 = new Vector3(0f, 0, 0.2f - (0.1f * i));
                //PROJECTILE_ANCHOR.position = new Vector3(0, 0, 1);
                qua2 = Quaternion.Euler(0, 176 + (2 * i), 0);
                PROJECTILE_ANCHOR.localRotation = Quaternion.Lerp(transform.rotation, qua2, 1);
                MakeRayCast();
                yield return new WaitForSeconds(0.000f);            
            }
        }
    */

    IEnumerator LightDelay()
    {
        lightEffect.enabled = true;
        yield return new WaitForSeconds(0.015f);
        lightEffect.enabled = false;
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


/*    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate(projPrefab);
        go.transform.position = PROJECTILE_ANCHOR.position;
        go.transform.rotation = PROJECTILE_ANCHOR.rotation;
        Projectile p = go.GetComponent<Projectile>();
        return p;
    }*/


    /*    IEnumerator AttackStartCor(float delay_1, float delay_2)
        {
            yield return new WaitForSeconds(delay_1);
            boxCollider.enabled = true;


            yield return new WaitForSeconds(delay_2);
            boxCollider.enabled = false;
            //attacking = false;
        }
    */


    

    

    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\

    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\

}




