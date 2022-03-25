using UnityEngine;


public class Player : Mover
{
    public bool isAlive = true;

        // Для прицела
    public Transform pointer;          // прицел       
    public bool aiming = true;    // прицеливание
    //Quaternion qua1;       // поворот
    int layerMask = 1 << 10;     // маска для прицела, игнорирует всё кроме 10 слоя
    //int layerMaskCam = 1 << 11;     // маска для прицела, игнорирует всё кроме 11 слоя

    // Передвижение
    const float locomationAnimationSmoothTime = .1f; // сглаживание бега
    public Vector3 motorVect;
    public bool slowed = false;     // для замедления от зомби
                                    // для замедления
    public float cooldownSlow = 2f;
    public float lastSlow;

    // Animation Info    
    private Animator anim;

        // Оружие
    ActiveWeapon activeWeapon;      // ссылка на активное оружие   

    float stopForce = 0;            // сила замедления от зомби

    public float maxSpeed = 6;
       
    public Animator finalSphereAnim;    // сфера, убивающая зомби, для финального ивента (типо их с пулемета вертолёта расстреляли)

    public Light lightF;                 // для управления фонари

    public GameObject playerChest;      // для зомби

    bool boostSpeed = false;            // Для тестового режима 

    public GameObject canvasMap;
    bool mapActive = false;



    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    protected override void Start()
    {
        base.Start();
        
        pointer = transform.Find("Sphere_Aim").gameObject.GetComponent<Transform>();
        //finalSphere = transform.Find("Final Sphere").gameObject.GetComponent<FinalSphere>();
        

        anim = GetComponent<Animator>();        
        activeWeapon = GetComponent<ActiveWeapon>();
        //layerMask = ~layerMask;
        //layerMaskCam = ~ layerMaskCam;
    }



