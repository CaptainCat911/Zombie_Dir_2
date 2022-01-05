using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour
{
    public int questNumber;
    bool triggerEnter = false;
    public void OnTriggerEnter(Collider collision)
    {
        triggerEnter = true;
    }

    public void OnTriggerExit(Collider other)
    {
        triggerEnter = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && triggerEnter)
        {
            switch (questNumber)
            {
                case 1:
                    GameManager.instance.quest1 = true;
                    Debug.Log("Вы подняли квестовый предмет №1");
                    break;
                case 2:
                    GameManager.instance.quest2 = true;
                    Debug.Log("Вы подняли квестовый предмет №2");
                    break;
                case 3:
                    GameManager.instance.quest3 = true;
                    Debug.Log("Вы подняли квестовый предмет №3");
                    break;

            }

            Destroy(gameObject);
        }
    }
}
