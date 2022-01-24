using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnPoint : MonoBehaviour
    
{
    //public float enemySpawnPerSecond;
    public GameObject[] prefabEnemies;
    NavMeshAgent agent;
    public bool active = false;
    public int maxZombie = 50;

    public float cooldown = 1f;
    private float lastSwing;

    public float radius;



    private void Update()
    {
        if (active && Time.time - lastSwing > cooldown)
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

        lastSwing = Time.time;

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
