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
    public ActiveWeapon.WeaponSlot weaponSlot;  // внутри перечисление слотов (основное, вторичное оружия) и выбирается в инспекторе префаба
    public string ammoType;     // тип патронов, указать в инспекторе (потом поменять на список)
    public int indexNumberWeapon;   // Индекс оружия для переключения между оружиями (Потом поменять на список или использовать weaponSlot[index]

    public float cooldown = 0.1f;  // 600 выстрелов в секунду 
    //public float bulletSpeed = 30f;     // скорость пули (не используется)
    public int rayDamage = 1;   // урон рейкаста

    private float lastSwing;        // для кд
    //public bool startAttack = false;
    //public bool attacking = false;

        // Перезарядка
    public int allAmmo;     // всего патронов
    public int clipSize;    // размер магазина 
    public int ammoCount;   // счетчик в магазине

        // Ссылки 
    Player player;          //ссылка на игрока
    AmmoPack playerAmmo;    //ссылка на патроны игрока
    ActiveWeapon activeWeapon;
    //public GameObject projPrefab;   // префаб снаряда
    public Transform PROJECTILE_ANCHOR;   // якорь для оружия
    public Transform EFFECT_ANCHOR;   // якорь для оружия
    public GameObject magazine;     // магазин

        // Эффекты
    public ParticleSystem[] muzzleFlash;    // вспышка оружия
    public ParticleSystem hitEffect;        // попадание в стену
    public ParticleSystem hitEffectBlood;   // попадание в зомби
    public TrailRenderer tracerEffect;  // трасер
    public Light lightEffect;     //вспышка света

    RaycastHit hit;
    Ray ray;
    Quaternion qua1;


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\

    public void Start()
    {
        player = GameManager.instance.player;
        playerAmmo = GameManager.instance.player.GetComponent<AmmoPack>();        
        TakeAmmo();     // Берем патроны из АммоПак игрока
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        
    }

    public void Update()
    {
        
    }

    public void TakeAmmo()
    {
        switch (ammoType)
        {
            case "9 mm":
                allAmmo = playerAmmo.allAmmo_9mm;
                break;
            case "7.62":
                allAmmo = playerAmmo.allAmmo_762;
                break;
            case "0.12":
                allAmmo = playerAmmo.allAmmo_012;
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

    /*    public void AttackMeele()
        {
            lastSwing = Time.time;
            //attackCooldown = 1f / attackSpeed;
            startAttack = false;
            //attacking = true;
            //Swing();
            StartCoroutine(AttackStartCor(hitBoxDelay, hitBoxTimeExist));
        }*/


    public void AttackRange()
    {
        if (ammoCount <= 0)
        {
            return;
        }

        ammoCount--;    // - патрон за каждый выстрел

        lastSwing = Time.time;

        //attackCooldown = 1f / attackSpeed;
        //startAttack = false;
        //attacking = true;

        // Снаряды
        //Projectile p;
        //Vector3 vel = PROJECTILE_ANCHOR.forward * bulletSpeed; // скорость пули

        //switch (type)
        //{
        //case WeaponType.blaster:
        //p = MakeProjectile();
        //p.rigid.AddForce(vel, ForceMode.Impulse);
        //}





        //----- Raycast стрельба и Эффекты -----\\        
        float dist = Vector3.Distance(PROJECTILE_ANCHOR.position, player.pointer.position);         // ТУТ ПЕРЕДЕЛАТЬ !
        if (dist > 2)
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

            // Настройки для трасеров
        var tracer = Instantiate(tracerEffect, EFFECT_ANCHOR.position, Quaternion.identity);
        tracer.AddPosition(EFFECT_ANCHOR.position);

          // Рейкаст
        ray.origin = PROJECTILE_ANCHOR.position;        // луч из позиции якоря
        ray.direction = PROJECTILE_ANCHOR.forward;      // луч с направлением вперед
        //Debug.DrawRay(PROJECTILE_ANCHOR.position, PROJECTILE_ANCHOR.transform.forward * 100f, Color.yellow);
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.DrawLine(ray.origin, hit.point, Color.red, 1.0f);

            tracer.transform.position = hit.point;   // трасеры пуль 

            Enemy_old enemy = hit.collider.GetComponentInParent<Enemy_old>();
            if (enemy)
            {
/*                if (tag == "Arm")
                {
                    Damage dmgArm = new Damage()
                    {
                        damageAmount = rayDamage/2,
                        origin = transform.position,
                        pushForce = 0
                    };
                    enemy.SendMessage("ReceiveDamage", dmgArm);
                }*/
 
                    Damage dmg = new Damage()
                    {
                        damageAmount = rayDamage,
                        origin = transform.position,
                        pushForce = 0
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


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\
    
    
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




