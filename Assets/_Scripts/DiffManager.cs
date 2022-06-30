using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffManager : MonoBehaviour
{
    NPC npc;                                // сслыка на НПС
    public int waveNumber = 1;              // номер волны
    public int zombieToKillWave;            // кол-во зомби в волне, сколько убить надо
    bool levelGo;                           // волна пошла
    bool levelStop;                         // волна остановлена
    public string message;                  // сообщение волны
    public bool messageReady;               // сообщение готово
    public bool start;                      // тригер старта волны
    public int delayWave;                   // задержка перед началом волны

    public void Start()
    {
        npc = GameManager.instance.npc;
    }

    public void Update()
    {
        if (GameManager.instance.enemyKilledCount >= zombieToKillWave && levelGo)    // если убили больше .. зомби и волна запущена
        {
            GameManager.instance.SetFinalDifficultyNumber(0);           // ставим сложность 0
            levelGo = false;                                            // волна не запущена
            levelStop = true;                                           // волна остановлена
            GameManager.instance.noKillSphere = true;                   // спаунсфера не убивает зомби за пределами

            //Debug.Log("Wave Stop (Test)");
        }


        if (GameManager.instance.enemyCount == 0 && levelStop)          // если убили всех врагов
        {
            messageReady = true;                                        // запускаем большое сообщение
            message = "Волна окончена !";                               // само сообщение
            waveNumber += 1;                                            // +1 к номеру волны
            GameManager.instance.enemyKilledCount = 0;                  // сбрасываем счётчик убийства зомби
            levelStop = false;                                          // волна не остановлена

            int random = Random.Range(0, GameManager.instance.transformSpawnPoints.Length);
            Transform transform = GameManager.instance.transformSpawnPoints[random];
            npc.SetDestinationNPC(transform.position, true);            // отправляем НПС к рандомному спавнеру

            npc.mapIcon.SetActive(true);                                // включаем у НПС иконку на карте
            start = true;                                               // старт волны
        }

        if (start) 
        {
            StartCoroutine(SafeTime(waveNumber));                       // запускаем таймер до следующей 
            start = false;                                              // сбрасываем старт
            GameManager.instance.noKillSphere = false;                  // спаунсфера убивает зомби за пределами
            Debug.Log("Level Starting in " + delayWave + " seconds");
        }
    }

    IEnumerator SafeTime(int diffLevel)
    {
        yield return new WaitForSeconds(delayWave);                     // задержка перед волной
        GameManager.instance.SetFinalDifficultyNumber(diffLevel);       // устанавливаем сложность
        message = "Пошла волна №" + waveNumber;
        messageReady = true;                                            
        yield return new WaitForSeconds(1);
        levelGo = true;                                                 // волна запущена
        zombieToKillWave = GameManager.instance.zombieToKillWaveGM;        // устанавливаем кол-во зомби для завершения волны 
        npc.SetDestinationNPC(new Vector3(323,0,-280), false);          // направляем НПС в домик
        npc.mapIcon.SetActive(false);                                   // отключаем иконку НПС на карте
        yield return new WaitForSeconds(5);
        npc.SetDestinationNPC(new Vector3(323, 0, -280), true);         // портуем НПС в домик

        //Debug.Log("Wave Go !");
    }

/*    IEnumerator FadeBigMessage()
    {
        yield return new WaitForSeconds(5);
        bigMessageReady = false;
    }*/
}
