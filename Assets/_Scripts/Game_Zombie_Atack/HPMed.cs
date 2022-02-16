using UnityEngine;
/// <summary>
/// Скрипт для аптечки
/// </summary>

public class HPMed : MonoBehaviour
{

    bool triggerEnter = false;

    public void Start()
    {

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




    public void Update()
    {        
        if (Input.GetKeyDown(KeyCode.E) && triggerEnter)  
        {
            if (GameManager.instance.player.currentHealth != GameManager.instance.player.maxHealth)
            {
                GameManager.instance.player.Heal(25);
                Destroy(gameObject);
            }            
        }
    }
}