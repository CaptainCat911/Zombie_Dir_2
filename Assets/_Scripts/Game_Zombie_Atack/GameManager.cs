using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;         // инстанс (объект одиночка ?)

    // References
    public Player player;                       // ссылка на игрока

    // Logic
    public int pesos;
    public int experience;

    // Enemy spawner
    public int enemyCount = 0;                  // счетчик зомби

    public bool inBuilding = false;             // для изменения сферы прозрачности в здании 

    // Quest    
    public bool quest1 = false;                 // для выполненых квестов 
    public bool quest2 = false;                 
    public bool quest3 = false;
    public bool quest1Compl = false;
    public bool quest2Compl = false;
    public bool quest3Compl = false;    
    public bool questFinish = false;            // собраны 3 предмета
    public bool questCompl = false;             // тру когда полностью выполнен квест и вызван вертолёт
    public bool final = false;                  // для финального ивента (обычные спавнеры останавливаются)
    public bool lightsOff = false;              // для выключения света в городе 

    public int weakZombiesChanse;               // для изменения процента появления слабых зомби (пока не используется)

    // Диалог
    public DialogueTrigger dialogueTrig;        // диалог (для текста в начале)

    // Текущая миссия
    public string mission;                      // текст миссии на экране

    public GameObject spawnPointsGameobject;    // для управления спавнерами (ссылка на группу спавнеров)
    private EnemySpawnPoint[] spawnPoints;      // тоже 

    public int startDiffDelay;                  // начальная задержка перед спауном зомби
    public int finalDelay = 60;                  // задержка перед завершением финального ивента
    public bool pultActive = false;              // активация пульта

    public GameObject finalSpotLamp;        // прожектор вертолёта 

    int i = 0;                                  // счетчик для сложности

    bool pause = true;                     // для паузы
    bool slowMo = false;                     // для слоумоушен
        
    bool playerDead = false;                // заряд для диалога при поражении

    public bool playerStop = true;                // для обездвижевания

    public GameObject bars;                     // для включения баров

    public bool postProcessFinal = false;





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
        spawnPoints = spawnPointsGameobject.GetComponentsInChildren<EnemySpawnPoint>(); 
        StartCoroutine(StartDiffCor());
        StartCoroutine(DialogePause());
        playerStop = true;
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //SaveState();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            slowMo = !slowMo;
            if (slowMo)
                Time.timeScale = 0.3f;
            if (!slowMo)
                Time.timeScale = 1f;
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            //SetDifficulty();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {            
            pause = !pause;
            if (pause)
               Time.timeScale = 1f;
            if (!pause)
                Time.timeScale = 0f;
        }



        // Квест
        if (quest1 && !quest1Compl)             // после выполнения квеста
        {
            SetDifficulty();                    // добавляем сложность
            quest1Compl = true;                 // этот квест выполнен

            // в целях поменать на +
        }
        if (quest2 && !quest2Compl)
        {
            SetDifficulty();
            quest2Compl = true;

            // в целях поменать на +
        }
        if (quest3 && !quest3Compl)
        {
            SetDifficulty();
            quest3Compl = true;
        }

        if (quest1 && quest2 && quest3)
        {
            //SetDifficulty();
            questFinish = true;         // собраны 3 предмета             
        }


        if (!player.isAlive && !playerDead)
        {
            StartCoroutine(DialogePauseDeath());
            playerDead = true;
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\



    public void FinalWave()
    {        
        StartCoroutine(FinalDelay());
    }

    IEnumerator FinalDelay()
    {
        yield return new WaitForSeconds(finalDelay);            // сколько длится финальный ивент        

        //Debug.Log("Wave!");
        finalSpotLamp.SetActive(true);                          // включаем прожектор вертолёта 
        yield return new WaitForSeconds(0.5f);                 
        player.FinalWave();                                     // запускаем волну, убивающую зомби (стрельба из вертолёта)
        yield return new WaitForSeconds(10f);
        dialogueTrig.TriggerDialogue(1);
        Pause();
        postProcessFinal = true;
    }   




    IEnumerator StartDiffCor()
    {
        //Debug.Log("Cor!");
        yield return new WaitForSeconds(startDiffDelay);
        SetDifficultyStart();
    }


    public void SetDifficultyStart()             // установление начальной сложности 
    {
        //Debug.Log("Set!");
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            
                spawnPoint.maxZombie = 30;
            
                spawnPoint.enemyNumberSpawn = 1;
            
                spawnPoint.cooldown = 8;
        }
    }



    public void SetDifficulty()             // увеличение сложности 
    {
        i++;
        //Debug.Log("Set!");
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {            
            spawnPoint.maxZombie += 5;

            if (i < 2)
                spawnPoint.enemyNumberSpawn += 1;
            
            spawnPoint.cooldown -= 1;
        }
    }

    public void MaxDifficulty()             // временное увеличение сложности на ивентах
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            spawnPoint.maxZombie += 10;
            spawnPoint.enemyNumberSpawn += 2;
            spawnPoint.cooldown -= 2;
        }
        StartCoroutine(MaxDiff());
    }

    IEnumerator MaxDiff()           // отключаем повышение сложности через .. секунд
    {
        yield return new WaitForSeconds(60);
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            spawnPoint.maxZombie -= 10;
            spawnPoint.enemyNumberSpawn -= 2;
            spawnPoint.cooldown += 2;
        }
    }

    public void SetFinalDifficulty()
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {

            spawnPoint.maxZombie = 70;

            spawnPoint.enemyNumberSpawn = 2;

            spawnPoint.cooldown = 8;
        }
    }

    public void SetNullDifficulty()
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {

            spawnPoint.maxZombie = 0;

            spawnPoint.enemyNumberSpawn = 0;

            spawnPoint.cooldown = 1000;
        }
    }


    IEnumerator DialogePause()
    {
        //playerStop = true;     
                
        yield return new WaitForSeconds(9f);               
        dialogueTrig.TriggerDialogue(0);        
        Pause();
        yield return new WaitForSeconds(1f);
        playerStop = false;
        bars.SetActive(true);
    }

    IEnumerator DialogePauseDeath()
    {
        yield return new WaitForSeconds(4f);
        dialogueTrig.TriggerDialogue(2);
        Pause();
        
    }


    public void Pause()
    {
        StartCoroutine(PauseDelay());
    }

    IEnumerator PauseDelay()
    {
        yield return new WaitForSeconds(0.5f);
        player.aiming = true;
        Time.timeScale = 0f;
    }


    public void UnPause()
    {
        Time.timeScale = 1f;
    }










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
