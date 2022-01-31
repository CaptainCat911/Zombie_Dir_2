using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollide : MonoBehaviour
{

    //public GameObject roof;


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player_Soldier")
        { 
            //roof.SetActive(false);
            GameManager.instance.inBuilding = true;
            //Debug.Log("Enter !");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player_Soldier")
        {
            //roof.SetActive(true);
            GameManager.instance.inBuilding = false;
            //Debug.Log("Exit !");
        }
    }

/*    private void OnTriggerStay(Collider other)
    {
        
        Debug.Log("Stay !");
    }*/
}
