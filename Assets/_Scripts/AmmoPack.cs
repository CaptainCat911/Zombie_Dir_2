using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    Player player;                          // сслыка на игрока
    public RaycastWeapon[] weapons;         // массив оружий
             

    int weaponNumber;                       // переменная для выбора оружия

    public int souls = 0;

    public int allAmmo_9 = 0;
    public int allAmmo_0_357 = 0;
    public int allAmmo_5_56 = 0;
    public int allAmmo_0_12 = 0;
    public int allAmmo_7_62 = 0;
    public int allAmmo_0_50 = 0;
    public int granate = 0;
    public int HPBox = 0;

    public string message;                  // сообщение при подборе патронов
    public bool messageReady;               // сообщение готово


    // Стоимость патронов
    [Header("Стоимость патронов")]
    public int ammoSoulsPistol;
    public int ammoSoulsAR;
    public int ammoSoulsRevolver;
    public int ammoSoulsShotgun;
    public int ammoSoulsSvd;
    public int ammoSoulsPulemet;
    public int ammoSoulsHp;
    public int ammoSoulsGranate;

    // Стоимость оружия
    [Header("Стоимость оружия")]
    public int WeaponSoulsAR;
    public int WeaponSoulsRevolver;
    public int WeaponSoulsShotgun;
    public int WeaponSoulsSVD;
    public int WeaponSoulsPulemet;

    // Стоимость апгрейда оружия
    [Header("Стоимость апгрейда оружия")]
    public int WeaponSoulsPistolUpgreade;
    public int WeaponSoulsARUpgreade;
    public int WeaponSoulsRevolverUpgreade;
    public int WeaponSoulsShotgunUpgreade;
    public int WeaponSoulsSVDUpgreade;
    public int WeaponSoulsPulemetUpgreade;

    // Оружие экипировано
    bool aR;
    bool shotgun;
    bool revolver;
    bool svd;
    bool pulemet;



    public void Start()
    {
        player = GameManager.instance.player;
    }


    public void GiveAmmo (string ammoType)
    {
        switch (ammoType)
        {
            case "9":
                if (souls >= ammoSoulsPistol)
                {
                    allAmmo_9 += 40;
                    souls -= ammoSoulsPistol;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
                break;

            case "5.56":
                if (souls >= ammoSoulsAR)
                {
                    allAmmo_5_56 += 100;
                    souls -= ammoSoulsAR;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
                break;


            case "0.12":
                if (souls >= ammoSoulsShotgun)
                {
                    allAmmo_0_12 += 52;
                    souls -= ammoSoulsShotgun;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
                break;

            case "0.357":
                if (souls >= ammoSoulsRevolver)
                {
                    allAmmo_0_357 += 36;
                    souls -= ammoSoulsRevolver;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
                break;

            case "0.50":
                if (souls >= ammoSoulsSvd)
                {
                    allAmmo_0_50 += 30;
                    souls -= ammoSoulsSvd;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
                break;

            case "7.62":
                if (souls >= ammoSoulsPulemet)
                {
                    allAmmo_7_62 += 100;
                    souls -= ammoSoulsPulemet;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
                break;

            case "hp":
                if (souls >= ammoSoulsHp)
                {
                    HPBox += 1;
                    souls -= ammoSoulsHp;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
                break;

            case "Granate":
                if (souls >= ammoSoulsGranate)
                {
                    granate += 1;
                    souls -= ammoSoulsGranate;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
                break;



                /*            case "0.12":
                                playerAmmo.allAmmo_0_12 += ammoSize;
                                playerAmmo.message = "+ патроны дробовик";
                                break;
                            case "7.62":
                                playerAmmo.allAmmo_7_62 += ammoSize;
                                playerAmmo.message = "+ патроны пулемет";
                                break;
                            case "0.50":
                                playerAmmo.allAmmo_0_50 += ammoSize;
                                playerAmmo.message = "+ патроны СВД";
                                break;
                            case "granate":
                                playerAmmo.message = "+ гранаты";
                                playerAmmo.granate += ammoSize;
                                break;*/
        }
    }

    public void GiveWeapon (string weapon)
    {        
        switch (weapon)
        {
            case "AR":
                if (aR)                                 // если винтовка уже есть
                {
                    SendToMessage("Уже куплено !");
                    return;
                }
                if (souls >= WeaponSoulsAR && !aR)      // если душ больше чем цена оружия и его нет
                {
                    weaponNumber = 0;                   // номер префаба
                    aR = true;                          // оружие купили
                    souls -= WeaponSoulsAR;             // забираем цену оружия из общих душ
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                    return;
                }
                break;

            case "Shotgun":
                if (shotgun)
                {
                    SendToMessage("Уже куплено !");
                    return;
                }
                if (souls >= WeaponSoulsShotgun && !shotgun)
                {
                    weaponNumber = 1;
                    shotgun = true;
                    souls -= WeaponSoulsShotgun;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                    return;
                }
                break;

            case "Revolver":
                if (revolver)
                {
                    SendToMessage("Уже куплено !");
                    return;
                }
                if (souls >= WeaponSoulsRevolver && !revolver)
                {
                    weaponNumber = 2;
                    revolver = true;
                    souls -= WeaponSoulsRevolver;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                    return;
                }
                break;

            case "SVD":
                if (svd)
                {
                    SendToMessage("Уже куплено !");
                    return;
                }
                if (souls >= WeaponSoulsSVD && !svd)
                {
                    weaponNumber = 3;
                    svd = true;
                    souls -= WeaponSoulsSVD;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                    return;
                }
                break;

            case "Pulemet":
                if (pulemet)
                {
                    SendToMessage("Уже куплено !");
                    return;
                }
                if (souls >= WeaponSoulsPulemet && !pulemet)
                {
                    weaponNumber = 4;
                    pulemet = true;
                    souls -= WeaponSoulsPulemet;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                    return;
                }
                break;
        }

        RaycastWeapon newWeapon = Instantiate(weapons[weaponNumber]);
        player.activeWeapon.GetWeaponUp(newWeapon);
        
    }

    public void UpgradeWeapon(string weapon)
    {
        //Debug.Log(player.activeWeapon.listWeapons);
        foreach (RaycastWeapon w in player.activeWeapon.listWeapons)
        {
            // Пистолет
            if (w.weaponName == weapon && weapon == "pistol")
            {
                if (souls >= WeaponSoulsPistolUpgreade)
                {
                    w.rayDamage += 8;
                    w.clipSize += 10;
                    souls -= WeaponSoulsPistolUpgreade;

                    Debug.Log("Pistol Upgraded");
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
            }

            // AR
            if (w.weaponName == weapon && weapon == "rifle")
            {
                if (souls >= WeaponSoulsARUpgreade)
                {
                    w.rayDamage += 10;
                    w.clipSize += 20;
                    souls -= WeaponSoulsARUpgreade;

                    Debug.Log("AR Upgraded");
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
            }

            // Револьвер
            if (w.weaponName == weapon && weapon == "revolver")
            {
                if (souls >= WeaponSoulsRevolverUpgreade)
                {
                    w.rayDamage += 50;
                    w.clipSize += 3;
                    souls -= WeaponSoulsRevolverUpgreade;

                    Debug.Log("Revolver Upgraded");
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
            }

            // Дробовик
            if (w.weaponName == weapon && weapon == "shotgun")
            {
                if (souls >= WeaponSoulsShotgunUpgreade)
                {
                    w.rayDamage += 5;
                    w.clipSize += 8;
                    souls -= WeaponSoulsShotgunUpgreade;

                    Debug.Log("Shotgun Upgraded");
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
            }

            // СВД
            if (w.weaponName == weapon && weapon == "sniper")
            {
                if (souls >= WeaponSoulsSVDUpgreade)
                {
                    w.rayDamage += 150;
                    w.clipSize += 5;
                    souls -= WeaponSoulsSVDUpgreade;

                    Debug.Log("SVD Upgraded");
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
            }

            // Пулемет
            if (w.weaponName == weapon && weapon == "heavy")
            {
                if (souls >= WeaponSoulsPulemetUpgreade)
                {
                    w.rayDamage += 20;
                    w.clipSize += 50;
                    souls -= WeaponSoulsPulemetUpgreade;

                    Debug.Log("Pulemet Upgraded");
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
            }
        }
    }

    public void EffectSmokeActivate(string weapon)
    {
        foreach (RaycastWeapon w in player.activeWeapon.listWeapons)
        {
            if (w.weaponName == weapon)
            {
                w.effectSmoke.SetActive(true);
            }
        }
    }



    public void SendToMessage(string messageToSend)
    {
        message = messageToSend;
        messageReady = true;
    }

}

