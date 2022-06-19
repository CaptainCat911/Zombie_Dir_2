using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Transform lookAt;           // сделать private
    public float boundX = 0f;
    public float boundZ = 0f;
    public float zCorrect;

    private void Start()
    {
        lookAt = GameManager.instance.player.transform; 
    }

    void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        float deltaX = lookAt.position.x - transform.position.x;
        if( deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
            {
                delta.x = deltaX - boundX;
            }
        else
            {
                delta.x = deltaX + boundX;
            }
        }

        float deltaZ = lookAt.position.z - transform.position.z ;
        if (deltaZ > boundZ || deltaZ < -boundZ)
        {
            if (transform.position.z < lookAt.position.z)
            {
                delta.z = deltaZ - boundZ;
            }
         else
            {
                delta.z = deltaZ + boundZ;
            }
        }

        transform.position += new Vector3(delta.x, 0, delta.z - zCorrect);
    }
}
