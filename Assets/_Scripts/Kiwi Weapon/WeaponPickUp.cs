using UnityEngine;

public class WeaponPickUp : Fighter
{
    public RaycastWeapon weaponPrefab;
    //bool triggerEnter = false;
    ActiveWeapon activeWeapon;
    public bool axe;
    TooltipText tooltipText;
    public string message;
    AmmoPack playerAmmo;                        // Ссылка на пак патронов игрока


    void Start()
    {
        activeWeapon = GameManager.instance.player.GetComponent<ActiveWeapon>();
        tooltipText = GetComponent<TooltipText>();
        playerAmmo = GameManager.instance.player.GetComponent<AmmoPack>();
    }



    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.I))
        {
            GameManager.instance.player.GetComponent<ActiveWeapon>().getAxe = true;           
            RaycastWeapon newWeapon = Instantiate(weaponPrefab);
            activeWeapon.GetWeaponUp(newWeapon);
            Destroy(gameObject);
        }
    }


    protected override void Death()
    {
        PickUp();
    }


    public void PickUp()
    {
        playerAmmo.messageReady = true;                         // чтобы запустилось сообщение
        playerAmmo.message = message;

        if ( axe)
        {
            GameManager.instance.player.GetComponent<ActiveWeapon>().getAxe = true;
            GameManager.instance.player.GetComponent<ActiveWeapon>().axeBack.SetActive(true);
            Destroy(gameObject);
            return;
        }

        else
        {
            RaycastWeapon newWeapon = Instantiate(weaponPrefab);
            activeWeapon.GetWeaponUp(newWeapon);
            Destroy(gameObject);
        }
    }
}