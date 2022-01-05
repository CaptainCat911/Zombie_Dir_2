using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int bulletDamage = 1;

    public Rigidbody rigid;

    [SerializeField]
    //private WeaponType _type;
  

/////////////////////////////////////////////////////////////////////////////////////////////


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }


//-------------------------------------------------------------------------------------------------------------------------------------?
    
    
    void Update()
    {
           
    }


    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Fighter")
        {
            if (coll.name == "Player" || coll.name == "Swoard")
                return;

            Damage dmg = new Damage()
            {
                damageAmount = bulletDamage,
                origin = transform.position,
                pushForce = 0
            };

            coll.SendMessage("ReceiveDamage", dmg);
            Destroy(this.gameObject);

            //Debug.Log(coll.name);            
        }
    }



    /*
    ////////////////////////////////////// Свойство type ///////////////////////////////////////


    // Это общедоступное свойство маскирует поле _type и обрабатывает
    // операции присваивания ему нового значения
    public WeaponType type
    {
        get
        {
            return (_type);
        }

        set
        {
            SetType(value);
        }
    }


    /// <summary>
    /// Изменяет скрытое поле _type и устанавливает цвет этого снаряда;
    /// как определено в WeaponDefinition.
    ///</summary>
    ///<param пате= "еТуре" > Тип WeaponType используемого оружия.</param>
    public void SetType(WeaponType eType)
    { 
      // Установить _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition( _type);
        rend.material.color = def.projectileColor;
    }
    */
}
