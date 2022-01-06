using UnityEngine;
using UnityEngine.UI;

public class TextHP : MonoBehaviour
{
    Player player;  //ссылка на игрока
    public Text Hp;
    public Text allBulletsText;
    public Text currentBulletsText;
    public Text enemyCountText;
    public Text currentMissionText;
    public ActiveWeapon activeWeapon;
    RaycastWeapon weapon;
    int enemyCount;


    void Start()
    {
        player = GameManager.instance.player;   
    }

    // Update is called once per frame
    void Update()
    {
        // HP
        Hp.text = player.currentHealth.ToString("0");


        // Патроны
        weapon = activeWeapon.GetActiveWeapon();
        if (weapon)
        {
            allBulletsText.text = weapon.allAmmo.ToString("0");     // Все патроны
            currentBulletsText.text = weapon.ammoCount.ToString("0");       // Патронов в обойме    
        }


        // Всего зомбей
        enemyCount = GameManager.instance.enemyCount;
        enemyCountText.text = enemyCount.ToString("0");

        // Текущая миссия
        currentMissionText.text = GameManager.instance.mission;

    }
}
