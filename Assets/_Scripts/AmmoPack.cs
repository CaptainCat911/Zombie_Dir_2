using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour
{
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

    public RaycastWeapon[] weapons;         // массив оружий
    Player player;                          // сслыка на игрока
    int weaponNumber;                       // переменная для выбора оружия

    // Стоимость патронов
    public int pistolAmmoSouls;

    // Стоимость оружия
    public int aRWeaponSouls;
    public int shotgunWeaponSouls;

    // Оружие экипировано
    bool AR;
    bool Shotgun;



    public void Start()
    {
        player = GameManager.instance.player;
    }


    public void GiveAmmo (string ammoType)
    {
        switch (ammoType)
        {
            case "9":
                if (souls >= pistolAmmoSouls)
                {
                    allAmmo_9 += 50;
                    souls -= pistolAmmoSouls;
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                }
                break;

            case "5.56":
                allAmmo_5_56 += 100;                
                break;

            case "0.357":
                allAmmo_0_357 += 36;                
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
            case "HPBox":                
                HPBox += 1;
                break;
        }
    }

    public void GiveWeapon (string weapon)
    {        
        switch (weapon)
        {
            case "AR":
                if (AR)                                 // если винтовка уже есть
                {
                    SendToMessage("Уже куплено !");
                    return;
                }
                if (souls >= aRWeaponSouls && !AR)      // если душ больше чем цена оружия и его нет
                {
                    weaponNumber = 0;                   // номер префаба
                    AR = true;                          // оружие купили
                    souls -= aRWeaponSouls;             // забираем цену оружия из общих душ
                }
                else
                {
                    SendToMessage("Недостаточно душ");
                    return;
                }
                break;

            case "Shotgun":
                if (Shotgun)
                {
                    SendToMessage("Уже куплено !");
                    return;
                }
                if (souls >= shotgunWeaponSouls && !Shotgun)
                {
                    weaponNumber = 1;
                    Shotgun = true;
                    souls -= shotgunWeaponSouls;
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

    public void SendToMessage(string messageToSend)
    {
        message = messageToSend;
        messageReady = true;
    }
}

