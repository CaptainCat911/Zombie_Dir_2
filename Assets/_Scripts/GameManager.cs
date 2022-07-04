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
    public Transform[] transformSpawnPoints;    // для точек назначения НПС

    public int startDiffDelay_1;                // начальная задержка перед спауном зомби
    public int startDiffDelay_2;                // начальная задержка перед спауном зомби
    public int startDiffDelay_3;                // начальная задержка перед спауном зомби

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
    public RaycastWeapon weaponPrefabAxe;       // префаб топора, чтобы экипировать в начале

    public GameObject mapPlayerIcon;            // иконка игрока на карте

    public int enemyKilledCount = 0;            // счетчик убийства зомби

    public bool test;                           // для режима теста

    public bool mainScene = true;               // для основной сцены

    public bool noKillSphere;                   // не убивать зомби при их выходе из спавнящей сферы

    public bool zombieFreeWalk;                 // зомби идут в рандомном направлении

    public int zombieToKillWaveGM;              // кол-во зомби для режима выживания

    public bool survZombie;                     // зомби для режима выживания (триггер = 1000)

    [HideInInspector]
    public DiffManager diffManager;             // ссылка на диффменеджер

    public bool enemyAllAmmo;                   // для всех видов патронов (выживание)

    public GameObject[] menus;                  // все меню

    public NPC npc;                             // ссылка на НПС

    public int delayUp;                         // задержка когда встаёт (для меин сцены 11,5 сек)


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
        diffManager = GetComponent<DiffManager>();
        dialogueTrig = GetComponent<DialogueTrigger>();                                     // Ссылка на диалог
        spawnPoints = spawnPointsGameobject.GetComponentsInChildren<EnemySpawnPoint>();
        transformSpawnPoints = spawnPointsGameobject.GetComponentsInChildren<Transform>();

        


        if (mainScene)
            StartCoroutine(StartDiffCor());                                                 // начальная сложность, задержка
        if (test)
            StartCoroutine(ActionStart());
        if (!test)
        {
            playerStop = true;                                                              // забираем контроль
            StartCoroutine(DialogePause(delayUp));                                                 // начальный диалог 
        }
        blackScreen.SetActive(true);                                                        // включаем черный экран (на ~полсекунды)
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
            SetDifficulty();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject menu in menus)
            {
                menu.SetActive(false);                
            }
            player.aiming = true;
            Time.timeScale = 1f;
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
    

    IEnumerator DialogePause(int delay)                                  // начальный ролик и настройки
    {
        yield return new WaitForSeconds(1f);
        blackScreen.SetActive(false);
        yield return new WaitForSeconds(delay);                 // задержка пока персонаж встаёт     
        dialogueTrig.TriggerDialogue(0);                        // показываем диалог
        PauseWithDelay();                                       // паузу, чтобы было время почитать
        yield return new WaitForSeconds(1f);
        StartCoroutine(ActionStart());

        //player.activeWeapon.EquipActiveStart();
    }

    IEnumerator ActionStart()
    {
        yield return new WaitForSeconds(1f);                    
        blackScreen.SetActive(false);
        playerStop = false;                                     // отдаём контроль
        bars.SetActive(true);                                   // показываем бары
        startCinema = false;                                    // ролик завершён

        RaycastWeapon newWeapon = Instantiate(weaponPrefab);        
        player.activeWeapon.GetWeaponUp(newWeapon);
      

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

    public void SetNullDifficulty()                                     // сложность обычных спавнеров для финала
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {

            spawnPoint.maxZombie = 0;

            spawnPoint.enemyNumberSpawn = 0;

            spawnPoint.cooldown = 1000;
        }
    }




    public void SetFinalDifficultyNumber(int diffNumber)                // сложность для режима выживания
    {       
        switch (diffNumber)
        {
            case 0:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 0;

                    spawnPoint.enemyNumberSpawn = 0;

                    spawnPoint.cooldown = 10;

                    spawnPoint.mediumZombieChanse = 0;

                    spawnPoint.strongZombieChanse = 0;

                    zombieToKillWaveGM = 999;
                }
                break;

            case 1:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 20;

                    spawnPoint.enemyNumberSpawn = 1;

                    spawnPoint.cooldown = 6;

                    spawnPoint.mediumZombieChanse = 5;

                    spawnPoint.strongZombieChanse = 0;

                    spawnPoint.darkZombieChanse = 0;

                    zombieToKillWaveGM = 21;

                    diffManager.positionNPC = new Vector3(290, 0, -171);
                }
                break;

            case 2:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 30;

                    spawnPoint.enemyNumberSpawn = 1;

                    spawnPoint.cooldown = 5;

                    spawnPoint.mediumZombieChanse = 10;

                    spawnPoint.strongZombieChanse = 1;

                    zombieToKillWaveGM = 30;

                    diffManager.positionNPC = new Vector3(190, 0, -140);
                }
                break;

            case 3:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 40;

                    spawnPoint.enemyNumberSpawn = 2;

                    spawnPoint.cooldown = 4;

                    spawnPoint.mediumZombieChanse = 15;

                    spawnPoint.strongZombieChanse = 3;

                    zombieToKillWaveGM = 40;

                    diffManager.positionNPC = new Vector3(56, 0, -112);
                }
                break;

            case 4:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 50;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 3;

                    spawnPoint.mediumZombieChanse = 20;

                    spawnPoint.strongZombieChanse = 6;

                    spawnPoint.darkZombieChanse = 2;

                    zombieToKillWaveGM = 50;

                    diffManager.positionNPC = new Vector3(-10, 0, -47);
                }
                break;

            case 5:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 50;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 2;

                    spawnPoint.mediumZombieChanse = 20;

                    spawnPoint.strongZombieChanse = 5;

                    spawnPoint.darkZombieChanse = 5;

                    zombieToKillWaveGM = 60;

                    diffManager.positionNPC = new Vector3(24, 0, -167);
                }
                break;

            case 6:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 50;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 2;

                    spawnPoint.mediumZombieChanse = 25;

                    spawnPoint.strongZombieChanse = 5;

                    spawnPoint.darkZombieChanse = 10;

                    zombieToKillWaveGM = 60;

                    diffManager.positionNPC = new Vector3(74, 0, -256);
                }
                break;

            case 7:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 50;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 2;

                    spawnPoint.mediumZombieChanse = 25;

                    spawnPoint.strongZombieChanse = 10;

                    spawnPoint.darkZombieChanse = 10;

                    zombieToKillWaveGM = 60;

                    diffManager.positionNPC = new Vector3(181, 0, -205);
                }
                break;



            case 8:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 50;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 2;

                    spawnPoint.mediumZombieChanse = 30;

                    spawnPoint.strongZombieChanse = 15;

                    spawnPoint.darkZombieChanse = 5;

                    zombieToKillWaveGM = 70;

                    diffManager.positionNPC = new Vector3(257, 0, -273);
                }
                break;

            case 9:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 50;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 2;

                    spawnPoint.mediumZombieChanse = 40;

                    spawnPoint.strongZombieChanse = 20;

                    spawnPoint.darkZombieChanse = 7;

                    zombieToKillWaveGM = 70;

                    diffManager.positionNPC = new Vector3(309, 0, -322);
                }
                break;

            case 10:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 50;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 2;

                    spawnPoint.mediumZombieChanse = 50;

                    spawnPoint.strongZombieChanse = 25;

                    spawnPoint.darkZombieChanse = 5;

                    zombieToKillWaveGM = 70;

                    diffManager.positionNPC = new Vector3(347, 0, -174);
                }
                break;

            case 11:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 50;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 2;

                    spawnPoint.mediumZombieChanse = 70;

                    spawnPoint.strongZombieChanse = 30;

                    spawnPoint.darkZombieChanse = 5;

                    zombieToKillWaveGM = 80;

                    diffManager.positionNPC = new Vector3(264, 0, -45);
                }
                break;

            case 12:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 50;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 2;

                    spawnPoint.mediumZombieChanse = 80;

                    spawnPoint.strongZombieChanse = 50;

                    spawnPoint.darkZombieChanse = 5;

                    zombieToKillWaveGM = 90;

                    diffManager.positionNPC = new Vector3(141, 0, -73);
                }
                break;



            case 13:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 60;

                    spawnPoint.enemyNumberSpawn = 4;

                    spawnPoint.cooldown = 2;

                    spawnPoint.mediumZombieChanse = 0;

                    spawnPoint.strongZombieChanse = 100;

                    spawnPoint.darkZombieChanse = 5;

                    zombieToKillWaveGM = 100;

                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;
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
        PauseWithDelay();
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

    public void PauseWithDelay()
    {
        StartCoroutine(PauseDelay());
    }

    IEnumerator PauseDelay()
    {
        yield return new WaitForSeconds(0.5f);
        player.aiming = true;
        Time.timeScale = 0f;
    }
    public void Pause()
    {
        player.aiming = false;
        Time.timeScale = 0f;
    }


    public void UnPause()
    {
        player.aiming = true;
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
