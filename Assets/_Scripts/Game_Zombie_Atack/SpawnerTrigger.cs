using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerTrigger : MonoBehaviour
{   
    public GameObject[] spawners;           // массив префабов с спавнерами
    bool triggerEnter = false;              // вход в триггер
    public bool trigDiffReady = true;       // для триггера 
    public bool alarm = false;
    public GameObject[] alarmGreen;
    public GameObject[] alarmRed;


    public void OnTriggerEnter(Collider collision)
    {
        if (collision.name == "Player_Soldier")
        {
            triggerEnter = true;            // вошёл в триггер 
            if (trigDiffReady)              // "заряд" триггера
            {
                GameManager.instance.MaxDifficulty();   // максимальную мощность на (60) секунд (вроде не нужно)
                trigDiffReady = false;
            }                

            if (alarm)
            {
                foreach(GameObject alarm in alarmGreen)
                {
                    alarm.SetActive(false);
                }
                foreach (GameObject alarm in alarmRed)
                {
                    alarm.SetActive(true);
                }
            }
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision.name == "Player_Soldier")
            triggerEnter = false;
    }

    private void Update()
    {
        if (triggerEnter)
        {
            foreach (GameObject spawner in spawners)
            {
                spawner.SetActive(true);                                     // активируем
            }
        }        
    }
}
