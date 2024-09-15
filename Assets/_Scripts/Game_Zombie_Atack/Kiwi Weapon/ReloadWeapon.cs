using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    public Animator rigController;
    public WeaponAnimationEvents animationEvents;
    public ActiveWeapon activeWeapon;
    public Transform leftHand;
    public AmmoPack ammoPack;
    //GameObject magazineHand;              // временный магазин в руке при перезарядке


    void Start()
    {
        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);     // получаем ивенты от анимации перезарядки
    }


    void Update()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        if (weapon && weapon.allAmmo > 0)       // перезаряжаемся только если есть оружие и всего патронов больше 0
        {          
            if (Input.GetKeyDown(KeyCode.R) && (weapon.ammoCount != weapon.clipSize))
            {
                rigController.SetTrigger("reload_weapon");
            }            
            if ( weapon.ammoCount <= 0)
            {
                StartCoroutine(ReloadDelay());                
            }           
        }
    }



    IEnumerator ReloadDelay()
    {        
        yield return new WaitForSeconds(0.5f);
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        if (weapon.ammoCount <= 0 && !activeWeapon.isHolsted)
            rigController.SetTrigger("reload_weapon");
    }



    void OnAnimationEvent(string eventName)
    {
        //Debug.Log(eventName);
        switch (eventName)
        {
            case "start_reload":
                activeWeapon.isHolsted = true;                      // оружие не активно
                activeWeapon.reloaring = true;                      // нельзя сменить оружие                 
                break;                
            case "detouch_magazine":
                activeWeapon.audioSourses.reloadStart.Play();            // звук перезарядки
                DetouchMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                activeWeapon.audioSourses.reloadEnd.Play();            // звук перезарядки
                break;
            case "stop_reload":
                activeWeapon.isHolsted = false;
                activeWeapon.reloaring = false;
                break;
        }
    }



    void DetouchMagazine()
    {
        
        //RaycastWeapon weapon = activeWeapon.GetActiveWeapon();      // ссылка на активное оружие
        //magazineHand = Instantiate(weapon.magazine, leftHand, true);    // создаём новый временный магазин с родителем левая рука
        //weapon.magazine.SetActive(false);   // прячем оригинальный магазин на оружии
    }
            

    void DropMagazine()
    {
/*        GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        droppedMagazine.GetComponent<BoxCollider>().size = new Vector3(0.001f, 0.002f, 0.001f);     // меш обоймы увеличен в 100 раз
        Destroy(droppedMagazine, 30);   // уничтожаем упавший магазин через 30 сек
        magazineHand.SetActive(false);*/
    }



    void RefillMagazine()
    {
        //magazineHand.SetActive(true);
    }



    void AttachMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
/*        weapon.magazine.SetActive(true);    // возвращаем магазин на оружии активным
        Destroy(magazineHand);*/

            // Система патронов
        int ammoTaken = weapon.clipSize - weapon.ammoCount;         // сколько патронов взято (размер магазина - патроны(счетчик) в обойме)
        if (ammoTaken > weapon.allAmmo)                             // если оставшихся патронов всего меньше, чем нам нужно зарядить
        {
            ammoTaken = weapon.allAmmo;
        }
        weapon.ammoCount += ammoTaken;                              // счетчик + сколько патронов взяли
        weapon.allAmmo -= ammoTaken;                                // из всех патронов вычитаем сколько взяли


        if (weapon.allAmmo < 0)
        {
            weapon.allAmmo = 0;
        }
                
        switch (weapon.ammoType)
        {
            case "9":
                ammoPack.allAmmo_9 = weapon.allAmmo;                
                break;
            case "0.357":
                ammoPack.allAmmo_0_357 = weapon.allAmmo;                
                break;
            case "5.56":
                ammoPack.allAmmo_5_56 = weapon.allAmmo;
                break;
            case "0.12":
                ammoPack.allAmmo_0_12 = weapon.allAmmo;
                break;
            case "7.62":
                ammoPack.allAmmo_7_62 = weapon.allAmmo;                
                break;
            case "0.50":
                ammoPack.allAmmo_0_50 = weapon.allAmmo;                
                break;
        }        

        rigController.ResetTrigger("reload_weapon");
    }
}
