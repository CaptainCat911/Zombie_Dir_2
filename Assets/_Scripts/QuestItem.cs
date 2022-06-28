using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
    public int questNumber;
    bool triggerEnter = false;
    private DialogueTrigger dialogueTrig;
    public GameObject image1;

    void Start()
    {
        dialogueTrig = GetComponent<DialogueTrigger>();
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
        if (Input.GetKeyDown(KeyCode.E) && triggerEnter)
        {
            switch (questNumber)
            {
                case 1:
                    GameManager.instance.quest1 = true;                    
                    break;
                case 2:
                    GameManager.instance.quest2 = true;                    
                    break;
                case 3:
                    GameManager.instance.quest3 = true;                    
                    break;
                case 4:
                    if (GameManager.instance.questCompl)
                    {
                        GameManager.instance.pultActive = true;
                        dialogueTrig.TriggerDialogue(0);
                        GameManager.instance.PauseWithDelay();
                        Destroy(this);
                    }
                    break;
            }
            
            
            if (questNumber != 4)
            {
                image1.SetActive(true);
                Destroy(gameObject);
                dialogueTrig.TriggerDialogue(0);
            }          
                

        }
    }
}
