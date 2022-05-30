using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalSpawnerTrigger : MonoBehaviour
{   
    public GameObject spawnerObject;                // объект со спавнерами
    public EnemySpawnPointTrigger[] spawners;       // извлекаем из него спавнеры
    public bool trigDiffReady = false;              // для повышения сложности после входа в триггер
    public bool afterDelay = false;                 // для активации спавнеров после начальной задержки
    public bool finalTrigStart = false;             // "заряд" для старта 

    public GameObject objectLamps;                  // объек с лампами
    private Animator[] lamps;                       // извлекаем аниматоры ламп

    public GameObject spawnPointsFinalGameObject;     // финальные спавнеры
    public GameObject spawnPointsGameobject;          // обычные спавнеры, которые надо вырубить в финале
    private EnemySpawnPoint[] spawnPoints;
    public EnemySpawnPoint[] spawnPointsFinal;

    /*    bool triggerEnter = false;                  // вход в триггер



        public void OnTriggerEnter(Collider collision)
        {
            if (collision.name == "Player_Soldier")
            {
                triggerEnter = true;            // вошёл в триггер             
            }
        }

        public void OnTriggerExit(Collider collision)
        {
            if (collision.name == "Player_Soldier")
                triggerEnter = false;
        }*/


    public void Start()
    {
        spawners = spawnerObject.GetComponentsInChildren<EnemySpawnPointTrigger>();         // извлекаем из него спавнеры
        lamps = objectLamps.GetComponentsInChildren<Animator>();                            // извлекаем аниматоры ламп
        foreach (Animator lamp in lamps)
        {
            lamp.SetBool("LightOff", true);         // отключаем
        }
        //objectLamps.SetActive(false);

        spawnPoints = spawnPointsGameobject.GetComponentsInChildren<EnemySpawnPoint>();

        spawnPointsFinal = spawnPointsFinalGameObject.GetComponentsInChildren<EnemySpawnPoint>();

        spawnPointsFinalGameObject.SetActive(false);
    }


    private void Update()
    {     
        if (afterDelay)         // если начальная задержка прошла 
        {
            foreach (EnemySpawnPointTrigger spawner in spawners)
            {
                spawner.active = true;                                          // активируем
            }
        }

        if (GameManager.instance.pultActive && !finalTrigStart )    // если квесты выполнены и активировали пульт и этот квест ещё не начат
        {            
                Debug.Log("FinalStart");
                StartCoroutine(finalTrigDelay());           // запускаем коронтин
                finalTrigStart = true;                      // запускаем квест
        }
    }

    
    IEnumerator finalTrigDelay()
    {
        spawnPointsFinalGameObject.SetActive(true);
        //objectLamps.SetActive(true);                 // включаем лампы
        foreach (Animator lamp in lamps)
        {
            lamp.SetBool("LightOff", false);         // включаем лампы
        }

        GameManager.instance.SetNullDifficulty();   // ставим сложность 0, чтобы остальные зомби не спавнились (только инстант)     
        GameManager.instance.final = true;          // чтобы спавнились только обычные зомби
        GameManager.instance.mission = "Продержитесь до прибытия вертолёта";

        yield return new WaitForSeconds(30);            // начальная задержка (передышка)

        GameManager.instance.lightsOff = true;          // вырубаем в городе лампы
        GameManager.instance.SetFinalDifficulty();      // делаем финальную сложность
        GameManager.instance.FinalWave();               // запускаем время последнего ивента
        /* afterDelay = true;                              // окончание начальной задержки для спавнеров */




        yield return new WaitForSeconds(1);

        foreach (EnemySpawnPoint spawnPoint in spawnPointsFinal)
        {

            spawnPoint.maxZombie = 70;

            spawnPoint.enemyNumberSpawn = 1;

            spawnPoint.cooldown = 3;
        }


        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {

            spawnPoint.maxZombie = 0;

            spawnPoint.enemyNumberSpawn = 0;

            spawnPoint.cooldown = 1000;
        }

        yield return new WaitForSeconds(60);            // спустя .. секунд ломаем фонари 

        foreach (Animator lamp in lamps)
        {
            lamp.SetBool("LightDamaged", true);     // ломаем 
        }

        yield return new WaitForSeconds(13);

        foreach (Animator lamp in lamps)
        {
            lamp.SetBool("LightOff", true);         // отключаем
        }        
    }
}
