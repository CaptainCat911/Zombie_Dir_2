using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{
    public Volume volume;
    LiftGammaGain gamma;
    //Bloom bloom;
    float x = -1;

    void Start()
    {

        if (volume.profile.TryGet<LiftGammaGain>(out gamma))
        {
            gamma.lift.value = new Vector4(0f, 0f, 0f, -1f);
        }
            //bloom.intensity.value = 100f;
    }

   
    void FixedUpdate()
    {
        if (!GameManager.instance.postProcessFinal)
        {
            if (x > 0)
            {
                return;
            }

            if (x < -0.2f)
            {          
                x += 0.006f;
            }
            else
            {
                x += 0.001f;
            }
        }

        if (GameManager.instance.postProcessFinal)
        {
            x += 0.001f;
        }


        gamma.lift.value = new Vector4(0f, 0f, 0f, x);
    }
}
