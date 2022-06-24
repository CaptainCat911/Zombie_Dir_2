
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Скрипт для коробки патронов
/// </summary>

public class AmmoPickUp : Fighter
{ 

    //public GameObject ammo;
    public string ammoType;             // Тип патронов, называю в инспекторе
    public int ammoSize;                // Кол-во патронов, которое добавляем
    AmmoPack playerAmmo;                // Ссылка на пак патронов игрока
    public ActiveWeapon activeWeapon;
    //bool triggerEnter = false;
    public Animator animBox;            // ссылка на аниматор
    public Text textMessage;
    TooltipText tooltipText;
    Outline outline;
    bool opened;
    

    public void Start()
    {
        
        playerAmmo = GameManager.instance.player.GetComponent<AmmoPack>();
        activeWeapon = GameManager.instance.player.GetComponent<ActiveWeapon>();
        animBox = GetComponentInChildren<Animator>();
        tooltipText = GetComponent<TooltipText>();
        outline = GetComponent<Outline>();

        //playerAmmo.message = "Ноль";
    }


    /*    public void OnTriggerEnter(Collider collision)
        {
            if (collision.name == "Player_Soldier")
            {
                //triggerEnter = true;
                GameManager.instance.player.inRangeUse = true;
            }

        }

        public void OnTriggerExit(Collider collision)
        {
            if (collision.name == "Player_Soldier")
            {
                //triggerEnter = false;
                GameManager.instance.player.inRangeUse = false;
            }
        }*/

    protected override void Death()
    {
        if (!opened)
        {
            PickUp();
            animBox.SetTrigger("Open");
        }
    }


    public void PickUp()
    {
        //Debug.Log("Trig!");
      
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
                    playerAmmo.message = "+ патроны пистолет";
                    break;
                case "0.357":
                    playerAmmo.allAmmo_0_357 += ammoSize;
                    playerAmmo.message = "+ патроны револьвер";
                    break;
                case "5.56":
                    playerAmmo.allAmmo_5_56 += ammoSize;
                playerAmmo.message = "+ 150 патронов 5.56 мм";
                    break;
                case "0.12":
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
                    break;
                case "HPBox":
                    playerAmmo.message = "+ аптечки";
                    playerAmmo.HPBox += ammoSize;
                    break;
        }

        if (ammoType != "granate" && weapon)
            weapon.TakeAmmo();
        playerAmmo.messageReady = true;             // чтобы запустилось сообщение
        tooltipText.text = "Пусто";                 // после открытия пишем пусто в подсказке
        Destroy(tooltipText, 0);
        outline.enabled = false;                    // отключаем подсветку
        opened = true;                              // ящик открыт
        Destroy(gameObject, 60);                    // уничтожить через 60 сек

    }
}