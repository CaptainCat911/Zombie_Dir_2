using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public RaycastWeapon weaponPrefab;

    public void OnTriggerStay(Collider collision)
    {
        //Debug.Log("Trig!");
        if (Input.GetKeyDown(KeyCode.E))
        {

            ActiveWeapon activeWeapon = collision.gameObject.GetComponent<ActiveWeapon>();
            if (activeWeapon)
            {
                RaycastWeapon newWeapon = Instantiate(weaponPrefab);
                activeWeapon.GetWeaponUp(newWeapon);
            }
            Destroy(gameObject);
        }
    }
}