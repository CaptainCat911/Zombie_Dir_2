using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalSphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("CollideExit !");

/*        if (other.tag == "Enemy")
        {
            //Debug.Log("ENEMY !");
            Enemy_old enemy = other.GetComponentInParent<Enemy_old>();
            enemy.DeathFinal();
        }*/
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            //Debug.Log("ENEMY !");
            Enemy_old enemy = other.GetComponentInParent<Enemy_old>();
            enemy.DeathFinal();
        }
    }
}
