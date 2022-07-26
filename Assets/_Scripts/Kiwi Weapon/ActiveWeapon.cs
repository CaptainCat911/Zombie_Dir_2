﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;



public class ActiveWeapon : MonoBehaviour
{
    public enum WeaponSlot
    {
        Pistol = 0,
        Rifle = 1,
        Heavy = 2,
        Melee = 3
    }

    public Player player;
    public Rig animLayer;               // ссылка на риг рук (общий)
    public Animator rigController;      // ссылка на аниматор риг
    public Animator playerAnim;
    //float animDuration = 0.3f;

    public RaycastWeapon[] equiped_weapons = new RaycastWeapon[3];     // массив оружий 
    public List<RaycastWeapon> listWeaponPistol;                // список пистолетов 
    public List<RaycastWeapon> listWeaponRifle;                 // список винтовок
    public List<RaycastWeapon> listWeaponHeavy;                 // список тяжелого оружия
    //public List<RaycastWeapon> listWeaponMelee;                 // список ближнего оружия


    int activeWeaponIndex;
    public bool isHolsted = false;              // оружие спрятано
    public bool attackActive = false;           // активация атаки
    public bool reloaring = false;              // во время перезарядки нельзя сменить оружие или убрать его (потом исправить)
    public bool switching = false;              // во время смены оружия нельзя чё-то делать?


    public Transform[] weaponSlots;             // слоты под оружие
    public Transform weaponLeftGrip;            // риг левой руки
    public Transform weaponRightGrip;

    int iPistol = -1;
    int iRifle = -1;
    int iHeavy = -1;
    //int iMelee = -1;


    private float lastThrow;                // для кд топора
    //public float cooldownGranate = 2f;  // кд ударов

    public WeaponAnimationEvents animationEvents;

    public float attackRadiusHitBox = 1;    // радиус хитбокса
    public LayerMask layerEnemy;            // маска для зомби
    public int damage;                      // урон топора
    public int damageHand;                  // урон рукой
    public int lifeStealHp;                 // сколько хп восстанавливает
    public float pushForce;                 // замедление

    public Transform hitBox;                // хитбокс (где будет создаваться сфера для урона)

    public GameObject axeBack;              // топор за спиной
    public GameObject axeHand;              // топор в руках
    public bool getAxe = false;             // подобрал топор или нет

    public GameObject granateBack;          // граната за спиной
    public GameObject granateHand;          // граната в руках
    public GameObject granateThrow;         // граната, которую бросают в руках
    public Transform rightHand;             // трансформ правой руки для броска гранаты
    public int granateForce;                // сила броска гранаты

    AmmoPack ammoPack;                      // ссылка на скрипт патронов (инвентарь)

    public AudioSourses audioSourses;       // ссылка на объект с аудиоисточниками

    bool granateInAction;                   // для правильного броска гранаты

    AmmoPickUpSphere ammoSphere;            // ссылка на скрипт сферы для поднятия патронов

    public LayerMask layerAmmo;             // маска для патронов

    public List<RaycastWeapon> listWeapons;     // список всех оружий

    public GameObject axeEffectSmoke;           // эффект дыма для топора
    public GameObject axeEffectAttack;          // эффект дыма для топора во время удара
    public GameObject bottleCola;               // для "аптечки"
    public bool lifeSteal;                      // для лайфстила топором


    //---------------------------------------------------------------------------------------------\\


    void Start()    
    {
        //listWeaponRifle = new List<RaycastWeapon>();

        playerAnim = GetComponent<Animator>();
        ammoSphere = GetComponentInChildren<AmmoPickUpSphere>();
        ammoPack = player.GetComponent<AmmoPack>();                                     // ссылка на аммопак (инвентарь)

        animationEvents.MeleeAnimationEvent.AddListener(OnAnimationEventAttack);        // получаем ивенты от анимации атаки
        animationEvents.GranateAnimationEvent.AddListener(OnAnimationEventThrow);       // получаем ивенты от анимации броска гранаты
        animationEvents.HealAnimationEvent.AddListener(OnAnimationEventHeal);           // получаем ивенты от анимации лечения
        animationEvents.HealAnimationEvent.AddListener(OnAnimationEventUse);            // получаем ивенты от анимации лечения

        

        //animationReload = FindAnimation(playerAnim, "weapon_anim_reload_pistol");
        

/*        foreach (AnimationState state in animationReload)
        {
            state.speed = 0.5f;
        }*/

        /*        getAxe = true;                                  // выдаем топор
                axeBack.SetActive(true);                        // топор активен*/
    }

