using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : UnityEvent<string>
{

}

public class WeaponAnimationEvents : MonoBehaviour
{
    public AnimationEvent WeaponAnimationEvent = new AnimationEvent();
    public AnimationEvent ZombieAnimationEvent = new AnimationEvent();
    public AnimationEvent ZombieAnimationGrabEvent = new AnimationEvent();


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
