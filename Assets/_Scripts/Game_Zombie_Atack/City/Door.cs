using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool triggerEnter = false;
    bool doorOpen = false;
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
        if (triggerEnter && Input.GetKeyDown(KeyCode.E) && !doorOpen)
        {
            anim.SetTrigger("Open");
            StartCoroutine(OpenDoorDelay());
        }

        if (triggerEnter && Input.GetKeyDown(KeyCode.E) && doorOpen)
        {
            anim.SetTrigger("Close");
            StartCoroutine(CloseDoorDelay());
        }
    }



    IEnumerator OpenDoorDelay()
    {
        yield return new WaitForSeconds(1);
        doorOpen = true;
    }

    IEnumerator CloseDoorDelay()
    {
        yield return new WaitForSeconds(1);
        doorOpen = false;
    }
}
