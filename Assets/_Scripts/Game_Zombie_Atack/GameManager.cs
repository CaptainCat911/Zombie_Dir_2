using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;    

    // References
    public Player player;

    // Logic
    public int pesos;
    public int experience;

    // Enemy spawner
    public int enemyCount = 0;

    public bool inBuilding = false;

    // Quest
    public bool quest1 = false;
    public bool quest2 = false;
    public bool quest3 = false;
    public bool questFinish = false;

    // Диалог
    public DialogueTrigger dialogueTrig;



    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
/*            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            Destroy(eventSys);*/

            return;
        }
        // присваем instance (?) этому обьекту и по ивенту загрузки запускаем функцию загрузки
        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;

        
    }
        

    public void Start()
    {
        dialogueTrig = GetComponent<DialogueTrigger>();     // Ссылка на диалог        
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveState();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            dialogueTrig.TriggerDialogue(0);            
        }

        // Квест
        if (quest1 == true)
        {
            // в целях поменать на +
        }
        if (quest2 == true)
        {
            // в целях поменать на +
        }
        if (quest3 == true)
        {
            // в целях поменать на +
        }

        if (quest1 && quest2 && quest3)
        {
            questFinish = true;
        }


    }

//---------------------------------------------------------------------------------------------------------------------------------------------------------\\

    // Floating text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
       
    }
 

//---------------------------------------------------------------------------------------------------------------------------------------------------------\\



//---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    // HitpointBar
    public void OnHitpointChange()
    {

    }


//---------------------------------------------------------------------------------------------------------------------------------------------------------\\





//---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    public void OnLevelUp()
    {
        //player.OnLevelUp();
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\

    /*
     * 
     * 
     * 
     */

    // On Scene Loaded

    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
       /* player.transform.position = GameObject.Find("SpawnPoint").transform.position;*/
    }


    // Death Menu and Respawn
    public void Respawn()
    {
/*        deathMenuAnim.SetTrigger("Hide");
        SceneManager.LoadScene("Main");
        player.Respawn();*/
    }

    public void SaveState()
    {
/*        string s = "";

        s += "0" + "|";
        s += pesos.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString();

        // записываем строку s в PlayerPrefs
        PlayerPrefs.SetString("SaveState", s);*/
        
    }


    public void LoadState(Scene s, LoadSceneMode mode)
    {
/*        SceneManager.sceneLoaded -= LoadState;
        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        // вытаскиваем строку s из PlayerPrefs 
        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        // Change player skin
        pesos = int.Parse(data[1]);

        // Experience
        experience = int.Parse(data[2]);
        if (GetCurrentLevel() != 1)
            player.SetLevel(GetCurrentLevel());

        // Change the weapon Level
        weapon.SetWeaponLevel( int.Parse(data[3]));

        */
    }


}
