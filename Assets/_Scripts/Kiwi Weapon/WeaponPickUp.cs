using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public RaycastWeapon weaponPrefab;
    bool triggerEnter = false;
    ActiveWeapon activeWeapon;
    public bool axe;


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
        if (triggerEnter && axe)
        {
            GameManager.instance.player.GetComponent<ActiveWeapon>().getAxe = true;
            GameManager.instance.player.GetComponent<ActiveWeapon>().axeBack.SetActive(true);
            Destroy(gameObject);
            return;
        }

        if (triggerEnter)
        {
            RaycastWeapon newWeapon = Instantiate(weaponPrefab);
            activeWeapon.GetWeaponUp(newWeapon);
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
/*            GameManager.instance.player.GetComponent<ActiveWeapon>().getAxe = true;
            GameManager.instance.player.GetComponent<ActiveWeapon>().axeBack.SetActive(true);
            RaycastWeapon newWeapon = Instantiate(weaponPrefab);
            activeWeapon.GetWeaponUp(newWeapon);
            Destroy(gameObject);*/
        }
    }
}