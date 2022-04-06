using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : UnityEvent<string>
{

}

public class WeaponAnimationEvents : MonoBehaviour
{
    public AnimationEvent MeleeAnimationEvent = new AnimationEvent();           // Для атаки топором
    public AnimationEvent WeaponAnimationEvent = new AnimationEvent();          // Для перезарядки
    public AnimationEvent ZombieAnimationEvent = new AnimationEvent();          // Для атаки зомби
    public AnimationEvent ZombieAnimationGrabEvent = new AnimationEvent();      // Для захвата зомби


    public void OnMeleeAnimationEvents(string eventName)
    {
        MeleeAnimationEvent.Invoke(eventName);
    }

    public void OnAnimationEvents(string eventName)
    {
        WeaponAnimationEvent.Invoke(eventName);
    }

    public void OnAnimationEventsZombie(string eventName)
    {
        ZombieAnimationEvent.Invoke(eventName);
    }   
    
    
    public void OnAnimationEventsZombieGrab(string eventName)
    {
        ZombieAnimationGrabEvent.Invoke(eventName);
    }
}
