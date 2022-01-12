using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Player : Mover
{
        // Делегат
    /*    public delegate void DamageTaken();
        public DamageTaken damageTakeCallback;*/




    private bool isAlive = true;

        // Для прицела
    public Transform pointer;          // прицел       
    bool aiming = true;    // прицеливание
    //Quaternion qua1;       // поворот
    int layerMask = 1 << 10;     // маска для прицела, игнорирует всё кроме 10 слоя
    //int layerMaskCam = 1 << 11;     // маска для прицела, игнорирует всё кроме 11 слоя

    // Передвижение
    const float locomationAnimationSmoothTime = .1f; // сглаживание бега
    Vector3 motorVect;

        // Animation Info    
    private Animator anim;

        // Оружие
    ActiveWeapon activeWeapon;      // ссылка на активное оружие    

    //Renderer rend = null;


    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    protected override void Start()
    {
        base.Start();
        
        pointer = transform.Find("Sphere_Aim").gameObject.GetComponent<Transform>();
        anim = GetComponent<Animator>();        
        activeWeapon = GetComponent<ActiveWeapon>();
        //layerMask = ~layerMask;
        //layerMaskCam = ~ layerMaskCam;

/*        damageTakeCallback += weapon.AttackRange;
        damageTakeCallback += TakeHit;   */


        //EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
    }




//-------------------------------------------------------------------------------------------------------------------------------------\\

    /*
    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        // Add new modifiers
        if (newItem != null)
        {
            armor.AddModifier(newItem.armorModifier);
            damage.AddModifier(newItem.damageModifier);
            pushForce.AddModifier(newItem.pushForceModifier);
            //weaponTypePlayer = newItem.weaponType.
            weaponTypePlayer = newItem.wTypeTest;
            //weaponPrefabPlayer = newItem.prefab;
            weapon.CreateWeapon(newItem.prefab);
        }

        // Remove old modifiers
        if (oldItem != null)
        {
            armor.RemoveModifier(oldItem.armorModifier);
            damage.RemoveModifier(oldItem.damageModifier);
            pushForce.RemoveModifier(oldItem.pushForceModifier);
        }

    }
    */

    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive)
            return;

        base.ReceiveDamage(dmg);
        TakeHit();

        /*GameManager.instance.OnHitpointChange();*/
    }

    public void TakeHit()
    {
        anim.SetTrigger("Take_Hit");
    }



    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\



    private void FixedUpdate()
    {
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

        if (isAlive)
        {              
            if ((activeWeapon.attackActive == true && !activeWeapon.isHolsted) || Input.GetKey(KeyCode.LeftControl))
            {
                motorVect.x = motorVect.x * 0.3f;
                motorVect.z = motorVect.z * 0.3f;

                anim.SetBool("Walk", true);
                UpdateMotor(motorVect);
            }
            else
            {
                anim.SetBool("Walk", false);
                UpdateMotor(motorVect);
            }
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
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
    }


    protected override void Death()
    {
        isAlive = false;
        Destroy(this.gameObject);
        //GameManager.instance.deathMenuAnim.SetTrigger("Show");
    }


}