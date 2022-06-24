using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUpSphere : MonoBehaviour
{
    public AmmoPickUp ammo;

    public void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "Weapon")
        {
            ammo = collision.GetComponent<AmmoPickUp>();
            if (ammo)
            {
                GameManager.instance.player.inRangeUse = true;
                //Debug.Log("Enter!");
            }
        }
        else
        {
            GameManager.instance.player.inRangeUse = false;
            ammo = null;
        }
    }

    public void Update()
    {
        //Debug.Log(GameManager.instance.player.inRangeUse);
    }
}
