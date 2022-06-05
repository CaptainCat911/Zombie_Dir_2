using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnPoint : MonoBehaviour
    
{
    //public float enemySpawnPerSecond;
    public GameObject[] prefabEnemies;      // массив префабов с зомби
    NavMeshAgent agent;
    public bool active = false;             // активация этого спавнера
    public int maxZombie = 50;              // максимальное кол-во зомби 
    public bool fewZombiesReady = true;     // для вызова нескольких зомби при активации
    public int enemyNumberSpawn = 1;            // кол-во зомби при активации
    public float cooldown = 1f;             // перезарядка спауна
    private float lastSpawn;

    public float radius;                    // радиус для спавна зомби за пределеами видимости игрока





    private void Update()
    {
        if (active && fewZombiesReady)
        {
            for (int i = 0; i < enemyNumberSpawn; i++)
            {
                SpawnEnemy();
            }
            fewZombiesReady = false;
        }

        if (!active)
            fewZombiesReady = true;

        if (active && Time.time - lastSpawn > cooldown)
        {
            float dist = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);  
            if (dist > radius)
            {            
                SpawnEnemy();                
            }
        }
    }




    public void SpawnEnemy()
    {
        if (GameManager.instance.enemyCount >= maxZombie)
        {
            return;
        }

        lastSpawn = Time.time;

        //Debug.Log(GameManager.instance.enemyCount);        
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]);// Создаём префаб   

        go.transform.SetParent(transform, false);           // Назначаем этот спавнер родителем
        agent = go.GetComponent<NavMeshAgent>();            // Находим НавМешАгент
        agent.Warp(transform.position);                     // Перемещаем префаб к спавнеру

        GameManager.instance.enemyCount += 1;    

        // Снова вызвать SpawnEnemy
/*        if (active)
            Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);*/ 
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
