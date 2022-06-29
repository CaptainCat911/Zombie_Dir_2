using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffManager : MonoBehaviour
{
    public int waveNumber = 1;
    public int enemyNumberDiff;
    bool levelGo;                           // волна пошла
    bool levelStop;                         // волна остановлена
    public string message;                  // сообщение волны
    public bool messageReady;               // сообщение готово

    public bool start;    

    public void Start()
    {

    }

    public void Update()
    {
        if (GameManager.instance.enemyKilledCount >= enemyNumberDiff && levelGo)    // если убили больше .. зомби и волна запущена
        {
            GameManager.instance.SetFinalDifficultyNumber(0);           // ставим сложность 0
            levelGo = false;                                            // волна не запущена
            levelStop = true;                                           // волна остановлена

            //Debug.Log("Wave Stop (Test)");
        }


        if (GameManager.instance.enemyCount == 0 && levelStop)          // если убили всех врагов
        {
            messageReady = true;                                        // запускаем большое сообщение
            message = "Волна окончена !";                               // само сообщение
            waveNumber += 1;                                            // +1 к номеру волны
            GameManager.instance.enemyKilledCount = 0;                  // сбрасываем счётчик убийства зомби
            levelStop = false;                                          // волна не остановлена

            start = true;
        }

        if (start) 
        {
            StartCoroutine(SafeTime(waveNumber));                       // запускаем таймер до следующей 
            start = false;
            //Debug.Log("Wave Complite !");
        }
    }

    IEnumerator SafeTime(int diffLevel)
    {
        yield return new WaitForSeconds(40);

        int random = Random.Range(0, GameManager.instance.transformSpawnPoints.Length);
        Transform transform = GameManager.instance.transformSpawnPoints[random];
        GameManager.instance.npc.SetDestinationNPC(transform);

        GameManager.instance.SetFinalDifficultyNumber(diffLevel);       // устанавливаем сложность
        message = "Пошла волна !";
        messageReady = true;                                            
        yield return new WaitForSeconds(1);
        enemyNumberDiff = GameManager.instance.enemyNumberDiff;         // устанавливаем кол-во зомби для завершения волны        
        levelGo = true;                                                 // волна запущена
        
        //Debug.Log("Wave Go !");
    }

/*    IEnumerator FadeBigMessage()
    {
        yield return new WaitForSeconds(5);
        bigMessageReady = false;
    }*/
}
