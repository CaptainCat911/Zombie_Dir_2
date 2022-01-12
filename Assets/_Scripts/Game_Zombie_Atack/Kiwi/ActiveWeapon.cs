using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
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

    public Rig animLayer;   // ссылка на риг рук (общий)
    public Animator rigController;      // ссылка на аниматор риг
    //float animDuration = 0.3f;

    RaycastWeapon[] equiped_weapons = new RaycastWeapon[3];   // массив оружий 
    public List<RaycastWeapon> listWeaponPistol;    // список пистолетов 
    public List<RaycastWeapon> listWeaponRifle;    // список винтовок
    public List<RaycastWeapon> listWeaponHeavy;    // список тяжелого оружия
    public List<RaycastWeapon> listWeaponMelee;    // список ближнего оружия


    int activeWeaponIndex;
    public bool isHolsted = false;      // оружие спрятано
    public bool attackActive = false;       // активация атаки
    public bool reloaring = false;      // во время перезарядки нельзя сменить оружие или убрать его (потом исправить)


    public Transform[] weaponSlots;     // слоты под оружие
    public Transform weaponLeftGrip;
    public Transform weaponRightGrip;

    int iPistol = -1;
    int iRifle = -1;
    int iHeavy = -1;
    int iMelee = -1;


    void Start()
    {
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();     // поиск оружия в иерархии
        if (existingWeapon)
            Equip(existingWeapon);

        listWeaponRifle = new List<RaycastWeapon>();
    }


      // для перезарядки 
    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }




    // для поиска активного оружия
    public RaycastWeapon GetWeapon (int index)
    {
        if (index < 0 || index >= equiped_weapons.Length)
        {
            return null;
        }
        return equiped_weapons[index];      // возвращаем из массива оружие с индексом
    }




    void FixedUpdate()
    {

    }




     void Update()
    {
        RaycastWeapon weapon = GetWeapon(activeWeaponIndex);
        if (weapon && !isHolsted)
        {
            weapon.UpdateWeapon();
        }
        
        if (activeWeaponIndex == 1 || activeWeaponIndex == 2)       // тут индекс оружия из которого стрелять (ПОМЕНЯТЬ ПОТОМ)
        {
            if (Input.GetMouseButtonDown(0))
            {
                attackActive = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                attackActive = false;
            }
        }

        if (activeWeaponIndex == 0)     // СДЕЛАТЬ ПРАВИЛЬНЫЕ ВЫСТРЕЛЫ ПИСТОЛЕТОМ ! (Пистолеты)
        {            
            if (Input.GetMouseButtonDown(0))
            {
                attackActive = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                attackActive = false;
            }
        }

        //Debug.Log(isHolsted);
        //Debug.Log(activeWeaponIndex);



        if (Input.GetKeyDown(KeyCode.X))
        {
            //ToggleActiveWeapon();
        }        

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (listWeaponPistol.Count > 1)         // Если больше 1-го пистолета
            {
                listWeaponPistol[iPistol].gameObject.SetActive(false);      // Прячем действующее оружие 
                if (activeWeaponIndex == 0)     // Если актиновое оружие пистолет
                    iPistol++;                  // Добавляем счетчик
                if (iPistol >= listWeaponPistol.Count)      // Если пистолетов 2 - сбрасываем счетчик
                {
                    iPistol = 0;
                }
                listWeaponPistol[iPistol].gameObject.SetActive(true);   // Делаем оружие активным 
                Equip(listWeaponPistol[iPistol]);       // Экипируем его 
            }
            if (listWeaponPistol.Count == 1)        // Если пистолет только один
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
    }




    // weaponSlotIndex - номер типа оружия 0 - пистолеты , 1 - винотвки



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
    }


    public void Equip(RaycastWeapon newWeapon)
    {
        //Debug.Log("Equip !");
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





    void ToggleActiveWeapon()
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
        //yield return new WaitForSeconds(2);
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
    }

    IEnumerator HolsterWeapon (int index)       
    {        
        //Debug.Log("Holsted !");
        isHolsted = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", true);
            do
            {
                yield return new WaitForEndOfFrame();
                //yield return new WaitForSeconds(0.1f);      // ПЕРЕДЕЛАТЬ !
                //Debug.Log("Frame !");
                //Debug.Log(rigController.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }
            while (false);       // НЕ РАБОТАЕТ ! rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
        }
    }

    IEnumerator ActivateWeapon(int index)
    {
        Debug.Log("Active !");
        //isHolsted = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", false);
            rigController.Play("equip_" + weapon.weaponName);
            do
            {
                yield return new WaitForEndOfFrame();
                //yield return new WaitForSeconds(0.1f);      // ПЕРЕДЕЛАТЬ !
            }
            while (false);     // НЕ РАБОТАЕТ !
        }
        weapon.TakeAmmo();
        isHolsted = false;
    }
}