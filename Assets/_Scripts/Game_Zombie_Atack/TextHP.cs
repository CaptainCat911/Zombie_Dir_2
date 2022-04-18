using UnityEngine;
using UnityEngine.UI;

public class TextHP : MonoBehaviour
{
    Player player;          //ссылка на игрока
    AmmoPack ammoPack;      // ссылка на скрипт патронов (инвентарь)

    public Text Hp;
    public Text allBulletsText;
    public Text currentBulletsText;
    public Text enemyCountText;
    public Text currentMissionText;
    public Text currentWeaponText;
    public Text GranateText;
    public ActiveWeapon activeWeapon;
    RaycastWeapon weapon;
    int enemyCount;

    

    void Start()
    {
        player = GameManager.instance.player;
        ammoPack = player.GetComponent<AmmoPack>();
    }


    void Update()
    {
        // HP
        Hp.text = player.currentHealth.ToString("0");
        
        // Granate
        GranateText.text = ammoPack.granate.ToString("0");


        // Патроны
        weapon = activeWeapon.GetActiveWeapon();
        if (weapon)
        {
            allBulletsText.text = weapon.allAmmo.ToString("0");             // Все патроны
            currentBulletsText.text = weapon.ammoCount.ToString("0");       // Патронов в обойме
            currentWeaponText.text = weapon.textNameWeapon;                 // оружие в руках
        }


        // Всего зомбей
        enemyCount = GameManager.instance.enemyCount;
        enemyCountText.text = enemyCount.ToString("0");

        // Текущая миссия
        currentMissionText.text = GameManager.instance.mission;

    }
}
