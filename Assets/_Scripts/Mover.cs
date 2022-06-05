using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : Fighter
{       
    protected Vector3 moveDelta;
    //protected RaycastHit2D hit;
    public float ySpeed = 7.0f;
    public float xSpeed = 7.0f;

    protected virtual void Start()   
    {
        
                
    }

    protected virtual void UpdateMotor( Vector3 input)
    {
        // Вектор для движения
        moveDelta = new Vector3(input.x * xSpeed, 0, input.z * ySpeed);

        //Add push vector, if any
        moveDelta += pushDirection;

        // Reduce push force enery frame, based off recovery speed
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

        transform.Translate(moveDelta.x * Time.deltaTime, 0 , moveDelta.z * Time.deltaTime, Space.World);
    }
}  