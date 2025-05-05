using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneCube : MonoBehaviour
{      

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        if (other.tag == "Enemy")
        {
            Debug.Log("ENEMY !");
            Enemy_old enemy = other.GetComponentInParent<Enemy_old>();
            if (enemy)
                enemy.Kill();
        }
    }
}