    //---------------------------------------------------------------------------------------------\\


    public void EquipActiveStart()              // не используется
    {
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();     // поиск оружия в иерархии
        if (existingWeapon)
            Equip(existingWeapon);
    }




    // Для перезарядки 
    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }




    // Для поиска активного оружия
    public RaycastWeapon GetWeapon (int index)
    {
        if (index < 0 || index >= equiped_weapons.Length)
        {
            return null;
        }
        return equiped_weapons[index];                      // возвращаем из массива оружие с индексом
    }



    void FixedUpdate()
    {

    }


    //---------------------------------------------------------------------------------------------\\


    void Update()
    {
        // Использование
        if (Input.GetKeyDown(KeyCode.E))       //&& player.inRangeUse
        {
            PickUpAmmo();
            //playerAnim.SetTrigger("Use");
        }

        //Debug.Log(player.inRangeUse);

        if (GameManager.instance.playerStop || !GameManager.instance.player.isAlive)            // если playerStop возвращаемся
        {            
            return;
        } 



        RaycastWeapon weapon = GetWeapon(activeWeaponIndex);        // находим активное оружие и если оно не спрятано - постоянно обновляем 
        if (weapon && !isHolsted)
        {
            weapon.UpdateWeapon();
        }
        
        if (activeWeaponIndex == 1 || activeWeaponIndex == 2)       // тут индекс оружия из которого стрелять (ПОМЕНЯТЬ ПОТОМ)
        {
            if (Input.GetMouseButton(0))
            {
                attackActive = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                attackActive = false;
            }
        }

        if (activeWeaponIndex == 0)                             // СДЕЛАТЬ ПРАВИЛЬНЫЕ ВЫСТРЕЛЫ ПИСТОЛЕТОМ ! (Пистолеты)
        {            
            if (Input.GetMouseButton(0))
            {
                attackActive = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                attackActive = false;
            }
        }






        //-------------------------------Управление-----------------------------------------\\


        // Удар рукой
        if (Input.GetMouseButtonDown(1) && !reloaring && !getAxe)
        {
            playerAnim.SetTrigger("hand_attack");
        }

        // Удар топором
        if (Input.GetMouseButtonDown(1) && !reloaring && getAxe)
        {
            playerAnim.SetTrigger("axe_attack");            
        }

        

        // Лечение
        if (Input.GetKeyDown(KeyCode.Q) && !reloaring && ammoPack.HPBox > 0 && player.currentHealth < player.maxHealth)
        {
            playerAnim.SetTrigger("Heal");
        }





        // Бросок гранаты
        if ((Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Mouse2)) && !reloaring && !granateInAction && ammoPack.granate > 0)
        {            
            float dist = Vector3.Distance(transform.position, player.pointer.position);
            if (dist > 5f)
            {
                playerAnim.SetTrigger("Throw");
            }
            else
            {
                playerAnim.SetTrigger("Throw_2");
            }                       
        }

        //Debug.Log(isHolsted);
        //Debug.Log(activeWeaponIndex);

        if (Input.GetKeyDown(KeyCode.X))
        {
            //ToggleActiveWeapon();
        }


        if (reloaring)
        {
            return;
        }        


        // Смена оружия
        if (Input.GetKeyDown(KeyCode.Alpha1))                               // || (Input.GetAxis("Mouse ScrollWheel") > 0 && activeWeaponIndex == 1) || (Input.GetAxis("Mouse ScrollWheel") < 0 && activeWeaponIndex == 2)
        {
            if (listWeaponPistol.Count > 1)                                 // Если больше 1-го пистолета
            {
                listWeaponPistol[iPistol].gameObject.SetActive(false);      // Прячем действующее оружие
                if (activeWeaponIndex == 0)                                 // Если актиновое оружие пистолет
                    iPistol++;                                              // Добавляем счетчик
                if (iPistol >= listWeaponPistol.Count)                      // Если пистолетов 2 - сбрасываем счетчик
                {
                    iPistol = 0;
                }
                listWeaponPistol[iPistol].gameObject.SetActive(true);       // Делаем оружие активным 
                Equip(listWeaponPistol[iPistol]);                           // Экипируем его 
            }
            if (listWeaponPistol.Count == 1)                                // Если пистолет только один
                Equip(listWeaponPistol[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (listWeaponRifle.Count > 1)
            {
                listWeaponRifle[iRifle].gameObject.SetActive(false);
                if (activeWeaponIndex == 1)
                    iRifle++;
                if (iRifle >= listWeaponRifle.Count)
                {
                    iRifle = 0;
                }
                listWeaponRifle[iRifle].gameObject.SetActive(true);
                //StartCoroutine(HolsterWeapon_2());
                Equip(listWeaponRifle[iRifle]);
            }
            if (listWeaponRifle.Count == 1)
                Equip(listWeaponRifle[0]);

            //Debug.Log(rifleCount);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (listWeaponHeavy.Count > 1)
            {
                listWeaponHeavy[iHeavy].gameObject.SetActive(false);
                if (activeWeaponIndex == 2)
                    iHeavy++;
                if (iHeavy >= listWeaponHeavy.Count)
                {
                    iHeavy = 0;
                }
                listWeaponHeavy[iHeavy].gameObject.SetActive(true);
                Equip(listWeaponHeavy[iHeavy]);
            }
            if (listWeaponHeavy.Count == 1)
                Equip(listWeaponHeavy[0]);
        }

/*        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Equip(listWeaponMelee[0]);
        }*/
    }




    // Ивенты удара топора
    void OnAnimationEventAttack(string eventName)              
    {
        //Debug.Log(eventName);
        switch (eventName)
        {
            case "axe_start":
                //axeEffectAttack.SetActive(true);
                audioSourses.axeMiss.Play();                    // звук замаха топором                
                //Debug.Log("Start !");
                ToggleActiveWeapon();                           // убираем основное оружие
                axeBack.SetActive(false);                       // прячем топор за спиной
                axeHand.SetActive(true);                        // активируем топор в руках
                reloaring = true;                               // перезарядка (для запрета других действий)
                GameManager.instance.playerStop = true;         // останавливаем игрока
                break;

            case "axe_hit":
                //Debug.Log("Hit !");                
                Collider[] collidersHitbox = Physics.OverlapSphere(hitBox.position, attackRadiusHitBox, layerEnemy);
                foreach (Collider enObjectBox in collidersHitbox)
                {
                    if (enObjectBox.tag != "Enemy")
                    {
                        //Debug.Log(enObjectBox.name);
                        //audioSourses.axeMiss.Play();
                        //continue;
                    }

                    if (enObjectBox.tag == "Enemy")
                    {
                        audioSourses.axeAttack.Play();                                      // звук попадания топором
                        Enemy_old enemy = enObjectBox.GetComponentInParent<Enemy_old>();
                        NPC npc = enObjectBox.GetComponentInParent<NPC>();
                        if (enemy)
                        {
                            Damage dmg = new Damage()
                            {
                                damageAmount = damage,
                                origin = transform.position,
                                stopForce = pushForce
                            };
                            if (lifeSteal)
                            {
                                player.Heal(lifeStealHp);
                            }
                            enemy.TakeHitAxeBlood();                            // эффект крови
                            enemy.SendMessage("ReceiveDamage", dmg);
                        }
                        else if (npc)
                        {
                            Damage dmg = new Damage()
                            {
                                damageAmount = damage,
                                origin = transform.position,
                                stopForce = pushForce
                            };
                            npc.TakeHitAxeBlood();                            // эффект крови
                            npc.SendMessage("ReceiveDamage", dmg);
                        }
                        collidersHitbox = null;
                    }
                }                
                break;            

            case "axe_stop":
                //Debug.Log("Stop");
                //axeEffectAttack.SetActive(false);
                reloaring = false;
                GameManager.instance.playerStop = false;
                axeBack.SetActive(true);
                axeHand.SetActive(false);
                ToggleActiveWeapon();
                break;



            case "hand_start":
                
                //audioSourses.axeMiss.Play();                    // звук замаха топором                
                ToggleActiveWeapon();                           // убираем основное оружие
                reloaring = true;                               // перезарядка (для запрета других действий)
                GameManager.instance.playerStop = true;         // останавливаем игрока
                break;

            case "hand_hit":                
                Collider[] collidersHitboxHand = Physics.OverlapSphere(hitBox.position, attackRadiusHitBox, layerEnemy);
                foreach (Collider enObjectBox in collidersHitboxHand)
                {
                    if (enObjectBox.tag != "Enemy")
                    {
                        //Debug.Log(enObjectBox.name);
                        //audioSourses.axeMiss.Play();
                        //continue;
                    }

                    if (enObjectBox.tag == "Enemy")
                    {
                        //audioSourses.axeAttack.Play();                                      // звук попадания топором
                        Enemy_old enemy = enObjectBox.GetComponentInParent<Enemy_old>();
                        NPC npc = enObjectBox.GetComponentInParent<NPC>();
                        if (enemy)
                        {
                            Damage dmg = new Damage()
                            {
                                damageAmount = damageHand,
                                origin = transform.position,
                                stopForce = pushForce
                            };
                            //enemy.TakeHitAxeBlood();                            // эффект крови
                            enemy.SendMessage("ReceiveDamage", dmg);
                        }
                        else if (npc)
                        {
                            Damage dmg = new Damage()
                            {
                                damageAmount = damageHand,
                                origin = transform.position,
                                stopForce = pushForce
                            };
                            //npc.TakeHitAxeBlood();                            // эффект крови
                            npc.SendMessage("ReceiveDamage", dmg);
                        }
                        collidersHitbox = null;
                    }
                }
                break;

            case "hand_stop":
                //Debug.Log("Stop");
                //axeEffectAttack.SetActive(false);
                reloaring = false;
                GameManager.instance.playerStop = false;
                ToggleActiveWeapon();
                break;

        }
    }



    // Ивенты броска гранаты
    void OnAnimationEventThrow(string eventName)               
    {
        //Debug.Log(eventName);
        
        switch (eventName)
        {
            case "start_granate":
                //Debug.Log("Start Granate !");
                ToggleActiveWeapon();                
                //granateBack.SetActive(false);
                granateHand.SetActive(true);
                reloaring = true;
                GameManager.instance.playerStop = true;
                break;

            case "throw_granate":
                //Debug.Log("Throw !");
                ammoPack.granate -= 1;
                granateHand.SetActive(false);
                GameObject go = Instantiate(granateThrow);                                    // Создаём префаб гранаты
                //go.transform.SetParent(transform, false);                                   // Назначаем этот спавнер родителем
                go.transform.position = rightHand.position;
                Rigidbody rb = go.GetComponent<Rigidbody>();

                float dist = Vector3.Distance(transform.position, player.pointer.position);
                if (dist > 7.5f)
                    dist = 7.5f;
                //Debug.Log(dist);
                Vector3 vec3 = new Vector3(0, 0.3f, 0) + transform.forward;         // чтобы летела вперед и чуть вверх
                rb.AddForce(vec3 * dist, ForceMode.Impulse);                        // бросаем её с учетом дистанции до прицела 
                break;

            case "end_granate":
                //Debug.Log("End Granate");
                reloaring = false;
                GameManager.instance.playerStop = false;
                ToggleActiveWeapon();
                break;
        }
    }



    // Ивенты лечения
    void OnAnimationEventHeal(string eventName)
    {
        switch (eventName)
        {
            case "start_heal":
                //Debug.Log("Heal!");
                ToggleActiveWeapon();
                player.walking = true;
                reloaring = true;
                bottleCola.SetActive(true);
                break;

            case "healed":
                player.walking = false;
                reloaring = false;
                player.Heal(25);
                ammoPack.HPBox -= 1;
                playerAnim.SetTrigger("Stop_Heal");
                ToggleActiveWeapon();
                bottleCola.SetActive(false);
                break;
        }
    }




    // Ивенты использования
    void OnAnimationEventUse(string eventName)
    {
        switch (eventName)
        {
            case "use_start":
                //Debug.Log("Use!");
                ToggleActiveWeapon();
                //player.walking = true;
                GameManager.instance.playerStop = true;
                reloaring = true;
                break;

            case "use_stop":
                //Debug.Log("Use_Stop!");
                //player.walking = false;
                GameManager.instance.playerStop = false;
                reloaring = false;
                ammoSphere.ammo.PickUp();
                ToggleActiveWeapon();
                break;
        }
    }



    // Подобрать патроны
    public void PickUpAmmo()
    {
        Collider[] collidersHitbox = Physics.OverlapSphere(hitBox.position, 0.3f, layerAmmo);
        foreach (Collider enObjectBox in collidersHitbox)
        {

            if (enObjectBox.tag == "Weapon")
            {
                //audioSourses.axeAttack.Play();                                      // звук попадания топором
                AmmoPickUp ammoBox = enObjectBox.GetComponentInParent<AmmoPickUp>();               
                WeaponPickUp weaponPickUp = enObjectBox.GetComponentInParent<WeaponPickUp>();
                
                Damage dmg = new Damage()
                {
                    damageAmount = 1,
                    origin = transform.position,
                    stopForce = pushForce
                };
                //enemy.TakeHitAxeBlood();                                          // эффект крови
                if (ammoBox)
                {
                    ammoBox.SendMessage("ReceiveDamage", dmg);
                }

                else if (weaponPickUp)
                {
                    weaponPickUp.SendMessage("ReceiveDamage", dmg);
                }
            }

            if (enObjectBox.tag == "Enemy")
            {
                NPC npc = enObjectBox.GetComponentInParent<NPC>();
                if (npc && !GameManager.instance.diffManager.levelGo)           // если есть НПС и волна ещё не началась
                {
                    npc.OpenMagazine();
                    //Debug.Log("Use!");
                }
            }
        }
    }


    // weaponSlotIndex - номер типа оружия 0 - пистолеты , 1 - винотвки




    // Подобрать оружие
    public void GetWeaponUp(RaycastWeapon newWeapon)            
    {
        if (newWeapon.indexNumberWeapon == 1)       // Если подобранное оружие - пистолет
        {
            listWeaponPistol.Add(newWeapon);
            if (listWeaponPistol.Count >= 2)
                listWeaponPistol[iPistol].gameObject.SetActive(false);
            iPistol++;
            Equip(newWeapon);            
        }

        if (newWeapon.indexNumberWeapon == 2)       // Винтовка
        {            
            listWeaponRifle.Add(newWeapon);
            if (listWeaponRifle.Count >= 2)
                listWeaponRifle[iRifle].gameObject.SetActive(false);
            iRifle++;
            Equip(newWeapon);
        }

        if (newWeapon.indexNumberWeapon == 3)       // Тяжелое
        {
            listWeaponHeavy.Add(newWeapon);
            if (listWeaponHeavy.Count >= 2)
                listWeaponHeavy[iHeavy].gameObject.SetActive(false);
            iHeavy++;
            Equip(newWeapon);
        }

        listWeapons.Add(newWeapon);
        /*        if (newWeapon.indexNumberWeapon == 4)       // Топор
                {
                    listWeaponMelee.Add(newWeapon);
                    Equip(newWeapon);
                }*/
    }




    // Часть кода из ютуба Kiwi
    // Экипировать оружие
    public void Equip(RaycastWeapon newWeapon)
    {
        //Debug.Log("Equip !");
        switching = true;
        int weaponSlotIndex = (int)newWeapon.weaponSlot;    // weaponSlot - это основное, дополнительное, тяжелое, мили 
        var weapon = GetWeapon(weaponSlotIndex);    //  получаем оружие с нашим индексом оружия

/*        if (weapon)
        {
            
        }*/

        weapon = newWeapon;     // присваиваем переменной weapon подобранное оружие
        weapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);     // ??? выбираем родителя по номеру слота (основное, дополнительное)
        /*weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;       */ 
        equiped_weapons[weaponSlotIndex] = weapon;
        SetActiveWeapon(newWeapon.weaponSlot);        
    }



    // Спрятать оружие
    public void ToggleActiveWeapon()
    {
        if (reloaring)
        {
            return;
        }
        bool isHolsted = rigController.GetBool("holster_weapon");
        if (isHolsted)
        {
            StartCoroutine(ActivateWeapon(activeWeaponIndex));
        }
        else
        {
            StartCoroutine(HolsterWeapon(activeWeaponIndex));
        }
    }




    void SetActiveWeapon (WeaponSlot weaponSlot)
    {
        if (reloaring)
        {
            return;
        }
        int holsterIndex = activeWeaponIndex;
        int activateIndex = (int)weaponSlot;

        if (holsterIndex == activateIndex)
        {
            holsterIndex = -1;
        }

        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }


    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        //yield return new WaitForSeconds(1);
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
    }

    IEnumerator HolsterWeapon (int index)       
    {        
        //Debug.Log("Holsted !");
        isHolsted = true;
        RaycastWeapon weapon = GetWeapon(index);
        if (true)
        {
            //Debug.Log("Holsted2 !");
            //weapon.gameObject.SetActive(false);
            rigController.SetBool("holster_weapon", true);
            //playerAnim.SetTrigger("holster_trig");
            do
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(0.2f);      // ПЕРЕДЕЛАТЬ !
                //Debug.Log("Frame !");
                //Debug.Log(rigController.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }
            while (false);       // НЕ РАБОТАЕТ ! rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
        }
    }


    IEnumerator ActivateWeapon(int index)
    {
        //Debug.Log("Active !");
        //isHolsted = true;
        RaycastWeapon weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", false);
            rigController.Play("equip_" + weapon.weaponName);
            do
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(0.1f);      // ПЕРЕДЕЛАТЬ !
            }
            while (false);     // НЕ РАБОТАЕТ !
        }
        if (weapon)
            weapon.TakeAmmo();
        isHolsted = false;
        switching = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(hitBox.position, attackRadiusHitBox);
    }
}