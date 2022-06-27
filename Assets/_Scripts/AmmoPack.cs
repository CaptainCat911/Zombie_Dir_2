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

    public string message;

    public bool messageReady;




    public void GiveAmmo (string ammoType)
    {
        switch (ammoType)
        {
            case "9":
                allAmmo_9 += 50;                
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
}

