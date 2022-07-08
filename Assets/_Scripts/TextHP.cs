using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextHP : MonoBehaviour
{
    Player player;          //ссылка на игрока
    AmmoPack ammoPack;      // ссылка на скрипт патронов (инвентарь)
    DiffManager diffManager;

    public ActiveWeapon activeWeapon;
    RaycastWeapon weapon;
        
    public Text soulsText;                  // кол-во душ
    public Text soulsTextMagazine;          // кол-во душ для магазина
    public Text Hp;                         // здоровье
    public Text armor;                      // броня
    public Text allBulletsText;             // всего патронов
    public Text currentBulletsText;         // патронов в обойме
    public Text enemyCountText;             // сколько зомби на карте
    public Text enemyKilled;                // убито зомби
    public Text currentMissionText;         // текущая миссия
    public Text currentWeaponText;          // оружие в руках
    public Text GranateText;                // кол-во гранат
    public Text MedBoxText;                 // кол-во аптечек
    public Text messageText;                // сообщение справа (при подъеме патронов)
    public Text bigMessageText;             // большое сообщение (волна!)
    public Text WaveText;                   // волна №
    public GameObject armorBar;             // бар армора

    int enemyCount;
    int enemyCountKiled;

    Vector3 tempPosMessage;
    Vector3 tempPosBigMessage;

    Animator animHP;

    

    void Start()
    {
        diffManager = GameManager.instance.diffManager;
        player = GameManager.instance.player;
        activeWeapon = player.GetComponent<ActiveWeapon>();
        ammoPack = player.GetComponent<AmmoPack>();
        tempPosMessage = messageText.transform.position;
        tempPosBigMessage = bigMessageText.transform.position;
        animHP = Hp.GetComponent<Animator>();
    }


    void Update()
    {
        // Души
        soulsText.text = ammoPack.souls.ToString("0");
        soulsTextMagazine.text = soulsText.text;

        // Волна
        WaveText.text = GameManager.instance.diffManager.waveNumber.ToString("0");

        // HP
        Hp.text = player.currentHealth.ToString("0");
        if (player.currentHealth <= 25)
            animHP.SetBool("HPlow", true);
        else
            animHP.SetBool("HPlow", false);

        // Броня
        armor.text = player.armor.ToString("0");
        if (player.armor > 0)
            armorBar.SetActive(true);
        else
            armorBar.SetActive(false);



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

        if (ammoPack.messageReady)                                  // если сообщение готово (в аммопаке)
        {
            //StartCoroutine(Message(ammoPack.message));
            messageText.text = ammoPack.message;                    // берем текст из сообщения в аммопаке
            messageText.transform.position = tempPosMessage;        // возвращаем сообщение в начальную позицию
            ammoPack.messageReady = false;                          // сообщение не готово
        }
        messageText.transform.position = new Vector3(messageText.transform.position.x, messageText.transform.position.y + 1, messageText.transform.position.z);             // поднимаем вверх сообщение


        if (diffManager.messageReady)
        {
            //StartCoroutine(Message(ammoPack.message));
            bigMessageText.text = diffManager.message;
            bigMessageText.transform.position = tempPosBigMessage;
            diffManager.messageReady = false;
        }
        bigMessageText.transform.position = new Vector3(bigMessageText.transform.position.x, bigMessageText.transform.position.y + 1, bigMessageText.transform.position.z);
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
