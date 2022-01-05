using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestRadio : MonoBehaviour
{
    bool questStart = false;
    bool questCompl = false;
    bool enterTrigger = false;
    int nCount = 0;

    private DialogueTrigger dialogueTrig;


    public void Start()
    {
        dialogueTrig = GetComponent<DialogueTrigger>();
    }


    public void OnTriggerEnter(Collider collision)
    {
        enterTrigger = true;   
    }

    public void OnTriggerExit(Collider collision)
    {
        enterTrigger = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !questCompl && enterTrigger)
        {
            nCount++;
            if (!questStart && !GameManager.instance.questFinish)
            {                                              
                dialogueTrig.TriggerDialogue(0);

                // написать цель и 3 состовляющие

                questStart = true;                
            }


            if (questStart && !GameManager.instance.questFinish && nCount > 1)
            {
                
                Debug.Log("Не все предметы найдены !");
                // написать, что нет квестовых предметов
            }


            if (GameManager.instance.questFinish)
            {                
                dialogueTrig.TriggerDialogue(1);

                // написать цель и таймер 

                questCompl = true;
            }
        }        
    }
}
