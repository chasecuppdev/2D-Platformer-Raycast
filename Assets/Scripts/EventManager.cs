using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton
public class EventManager : MonoBehaviour
{
    private static EventManager instance = null;

    public static EventManager Instance
    {
        get { return Instance; }
        set { }
    }

    private Dictionary<EVENT_TYPE, List<IListener>> Listeners = new Dictionary<EVENT_TYPE, List<IListener>>();

    private void Awake()
    {
        //If no instance currently exists, then we will assign this instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //We want the Singleton to persist between scenes
        }
        else
        {
            DestroyImmediate(this); //If the singleton already exists, we don't want to create this new one, so destroy it
        }
    }

    /// <summary>
    /// Add listeners to the EventManager
    /// </summary>
    /// <param name="event_type"></param>
    /// <param name="listener"></param>
    public void AddListener(EVENT_TYPE event_type, IListener listener)
    {
        List<IListener> listenerList = null;

        //If a list already exists for this event_type, just add the listener to that list
        if (Listeners.TryGetValue(event_type, out listenerList))
        {
            listenerList.Add(listener); //This is the actual list persisted in the dictionary, obtained from the out parameter of TryGetValue
            return;
        }

        //A list for the event_type did NOT already exist, so create it, add the listener, and add to the dictionary
        listenerList = new List<IListener>();
        listenerList.Add(listener);
        Listeners.Add(event_type, listenerList);
    }

    /// <summary>
    /// Notify all listeners for an EVENT_TYPE that the event has occurred
    /// </summary>
    /// <param name="event_type"></param>
    /// <param name="Sender"></param>
    /// <param name="param"></param>
    public void PostNotification(EVENT_TYPE event_type, Component Sender, Object param = null)
    {
        List<IListener> listenerList = null;

        //If no list exists for event_type, just return because no one is listening
        if (!Listeners.TryGetValue(event_type, out listenerList))
        {
            return;
        }

        //Cycle through each listener in listernerList and check for nulls\
        foreach (IListener listener in listenerList)
        {
            //If the gameobject has been destroyed, remove it from the listenerList
            if (listener == null)
            {
                listenerList.Remove(listener);
            }
            else
            {
                listener.OnEvent(event_type, Sender, param);
            }
        }
    }
}
