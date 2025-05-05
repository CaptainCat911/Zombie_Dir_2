using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereAim : MonoBehaviour
{

    public LayerMask layerMask;
    RaycastHit hit;
    Ray ray;

    public float x;
    public float y;
    public float z;


    void Update()
    {
        ray.origin = transform.position;                        // луч из позиции
        ray.direction = -transform.up;                          // луч с направлением вверх

        Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(transform.position, -transform.up * 100f, Color.yellow);

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            if (hit.distance > 1.1f) 
            {
                y += -0.1f;
            }
            if (hit.distance < 0.9f)
            {
                y += 0.1f;
            }
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Debug.Log(hit.distance);
        }

        if (Physics.Raycast(ray1, out hit, 100, layerMask))
        {
            transform.position = new Vector3(hit.point.x - x, hit.point.y + y, hit.point.z - z);
        }
    }
}
