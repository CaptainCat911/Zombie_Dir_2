using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampControl : MonoBehaviour
{
    public Animator anim;
    public bool lightDamaged = false;
    public bool lightGood;
    public bool finalLamp = false;



    void Start()
    {
        if (lightGood)
            return;
        int random = Random.Range(0, 100);
        if (random > 95 || lightDamaged)
        {
            anim.SetBool("LightDamaged", true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            anim.SetBool("LightOff", false);
        }



        if (Input.GetKeyDown(KeyCode.N) || GameManager.instance.final && !finalLamp)
        {
            anim.SetBool("LightOff", true);
        }
    }
}