    protected override void UpdateMotor(Vector3 input)
    {
        if (!isAlive || GameManager.instance.playerStop)
            return;
        // Вектор для движения
        moveDelta = new Vector3(input.x * xSpeed, 0, input.z * ySpeed);


        //Add push vector, if any
        //moveDelta += pushDirection;

        // Reduce push force enery frame, based off recovery speed
        //pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

        transform.Translate(moveDelta.x * Time.deltaTime, 0, moveDelta.z * Time.deltaTime, Space.World);

    }


    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive)
            return;

        base.ReceiveDamage(dmg);
        stopForce = dmg.pushForce;
        anim.SetTrigger("Take_Hit");

        /*GameManager.instance.OnHitpointChange();*/
    }




    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\



    private void FixedUpdate()
    {
        if (!isAlive || GameManager.instance.playerStop)
            return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        motorVect.x = x;
        motorVect.z = z;

            // Альтернативный вариант 
        /*
                float xAngle = motorVect.z * -(Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad)) + motorVect.x * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad);
                float zAngle = motorVect.z * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad) + motorVect.x * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad);
        */

        float velocityX = Vector3.Dot(motorVect.normalized, transform.right);
        float velocityZ = Vector3.Dot(motorVect.normalized, transform.forward);


        anim.SetFloat("speedPlayerX", velocityX, locomationAnimationSmoothTime, Time.deltaTime);
        anim.SetFloat("speedPlayerZ", velocityZ, locomationAnimationSmoothTime, Time.deltaTime);

        if (motorVect.magnitude > 1f)
            motorVect.Normalize();

          
             

        if ((activeWeapon.attackActive == true && !activeWeapon.isHolsted) || Input.GetKey(KeyCode.LeftControl) || slowed)
        {
            if (xSpeed > 2.5f)
            {
                xSpeed *= 0.93f;
                ySpeed *= 0.93f;
            }

            //anim.SetBool("Walk", true);
            UpdateMotor(motorVect);
        }

        else
        {
            //anim.SetBool("Walk", false);
            UpdateMotor(motorVect);
        }


        // Перезарядка замедления
        if (Time.time - lastSlow > cooldownSlow)
        {
            slowed = false;            
        }

        if (slowed)
        {
            xSpeed *= stopForce;           // скорость замедления 
            ySpeed *= stopForce;
        }

        if (!slowed)
        {
            if (xSpeed < maxSpeed)
            {
                xSpeed *= 1.03f;           // скорость восстановления после замедления
                ySpeed *= 1.03f;
            }            
        }

        if (xSpeed < 1f)
        {
            xSpeed = 1f;            // минимальная скорость
            ySpeed = 1f;
        }

        anim.SetFloat("Speed", xSpeed, locomationAnimationSmoothTime, Time.deltaTime);
        //Debug.Log(xSpeed);


        if (boostSpeed)
        {
            xSpeed = 20f;
            ySpeed = 20f;
            maxSpeed = 20f;
            currentHealth = 100;
        }


        /*
       // Для разворота без прицеливания

       if (aiming == false && (x != 0 ||  z != 0) && !weapon.attacking)
       {          
               // Находим угол             
           float angle = Mathf.Atan2(motorVect.x, motorVect.z) * Mathf.Rad2Deg;            

               // Устанавливаем этот угол
           qua1 = Quaternion.Euler(transform.eulerAngles.x, angle, transform.eulerAngles.z);

               // Делаем Lerp      
           transform.rotation = Quaternion.Lerp(transform.rotation, qua1, Time.fixedDeltaTime * 15f);
       }
       */
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //transform.Rotate(0, 1, 0, Space.Self);
            if (aiming == false)
                aiming = true;
            else
                aiming = false;
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            boostSpeed = !boostSpeed;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            currentHealth += 100;            
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isAlive || GameManager.instance.playerStop)
            {                
                return;
            }

            lightF.enabled = !lightF.enabled;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isAlive || GameManager.instance.playerStop)
            {
                return;
            }
            mapActive = !mapActive;
            if (mapActive)
            {
                canvasMap.SetActive(true);
                Time.timeScale = 0f;
            }
            if (!mapActive)
            {
                canvasMap.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        if (!isAlive || GameManager.instance.playerStop)
        {
            lightF.enabled = false;
            return;
        }


       
            


        //-------------------------- Прицел -----------------------\\
        //Ray ray1 = new Ray(transform.position, transform.forward);
        Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(transform.position, transform.forward * 100f, Color.yellow);
        RaycastHit hit;
        if (Physics.Raycast(ray1, out hit, Mathf.Infinity, layerMask))
        {
            /*            var direction = hit.point - transform.position;
                        direction.y = 0f;
                        direction.Normalize();
                        transform.forward = direction;*/

            pointer.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);

            //hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.yellow;           
            
            //Material mat1 = hit.collider.gameObject.GetComponent<Renderer>().material;            
            //ChangeAlpha(mat1, 0.5f);
        }              

        if (aiming)         // или weapon.attacking
        {
            // Находим угол 
            Vector3 lookDir = pointer.position - transform.position;
            float angle = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
            //float angleRound = Mathf.Round(angle);

            // Устанавливаем этот угол
            Quaternion qua1 = Quaternion.Euler(transform.eulerAngles.x, angle, transform.eulerAngles.z);

            // Делаем Lerp      
            transform.rotation = Quaternion.Lerp(transform.rotation, qua1, Time.fixedDeltaTime * 15f);            
        }







        //-------------------------- Скрывать объекты мешающие камере -----------------------\\
        //Vector3 playerPos = new Vector3(0, transform.position.x, transform.position.z);
        //Ray rayCam = Camera.main.ScreenPointToRay(playerPos);
        //Ray rayCam1 = Camera.main.ScreenToWorldPoint (transform.position);
        // RaycastHit hitCam;


        /*       bool rayBool = Physics.Linecast(Camera.main.transform.position, this.transform.position, out hitCam, layerMaskCam);
               if (rayBool)
               *//*        {
                           MeshRenderer meshRenderer = hitCam.collider.gameObject.GetComponent<MeshRenderer>();
                           Material[] materials = meshRenderer.materials;
                           foreach (Material mat in materials)
                           {
                               Color tempColor = mat.color;
                               tempColor.a = 0.5f;
                               mat.color = tempColor;
                           }
                       }

                       if (rayBool == false)
                       {

                       }*//*



               {
                   rend = hitCam.collider.gameObject.GetComponent<Renderer>();
                   rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
               }

               if (rayBool == false)
               {
                   if (rend == null)
                       return;
                   rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
               }
       */

    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\



    public void FinalWave()
    {
        finalSphereAnim.SetTrigger("Final");
    }


    
/*    public void ChangeAlpha(Material mat, float alphaValue)
    {
        Color oldColor = mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaValue);
        mat.SetColor("_Color", newColor);
    }*/


    

    public void SwapSprite(int skinId)
    {
        //spriteRenderer.sprite = GameManager.instance.playerSprites[skinId];
    }



    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    /*
        public void OnLevelUp()
        {
            maxHealth++;
            currentHealth = maxHealth;
            GameManager.instance.OnHitpointChange();
        }
    */


    /*
        public void SetLevel (int level)
        {
            for (int i = 0; i < level; i++)
            {
                OnLevelUp();
            }
        }
    */


    public void Heal(int healingAmount)
    {
        if (currentHealth == maxHealth)
            return;

        currentHealth += healingAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        //GameManager.instance.ShowText("+" + healingAmount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.5f);
        //GameManager.instance.OnHitpointChange();
    }

    public void Respawn()
    {
        Heal(maxHealth);
        isAlive = true;
        //lastImmune = Time.time;
        pushDirection = Vector3.zero;
    }


    protected override void Death()
    {
        isAlive = false;
        anim.SetTrigger("Death");
        //Destroy(this.gameObject);
        //GameManager.instance.deathMenuAnim.SetTrigger("Show");
    }
}