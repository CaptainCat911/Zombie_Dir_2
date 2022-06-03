using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool triggerEnter = false;
    bool doorOpen = false;
    bool readyDoor = true;
    Animator anim;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.name == "Player_Soldier")
            triggerEnter = true;
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision.name == "Player_Soldier")
            triggerEnter = false;
    }


    private void Update()
    {
        if (triggerEnter && Input.GetKeyDown(KeyCode.E) && !doorOpen && readyDoor)
        {
            anim.SetTrigger("Open");
            doorOpen = true;
            readyDoor = false;
            StartCoroutine(OpenDoorDelay());
        }

        if (triggerEnter && Input.GetKeyDown(KeyCode.E) && doorOpen && readyDoor)
        {
            anim.SetTrigger("Close");
            doorOpen = false;
            readyDoor = false;
            StartCoroutine(OpenDoorDelay());
        }
    }



    IEnumerator OpenDoorDelay()
    {
        yield return new WaitForSeconds(1);
        readyDoor = true;
    }


}
