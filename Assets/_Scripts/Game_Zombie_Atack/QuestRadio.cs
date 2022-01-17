using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestRadio : MonoBehaviour
{
    bool questStart = false;
    bool questCompl = false;
    bool enterTrigger = false;
    int nCount = 0;    

    private DialogueTrigger dialogueTrig;   // ссылка на диалог 

    public GameObject[] images;



    public void Start()
    {
        dialogueTrig = GetComponent<DialogueTrigger>();
        GameManager.instance.mission = "Найдите радио";     // пишем текст миссии
    }


    public void OnTriggerEnter(Collider collision)
    {
        if (collision.name == "Player_Soldier")
            enterTrigger = true;   
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision.name == "Player_Soldier")
            enterTrigger = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !questCompl && enterTrigger)     // если квест не начат и игрок зашёл в триггер и нажал "е"
        {
            nCount++;
            if (!questStart && !GameManager.instance.questFinish)   
            {                                              
                dialogueTrig.TriggerDialogue(0);                

                questStart = true;
                GameManager.instance.mission = "Найдите инструменты, транзисторы и бензин, чтобы починить радио";
            }


            if (questStart && !GameManager.instance.questFinish && nCount > 1)
            {                                
                // игнорировать, если нет квестовых предметов
            }


            if (GameManager.instance.questFinish)
            {                
                dialogueTrig.TriggerDialogue(1);        // запускаем второй диалог
                foreach (GameObject image in images)    // деактивируем иконки предметовы
                {
                    image.SetActive(false);
                }

                // запустить таймер

                questCompl = true;
                GameManager.instance.mission = "Продержитесь : таймер";
            }
        }        
    }
}
