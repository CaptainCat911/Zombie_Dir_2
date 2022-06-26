using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextHP : MonoBehaviour
{
    Player player;          //ссылка на игрока
    AmmoPack ammoPack;      // ссылка на скрипт патронов (инвентарь)

    public Text Hp;
    public Text allBulletsText;
    public Text currentBulletsText;
    public Text enemyCountText;
    public Text enemyKilled;
    public Text currentMissionText;
    public Text currentWeaponText;
    public Text GranateText;
    public Text MedBoxText;
    public Text messageText;
    public ActiveWeapon activeWeapon;
    RaycastWeapon weapon;
    int enemyCount;
    int enemyCountKiled;

    Vector3 tempPosMessage;

    

    void Start()
    {
        player = GameManager.instance.player;
        activeWeapon = player.GetComponent<ActiveWeapon>();
        ammoPack = player.GetComponent<AmmoPack>();
        tempPosMessage = messageText.transform.position;
    }


    void Update()
    {
        // HP
        Hp.text = player.currentHealth.ToString("0");
        
        // Granate
        GranateText.text = ammoPack.granate.ToString("0");

        // MedBox
        MedBoxText.text = ammoPack.HPBox.ToString("0");


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

        // Всего убито зомби
        enemyCountKiled = GameManager.instance.enemyKilledCount;
        enemyKilled.text = enemyCountKiled.ToString("0");

        if (ammoPack.messageReady)
        {
            //StartCoroutine(Message(ammoPack.message));
            messageText.text = ammoPack.message;
            messageText.transform.position = tempPosMessage;
            ammoPack.messageReady = false;
        }

        messageText.transform.position = new Vector3(messageText.transform.position.x, messageText.transform.position.y + 1, messageText.transform.position.z);
        
    }

/*    IEnumerator Message(string messg)
    {
        // Сообщение
        *//*        GameObject go = Instantiate(messageText);     
                Text goText = go.GetComponent<Text>();
                goText.text = messg;
                yield return new WaitForSeconds(60);
                goText.text = null;*//*
        messageText.text = messg;
        yield return new WaitForSeconds(10);
        messageText.text = null;

    }*/
}
