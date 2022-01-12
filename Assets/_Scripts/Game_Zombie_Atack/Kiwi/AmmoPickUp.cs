
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    //public GameObject ammo;
    public string ammoType;
    public int ammoSize;
    AmmoPack playerAmmo;
    public ActiveWeapon activeWeapon;

    public void Start()
    {
        playerAmmo = GameManager.instance.player.GetComponent<AmmoPack>();
        activeWeapon = GameManager.instance.player.GetComponent<ActiveWeapon>();
    }

    public void OnTriggerStay(Collider collision)
    {
        //Debug.Log("Trig!");
        if (Input.GetKeyDown(KeyCode.E) && collision.name == "Player_Soldier")
        {
            RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
            switch (ammoType)
            {
                case "9 mm":
                    playerAmmo.allAmmo_9mm += ammoSize;
                    break;
                case "7.62":
                    playerAmmo.allAmmo_762 += ammoSize;
                    break;
                case "0.12":
                    playerAmmo.allAmmo_012 += ammoSize;
                    break;
            }
            weapon.TakeAmmo();

            Destroy(gameObject);
        }
    }
}