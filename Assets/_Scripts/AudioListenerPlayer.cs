using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerPlayer : MonoBehaviour
{
    private Transform lookAt;           // сделать private


    private void Start()
    {
        lookAt = GameManager.instance.player.transform;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(lookAt.position.x, lookAt.position.y, lookAt.position.z);
    }
}
