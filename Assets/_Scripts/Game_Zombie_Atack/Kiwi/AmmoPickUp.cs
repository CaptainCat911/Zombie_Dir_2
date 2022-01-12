
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    /// <summary>
    /// Скрипт для коробки патронов
    /// </summary>
    

    //public GameObject ammo;
    public string ammoType;     // Тип патронов, называю в инспекторе
    public int ammoSize;        // Кол-во патронов, которое добавляем
    AmmoPack playerAmmo;        // Ссылка на пак патронов игрока
    public ActiveWeapon activeWeapon;

    public void Start()
    {
        playerAmmo = GameManager.instance.player.GetComponent<AmmoPack>();
        activeWeapon = GameManager.instance.player.GetComponent<ActiveWeapon>();
    }

    public void OnTriggerStay(Collider collision)
    {
        //Debug.Log("Trig!");
        if (Input.GetKeyDown(KeyCode.E) && collision.name == "Player_Soldier")  // ТУТ ПОМЕНЯТЬ
        {
            RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
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