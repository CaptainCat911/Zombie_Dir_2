using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public RaycastWeapon weaponPrefab;
    bool triggerEnter = false;
    ActiveWeapon activeWeapon;


    void Start()
    {
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




    private void Update()
    {
        if (triggerEnter)
        {
            RaycastWeapon newWeapon = Instantiate(weaponPrefab);
            activeWeapon.GetWeaponUp(newWeapon);
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            RaycastWeapon newWeapon = Instantiate(weaponPrefab);
            activeWeapon.GetWeaponUp(newWeapon);
            Destroy(gameObject);
        }
    }
}