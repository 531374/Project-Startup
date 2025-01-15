using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus<T> where T : Event
{
    public static event Action<T> OnEvent;

    public static void Publish(T pEvent)
    {
        OnEvent?.Invoke(pEvent);
    }
}

public abstract class Event
{

}
