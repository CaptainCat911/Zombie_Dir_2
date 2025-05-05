using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerPlayer : MonoBehaviour
{
    public Transform lookAt;           // сделать private


    private void Start()
    {
        
    }

    void LateUpdate()
    {
        transform.position = new Vector3(lookAt.position.x, lookAt.position.y + 1, lookAt.position.z);
    }

    public void SetPlayerListner()
    {
        lookAt = GameManager.instance.player.transform;
    }
}
