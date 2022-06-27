using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffManager : MonoBehaviour
{
    public int waveNumber = 1;
    public int enemyNumberDiff;
    public bool levelGo;                    // волна пошла
    public bool levelStop;                  // волна остановлена
    public string message;           // сообщение волны
    public bool messageReady;


    public void Start()
    {
        //GameManager.instance.SetFinalDifficultyNumber(0);             // ставим сложность 0 при старте        
        StartCoroutine(SafeTime(waveNumber));                           // запускаем таймер до волны
        
    }

    public void Update()
    {
        if (GameManager.instance.enemyKilledCount >= enemyNumberDiff && levelGo)      // если убили больше .. зомби 
        {
            GameManager.instance.SetFinalDifficultyNumber(0);           // ставим сложность 0
            levelGo = false;
            levelStop = true;
            //Debug.Log("Wave Stop (Test)");
        }

        if (GameManager.instance.enemyCount == 0 && levelStop)            // если убили всех врагов
        {
            waveNumber += 1;
            GameManager.instance.enemyKilledCount = 0;                  // сбрасываем счётчик убийства зомби
            levelStop = false;
            StartCoroutine(SafeTime(waveNumber));                       // запускаем таймер до следующей волны            
            messageReady = true;
            message = "Волна окончена !";
            //Debug.Log("Wave Complite !");
        }
    }

    IEnumerator SafeTime(int diffLevel)
    {
        yield return new WaitForSeconds(20);
        GameManager.instance.SetFinalDifficultyNumber(diffLevel);       // устанавливаем сложность
        message = "Пошла волна !";
        yield return new WaitForSeconds(1);
        enemyNumberDiff = GameManager.instance.enemyNumberDiff;         // устанавливаем кол-во зомби для завершения волны        
        levelGo = true;
        messageReady = true;
        //Debug.Log("Wave Go !");
    }

/*    IEnumerator FadeBigMessage()
    {
        yield return new WaitForSeconds(5);
        bigMessageReady = false;
    }*/
}
