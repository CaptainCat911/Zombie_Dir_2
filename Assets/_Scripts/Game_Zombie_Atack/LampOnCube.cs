using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampOnCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("CollideEnter !");
        if (other.tag == "Lamp")
        {
            LampControl lampControl = other.GetComponent<LampControl>();
            lampControl.lamp.SetActive(true);
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
        if (other.tag == "Lamp")
        {
            LampControl lampControl = other.GetComponent<LampControl>();
            lampControl.lamp.SetActive(false);
        }
    }
}
