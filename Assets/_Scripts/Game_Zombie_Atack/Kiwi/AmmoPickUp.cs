
using UnityEngine;
/// <summary>
/// Скрипт для коробки патронов
/// </summary>

public class AmmoPickUp : MonoBehaviour
{

    

    //public GameObject ammo;
    public string ammoType;     // Тип патронов, называю в инспекторе
    public int ammoSize;        // Кол-во патронов, которое добавляем
    AmmoPack playerAmmo;        // Ссылка на пак патронов игрока
    public ActiveWeapon activeWeapon;
    bool triggerEnter = false;

    public void Start()
    {
        playerAmmo = GameManager.instance.player.GetComponent<AmmoPack>();
        activeWeapon = GameManager.instance.player.GetComponent<ActiveWeapon>();
    }
    

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.name == "Player_Soldier")
            triggerEnter = true;
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision.name == "Player_Soldier")
            triggerEnter = false;
    }




    public void Update()
    {
        //Debug.Log("Trig!");
        if (triggerEnter)  
        {
            RaycastWeapon weapon = activeWeapon.GetActiveWeapon();

            /*            playerAmmo.allAmmo_9 += 60 * ammoSizeMag;
                        playerAmmo.allAmmo_5_56 += 100 * ammoSizeMag;
                        playerAmmo.allAmmo_0_12 += 28 * ammoSizeMag;
                        playerAmmo.allAmmo_0_357 += 30 * ammoSizeMag;
                        playerAmmo.allAmmo_7_62 += 50 * ammoSizeMag;
                        playerAmmo.allAmmo_0_50 += 20 * ammoSizeMag;*/


            switch (ammoType)
            {
                case "9":
                    playerAmmo.allAmmo_9 += ammoSize;
                    break;
                case "0.357":
                    playerAmmo.allAmmo_0_357 += ammoSize;
                    break;
                case "5.56":
                    playerAmmo.allAmmo_5_56 += ammoSize;
                    break;
                case "0.12":
                    playerAmmo.allAmmo_0_12 += ammoSize;
                    break;
                case "7.62":
                    playerAmmo.allAmmo_7_62 += ammoSize;
                    break;
                case "0.50":
                    playerAmmo.allAmmo_0_50 += ammoSize;
                    break;

            }
            weapon.TakeAmmo();

            Destroy(gameObject);
        }
    }
}