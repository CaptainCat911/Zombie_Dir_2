using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalSpawnerTrigger : MonoBehaviour
{   
    public GameObject spawnerObject;           // объект со спавнерами
    public EnemySpawnPointTrigger[] spawners;   // извлекаем из него спавнеры
    public bool trigDiffReady = false;       // для повышения сложности после входа в триггер
    public bool afterDelay = false;         // для активации спавнеров после начальной задержки
    public bool finalTrigStart = false;     // "заряд" для старта 

    public GameObject objectLamps;          // объек с лампами
    public Animator[] lamps;                // извлекаем аниматоры ламп

    


    public void Start()
    {
        spawners = spawnerObject.GetComponentsInChildren<EnemySpawnPointTrigger>();         // извлекаем из него спавнеры
        lamps = objectLamps.GetComponentsInChildren<Animator>();                            // извлекаем аниматоры ламп
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

        if (GameManager.instance.questFinish && GameManager.instance.pultActive && !finalTrigStart )    // если квесты выполнены и активировали пульт и этот квест ещё не начат
        {            
                Debug.Log("FinalStart");
                StartCoroutine(finalTrigDelay());           // запускаем коронтин
                finalTrigStart = true;                      // запускаем квест
        }
    }

    
    IEnumerator finalTrigDelay()
    {
        GameManager.instance.SetFinalDifficulty();   // активируем финал, чтобы остальные зомби не спавнились (только инстант)
        GameManager.instance.final = true;          // вырубаем в городе лампы
        yield return new WaitForSeconds(15);        // начальная задержка
        GameManager.instance.FinalWave();           // запускаем время последнего ивента
        afterDelay = true;                          // окончание начальной задержки для спавнеров 

        yield return new WaitForSeconds(60);        // спустя .. секунд ломаем фонари 

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
