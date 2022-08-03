using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{
    public Volume volume;           // ссылка на волюмэ (сам построцесс)
    LiftGammaGain gamma;            // для гаммы
    //Bloom bloom;
    float x = -1;                   // переменная для установки гаммы

    void Start()
    {

        if (volume.profile.TryGet<LiftGammaGain>(out gamma))
        {
            //gamma.lift.value = new Vector4(0f, 0f, 0f, 0f);        
        }
            //bloom.intensity.value = 100f;
    }

   
    void FixedUpdate()
    {
        if (!GameManager.instance.postProcessFinal && GameManager.instance.postProcessStart)                 // если не финал, повышаем гамму до нормы 
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
                x += 0.0015f;                                       // скорость выхода из затемнения
            }
        }

        if (GameManager.instance.postProcessFinal)                  // если финал, засветляем экран
        {
            x += 0.002f;                                            // скорость засветления
        }


        gamma.lift.value = new Vector4(0f, 0f, 0f, x);              // устанавливаем гамму
    }

    public void SetGammaMinus()
    {
        gamma.lift.value = new Vector4(0f, 0f, 0f, -1f);        // задаём темноту (гамма = -1)
    }
}
