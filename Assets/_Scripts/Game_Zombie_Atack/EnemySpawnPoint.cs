using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
    
{
    public float enemySpawnPerSecond;
    public GameObject prefabEnemy;



    private void Start()
    {
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

    }
    public void SpawnEnemy()
    {
        if (GameManager.instance.enemyCount >= 100)
        {
            return;
        }
        
        //Debug.Log(GameManager.instance.enemyCount);
        //int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemy);
        GameManager.instance.enemyCount += 1;


        //if (go.GetComponent<BoundsCheck>() != null)
        // {
        //     enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        // }


        go.transform.position = transform.position;

        // Снова вызвать SpawnEnemy
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond); 
    }
}
