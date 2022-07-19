using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffManager : MonoBehaviour
{
    Player player;
    NPC npc;                                // сслыка на НПС

    public int waveNumber = 1;              // номер волны
    public int waveN = 1;                   // счётчик волны

    public int zombieToKillWave;            // кол-во зомби в волне, сколько убить надо

    public bool start;                      // тригер старта волны  
    public bool levelGo;                    // волна пошла, зомби спавнятся
    public bool levelStop;                  // волна остановлена, все зомби убиты 
    public bool waveStarted;                // волна началась    


    public int delayFirstWave;              // задержка перед началом первой волны
    public int delayWave;                   // задержка перед началом волны
    public Vector3 positionNPC;             // позиция для НПС
    //[HideInInspector]
    public bool testDiff;                   // тестовый режим

    public string message;                  // сообщение волны
    public bool messageReady;               // сообщение готово

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
            levelGo = false;                                            // останавливаем спавн зомби
            levelStop = true;                                           // волна остановлена
            GameManager.instance.noKillSphere = true;                   // спаунсфера не убивает зомби за пределами
            //Debug.Log("Wave Stop (Test)");
        }

        // Конец волны
        if (GameManager.instance.enemyCount <= 0 && levelStop)          // если убили всех врагов
        {
            messageReady = true;                                        // запускаем большое сообщение
            message = "Волна окончена !";                               // само сообщение

            waveNumber += 1;                                            // +1 к номеру волны
            waveN += 1;
            if (waveNumber == 22)
            {
                StartCoroutine(AllWaveEndDelay());
                waveNumber = 1;
            }

            GameManager.instance.enemyKilledCount = 0;                  // сбрасываем счётчик убийства зомби
            waveStarted = false;                                        // волна не стартовала
            levelStop = false;                                          // сбрасываем остановку волны
            if (!npc.dead)
                StartCoroutine(NPCdelay(3));                                // вызываем задержку перед вызовом НПС

            //start = true;                                               // старт волны
        }

        // Начало волны
        if (start) 
        {
            npc.currentHealth = 10000;
            /*            if (testDiff)
                        {
                            waveNumber = 99;
                            waveN = 99;
                        }*/
            if (npc.dead)
            {
                waveNumber = 101;
                waveN = 101;
            }
            StartCoroutine(SafeTime(waveNumber));                       // запускаем таймер до следующей
            start = false;                                              // сбрасываем старт            
            GameManager.instance.noKillSphere = false;                  // спаунсфера убивает зомби за пределами
            Debug.Log("Wave Starting in " + delayWave + " seconds");
        }
    }




    // старт волны с задержкой
    IEnumerator SafeTime(int diffLevel)                               
    {
        if (waveN == 1 && !GameManager.instance.mutation)
            yield return new WaitForSeconds(delayFirstWave);            // задержка перед первой волной
        else
            yield return new WaitForSeconds(delayWave);                 // задержка перед волной

        //Debug.Log("Cor !");
        GameManager.instance.SetDifficultyNumber(diffLevel);            // устанавливаем сложность
        if (waveNumber == 101)
            message = "Волна № ???";
        else
            message = "Волна №" + waveN;                              
        messageReady = true;                       
        yield return new WaitForSeconds(1);
        
        levelGo = true;                                                 // волна запущена
        zombieToKillWave = GameManager.instance.zombieToKillWaveGM;     // устанавливаем кол-во зомби для завершения волны
        if (!npc.dead)
            npc.mapIcon.SetActive(false);                                   // отключаем иконку НПС на карте
        if (!testDiff && !npc.dead)
            npc.SetDestinationNPC(positionNPC, false);                  // направляем НПС к точке 
        //Debug.Log("Wave Go !");
        yield return new WaitForSeconds(9);                            // за это время происходит телепортация

        if (!testDiff && !npc.dead)
            npc.SetDestinationNPC(new Vector3(322, 0, -281), true);     // портуем НПС в домик
        if (!npc.dead)
            npc.anim.SetTrigger("Dance!");
    }




    // задержка появляения НПС
    IEnumerator NPCdelay(int delayNPC)                                  
    {
        if (waveN == 2 && !GameManager.instance.mutation)
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
        //Debug.Log(positionNPC);
        npc.SetDestinationNPC(positionNPC, true);                   // портуем НПС к следующей точке
        npc.mapIcon.SetActive(true);                                // включаем у НПС иконку на карте      
    }



    // задержка перед появлениям сообщения о прохождении всех волн
    IEnumerator AllWaveEndDelay()
    {
        yield return new WaitForSeconds(2);
        GameManager.instance.dialogueTrig.TriggerDialogue(3);       // вызываем диалог торговца
        GameManager.instance.PauseWithDelay();
        GameManager.instance.SetDifficultyNumber(22);               // устанавливаем сложность (включаем мутацию)
    }
}
