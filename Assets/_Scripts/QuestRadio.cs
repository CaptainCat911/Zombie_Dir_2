using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestRadio : MonoBehaviour
{
    bool questStart = false;            // первое взаимодействие с квестом
    
    bool enterTrigger = false;          // вход игрока в триггер
    int nCount = 0;    

    private DialogueTrigger dialogueTrig;   // ссылка на диалог 

    public GameObject[] images;             // для предметов 



    public void Start()
    {
        dialogueTrig = GetComponent<DialogueTrigger>();
        GameManager.instance.mission = "Найдите радиостанцию в юго-западной части города";     // пишем текст миссии
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
        if (Input.GetKeyDown(KeyCode.E) && !GameManager.instance.questCompl && enterTrigger)     // если квест не начат и игрок зашёл в триггер и нажал "е"
        {
            nCount++;
            if (!questStart && !GameManager.instance.questFinish)           // если квест не начат и не закончен
            {                                              
                dialogueTrig.TriggerDialogue(0);
                GameManager.instance.PauseWithDelay();

                questStart = true;                                          // квест начат
                GameManager.instance.questAmmo = true;
                GameManager.instance.mission = "- Найдите инструменты (супермаркет)"+ "\n- Найдите детали (военная часть)" +"\n- Найдите кабель (промышленный склад)";
            }


            if (questStart && !GameManager.instance.questFinish && nCount > 1)
            {                                
                // игнорировать, если нет квестовых предметов
            }


            if (GameManager.instance.questFinish)       // если квест закончен (собраны 3 предмета)
            {                
                dialogueTrig.TriggerDialogue(1);        // запускаем второй диалог
                GameManager.instance.PauseWithDelay();
                foreach (GameObject image in images)    // деактивируем иконки предметовы
                {
                    image.SetActive(false);
                }

                // запустить таймер

                GameManager.instance.questCompl = true;                      // квест выполнен (больше не можем взаимодействовать)
                GameManager.instance.SetDifficulty();      // повышаем сложность
                GameManager.instance.mission = "Найдите вертолётную площадку в северо-западной части города и зажгите прожекторы";
            }
        }        
    }
}
