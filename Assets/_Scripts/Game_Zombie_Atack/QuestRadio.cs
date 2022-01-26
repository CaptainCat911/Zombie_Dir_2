using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestRadio : MonoBehaviour
{
    bool questStart = false;            // первое взаимодействие с квестом
    bool questCompl = false;            // тру когда полностью выполнен квест
    bool enterTrigger = false;          // вход игрока в триггер
    int nCount = 0;    

    private DialogueTrigger dialogueTrig;   // ссылка на диалог 

    public GameObject[] images;             // для предметов 



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
            if (!questStart && !GameManager.instance.questFinish)           // если квест не начат и не закончен
            {                                              
                dialogueTrig.TriggerDialogue(0);                

                questStart = true;                                          // квест начат
                GameManager.instance.mission = "Найдите инструменты, транзисторы и бензин, чтобы починить радио";
            }


            if (questStart && !GameManager.instance.questFinish && nCount > 1)
            {                                
                // игнорировать, если нет квестовых предметов
            }


            if (GameManager.instance.questFinish)       // если квест закончен (собраны 3 предмета)
            {                
                dialogueTrig.TriggerDialogue(1);        // запускаем второй диалог
                foreach (GameObject image in images)    // деактивируем иконки предметовы
                {
                    image.SetActive(false);
                }

                // запустить таймер

                questCompl = true;                      // квест выполнен (больше не можем взаимодействовать)
                GameManager.instance.SetDifficulty();      // повышаем сложность
                GameManager.instance.mission = "Продержитесь : таймер";
            }
        }        
    }
}
