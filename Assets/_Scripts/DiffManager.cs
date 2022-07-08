using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffManager : MonoBehaviour
{
    Player player;
    NPC npc;                                // сслыка на НПС
    public int waveNumber = 1;              // номер волны
    public int zombieToKillWave;            // кол-во зомби в волне, сколько убить надо
    public bool levelGo;                    // волна пошла
    public bool levelStop;                  // волна остановлена
    public string message;                  // сообщение волны
    public bool messageReady;               // сообщение готово
    public bool start;                      // тригер старта волны
    public int delayFirstWave;              // задержка перед началом первой волны
    public int delayWave;                   // задержка перед началом волны
    public Vector3 positionNPC;             // позиция для НПС
    public bool waveStarted;                // волна началась

    public void Start()
    {
        player = GameManager.instance.player;
        npc = GameManager.instance.npc;
    }

    public void Update()
    {
        // Почти конец волны
        if (GameManager.instance.enemyKilledCount >= zombieToKillWave && levelGo)    // если убили больше .. зомби и волна запущена
        {
            GameManager.instance.SetDifficultyNumber(0);                // ставим сложность 0
            levelGo = false;                                            // волна не запущена
            levelStop = true;                                           // волна остановлена
            GameManager.instance.noKillSphere = true;                   // спаунсфера не убивает зомби за пределами
            //Debug.Log("Wave Stop (Test)");
        }

        // Конец волны
        if (GameManager.instance.enemyCount < 3 && levelStop)          // если убили всех врагов
        {
            messageReady = true;                                        // запускаем большое сообщение
            message = "Волна окончена !";                               // само сообщение
            waveNumber += 1;                                            // +1 к номеру волны
            GameManager.instance.enemyKilledCount = 0;                  // сбрасываем счётчик убийства зомби
            waveStarted = false;                                        // волна не стартовала
            levelStop = false;                                          // волна не остановлена
            StartCoroutine(NPCdelay(2));
            //start = true;                                               // старт волны
        }

        // Начало волны
        if (start) 
        {
            StartCoroutine(SafeTime(waveNumber));                       // запускаем таймер до следующей
            start = false;                                              // сбрасываем старт
            GameManager.instance.noKillSphere = false;                  // спаунсфера убивает зомби за пределами
            Debug.Log("Wave Starting in " + delayWave + " seconds");
        }
    }

    IEnumerator SafeTime(int diffLevel)
    {
        if (waveNumber == 1)
            yield return new WaitForSeconds(delayFirstWave);            // задержка перед первой волной
        else
            yield return new WaitForSeconds(delayWave);                 // задержка перед волной

        GameManager.instance.SetDifficultyNumber(diffLevel);            // устанавливаем сложность
        if (waveNumber == 21)
            message = "Пошла волна финальная волна";                       
        else
            message = "Пошла волна №" + waveNumber;                         
        messageReady = true;                                            
        yield return new WaitForSeconds(1);
        levelGo = true;                                                 // волна запущена
        zombieToKillWave = GameManager.instance.zombieToKillWaveGM;     // устанавливаем кол-во зомби для завершения волны
        npc.SetDestinationNPC(positionNPC, false);                      // направляем НПС к точке 
        npc.mapIcon.SetActive(false);                                   // отключаем иконку НПС на карте
        //Debug.Log("Wave Go !");
        yield return new WaitForSeconds(5);                             
        npc.SetDestinationNPC(new Vector3(322, 0, -281), true);         // портуем НПС в домик
        npc.anim.SetTrigger("Dance!");
    }

    IEnumerator NPCdelay(int delayNPC)
    {
        if (waveNumber == 2)
        {
            yield return new WaitForSeconds(7);
            /*player.ammoPack.messageReady = true;
            player.ammoPack.message = "Вы чувствуете странное присутствие";*/
            GameManager.instance.dialogueTrig.TriggerDialogue(2);       // вызываем диалог торговца
            GameManager.instance.PauseWithDelay();
            npc.chasing = true;                                         // направляем НПС к игроку
        }
        else
        {
            yield return new WaitForSeconds(delayNPC);
        }
        npc.anim.SetTrigger("Stop_Dance!");
        Debug.Log(positionNPC);
        npc.SetDestinationNPC(positionNPC, true);                   // портуем НПС к следующей точке
        npc.mapIcon.SetActive(true);                                // включаем у НПС иконку на карте      
    }
}
