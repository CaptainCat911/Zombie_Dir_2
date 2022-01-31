using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerTrigger : MonoBehaviour
{   
    public GameObject[] spawners;      // массив префабов с спавнерами
    bool triggerEnter = false;

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.name == "Player_Soldier")
            triggerEnter = true;
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
                EnemySpawnPointTrigger spawnerTrig = spawner.GetComponent<EnemySpawnPointTrigger>();
                spawnerTrig.active = true;
            }
        }

    }
}
