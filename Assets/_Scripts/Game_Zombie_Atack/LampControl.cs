using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampControl : MonoBehaviour
{
    public Animator anim;
    public bool lightDamaged = false;       // мигающий свет
    public bool lightGood;                  // хороший свет
    public bool finalLamp = false;          // финальная лампа
    public GameObject lamp;
        


    void Start()
    {
        if (lightGood)
            return;
        if (lightDamaged)
        {
            lamp.SetActive(true);
            anim.SetBool("LightDamaged", true);
            lamp.SetActive(false);
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            //anim.SetBool("LightOff", false);
        }



        if (GameManager.instance.lightsOff && !finalLamp)
        {
            //anim.SetBool("LightOff", true);
            lamp.SetActive(false);
        }
    }
}
