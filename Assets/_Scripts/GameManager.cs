using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;         // инстанс (объект одиночка ?)

    // References
    public Player player;                       // ссылка на игрока

    // Enemy spawner
    [HideInInspector]
    public int enemyCount = 0;                  // счетчик зомби
    [HideInInspector]
    public bool inBuilding = false;             // для изменения сферы прозрачности в здании 

    // Quest    
    [HideInInspector]
    public bool questAmmo = false;              // для выпадения патронов на все оружия
    [HideInInspector]
    public bool quest1 = false;                 // для выполненых квестов 
    [HideInInspector]
    public bool quest2 = false;
    [HideInInspector]
    public bool quest3 = false;
    [HideInInspector]
    public bool quest1Compl = false;
    [HideInInspector]
    public bool quest2Compl = false;
    [HideInInspector]
    public bool quest3Compl = false;
    [HideInInspector]
    public bool questFinish = false;            // собраны 3 предмета
    
    public bool questCompl = false;             // тру когда полностью выполнен квест и вызван вертолёт
    
    public bool final = false;                  // для финального ивента (обычные спавнеры останавливаются)

    [HideInInspector]
    public bool lightsOff = false;              // для выключения света в городе 

    [HideInInspector]
    public int weakZombiesChanse;               // для изменения процента появления слабых зомби (пока не используется)

    // Диалог
    public DialogueTrigger dialogueTrig;        // диалог (для текста в начале)

    // Текущая миссия
    public string mission;                      // текст миссии на экране

    public GameObject spawnPointsGameobject;    // для управления спавнерами (ссылка на группу спавнеров)
    private EnemySpawnPoint[] spawnPoints;      // тоже 

    public int startDiffDelay_1;                  // начальная задержка перед спауном зомби
    public int startDiffDelay_2;                  // начальная задержка перед спауном зомби
    public int startDiffDelay_3;                  // начальная задержка перед спауном зомби

    public int finalDelay = 60;                  // задержка перед завершением финального ивента (сколько он длится)
    public bool pultActive = false;              // активация пульта

    public GameObject finalSpotLamp;            // прожектор вертолёта 

    int i = 0;                                  // счетчик для сложности

    bool pause = false;                          // для паузы
    bool slowMo = false;                        // для слоумоушен
        
    public bool playerDead = false;             // заряд для диалога при поражении

    public bool playerStop = false;             // для обездвижевания
        
    public GameObject bars;                     // для включения баров

    [Header("Засветление")]
    [Tooltip("Начинает засветлять экран")]
    public bool postProcessFinal = false;       // для "засветления" в финале

    public GameObject tempCam;                  // камера при старте для карты
    public GameObject tempLight;                // свет при старте для карты

    public GameObject canvasMap;                // Карта
    bool mapActive = false;                     // вкл/выкл карту

    public GameObject deathMenu;                // меню при поражении

    public GameObject pauseMenu;                // меню паузы

    bool startCinema = true;                    // для начального ролика

    public GameObject blackScreen;              // черный экран для начала игры

    public RaycastWeapon weaponPrefab;          // префаб пистолета, чтобы экипировать в начале

    public GameObject mapPlayerIcon;            // иконка игрока на карте

    public int enemyKilledCount = 0;            // счетчик убийства зомби

    public bool test;                           // для режима теста





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
        dialogueTrig = GetComponent<DialogueTrigger>();                                     // Ссылка на диалог
        spawnPoints = spawnPointsGameobject.GetComponentsInChildren<EnemySpawnPoint>(); 
        StartCoroutine(StartDiffCor());                                                     // начальная сложность, задержка
        if (!test)
        {
            StartCoroutine(DialogePause());                                                 // ПОТОМ ВКЛЮЧИТЬ начальная сложность
            playerStop = true;                                                              // ПОТОМ ВКЛЮЧИТЬ вызов диалога
            blackScreen.SetActive(true);                                                    // включаем черный экран (на ~полсекунды)
        }
        tempCam.SetActive(true);                                                            // для карты
        tempLight.SetActive(true);                                                          // для карты
        StartCoroutine(TempCamDelay());                                                     // для карты
        mapPlayerIcon.SetActive(true);                                                      // включаем иконку игрока на карте

    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\




    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //SaveState();
        }

        // Слоумоушион нах
        if (Input.GetKeyDown(KeyCode.U))
        {
            slowMo = !slowMo;
            if (slowMo)
                Time.timeScale = 0.3f;
            if (!slowMo)
                Time.timeScale = 1f;
        }        


        // Пауза
        if (Input.GetKeyDown(KeyCode.Escape) && !playerDead && !startCinema)
        {            
            if (pause)
            {
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                playerStop = false;
                pause = false;
            }
                
            else
            {
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                playerStop = true;
                pause = true;
            }
        }
      

        // Карта
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!player.isAlive || playerStop || startCinema)
            {
                return;
            }

            mapActive = !mapActive;

            if (mapActive)
            {
                canvasMap.SetActive(true);
                //Time.timeScale = 0f;
            }
            if (!mapActive)
            {
                canvasMap.SetActive(false);
                //Time.timeScale = 1f;
            }
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


        // Если проиграл
        if (!player.isAlive && !playerDead)
        {
            StartCoroutine(DialogePauseDeath());
            playerDead = true;
        }
    }



    //-------------------------------------------------Начальные ролик и настройки--------------------------------------------------------------------------------------------------\\

    IEnumerator TempCamDelay()                                  // задержка для выключения камеры карты
    {
        yield return new WaitForSeconds(0.1f);            
        tempCam.SetActive(false);
        tempLight.SetActive(false);
    }
    

    IEnumerator DialogePause()                                  // начальный ролик и настройки
    {
        yield return new WaitForSeconds(1f);
        blackScreen.SetActive(false);
        yield return new WaitForSeconds(11.5f);                 // задержка пока персонаж встаёт     
        dialogueTrig.TriggerDialogue(0);                        // показываем диалог
        Pause();                                                // паузу, чтобы было время почитать
        yield return new WaitForSeconds(1f);
        playerStop = false;                                     // отдаём контроль
        bars.SetActive(true);                                   // показываем бары
        startCinema = false;                                    // ролик завершён

        RaycastWeapon newWeapon = Instantiate(weaponPrefab);
        player.activeWeapon.GetWeaponUp(newWeapon);

        //player.activeWeapon.EquipActiveStart();
    }


    public void HidePauseMenu()                                 // для кнопки "продолжить" в меню паузы
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        playerStop = false;
        pause = false;
    }


    //--------------------------------------- Управление сложностью --------------------------------------------------\\


    IEnumerator StartDiffCor()      // постепенное увеличение сложности
    {
        //Debug.Log("Cor!");
        yield return new WaitForSeconds(startDiffDelay_1);
        SetDifficultyStart();
        yield return new WaitForSeconds(startDiffDelay_2);
        SetDifficulty();
        yield return new WaitForSeconds(startDiffDelay_3);
        SetDifficulty();
    }


    public void SetDifficultyStart()             // установление начальной сложности 
    {
        //Debug.Log("Set!");
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            
                spawnPoint.maxZombie = 9;
            
                spawnPoint.enemyNumberSpawn = -1;
            
                spawnPoint.cooldown = 12;
        }
    }


    public void SetDifficulty()                              // увеличение сложности 
    {
        i++;
        //Debug.Log("Set!");
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {            
            spawnPoint.maxZombie += 7;

            if (i < 2)
                spawnPoint.enemyNumberSpawn += 1;
            
            spawnPoint.cooldown -= 1;
        }
    }

    public void MaxDifficulty()                             // временное увеличение сложности на ивентах
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            spawnPoint.maxZombie += 10;
            spawnPoint.enemyNumberSpawn += 2;
            spawnPoint.cooldown -= 2;
        }
        StartCoroutine(MaxDiffOff());
    }

    IEnumerator MaxDiffOff()                                   // отключаем повышение сложности через .. секунд
    {
        yield return new WaitForSeconds(60);
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            spawnPoint.maxZombie -= 10;
            spawnPoint.enemyNumberSpawn -= 2;
            spawnPoint.cooldown += 2;
        }
    }

    public void SetFinalDifficulty()                        // финальная сложность
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {

            spawnPoint.maxZombie = 50;

            spawnPoint.enemyNumberSpawn = 2;

            spawnPoint.cooldown = 8;
        }
    }

    public void SetNullDifficulty()                         // сложность обычных спавнеров для финала
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {

            spawnPoint.maxZombie = 0;

            spawnPoint.enemyNumberSpawn = 0;

            spawnPoint.cooldown = 1000;
        }
    }



    //-----------------------------------Финальный ивент------------------------------------------------------\\



    public void FinalWave()                 // запуск финального ивента 
    {
        StartCoroutine(FinalDelay());
    }


    IEnumerator FinalDelay()
    {
        yield return new WaitForSeconds(finalDelay);            // сколько длится финальный ивент        

        //Debug.Log("Wave!");
        finalSpotLamp.SetActive(true);                          // включаем прожектор вертолёта 
        yield return new WaitForSeconds(1f);
        player.FinalWave();                                     // запускаем волну, убивающую зомби (стрельба из вертолёта)
        yield return new WaitForSeconds(15f);
        dialogueTrig.TriggerDialogue(1);
        Pause();
        postProcessFinal = true;
    }



    //-----------------------------------При поражении-------------------------------------------------\\


    IEnumerator DialogePauseDeath()
    {
        yield return new WaitForSeconds(4f);        
        deathMenu.SetActive(true);
        //Pause();        
    }



    //------------------------------------Пауза-----------------------------------------------------\\

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
