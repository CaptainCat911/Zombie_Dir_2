using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSphere : MonoBehaviour
{
     
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("CollideEnter !");
        if (other.tag == "EnemySpawner")
        {
            EnemySpawnPoint sp = other.GetComponent<EnemySpawnPoint>();
            sp.active = true;                   
        }
        


        /*        if (other.tag == "Enemy")
                {
                    Debug.Log("ENEMY !");
                    Enemy_old enemy = other.GetComponentInParent<Enemy_old>();
                    enemy.Kill();
                }*/
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("CollideExit !");
        if (other.tag == "EnemySpawner")
            other.GetComponent<EnemySpawnPoint>().active = false;

        if (other.tag == "Enemy")
        {
            //Debug.Log("ENEMY !");
            Enemy_old enemy = other.GetComponentInParent<Enemy_old>();              
            enemy.Kill();        
        }
    }
}
