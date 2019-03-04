using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public delegate void EventAction(params object[] objs);

    public class Event
    {
        public string name;
        public bool active;
        public EventAction action;

        public Event(string name, EventAction action)
        {
            active = true;
            this.name = name;
            this.action = action;
        }

    }

    /// <summary>
    /// 一个事件对应的事件响应集合
    /// </summary>
    public class EventStation 
    {
        List<Event> eventList = new List<Event>();
        public void Dispatch(object []objs)
        {
            foreach(Event signal in eventList)
            {
                if(signal.active && signal.action != null)
                {
                    try
                    {
                        signal.action(objs);
                    }
                    catch(System.Exception exception)
                    {
                        Debug.LogError("signal handle error: "+ signal.name);
                        Debug.LogError(exception);
                    }
                }
            }
        }

        public void AppendEvent(string name, EventAction action)
        {
            bool exist = false;
            foreach(Event signal in eventList)
            {
                if(signal.name == name)
                {
                    signal.action = action;
                    signal.active = true;
                    exist = true;
                }

                if(!exist)
                {
                    eventList.Add(new Event(name, action));
                }
            }
        }

        public void RemoveEvent(string name)
        {
            foreach(Event signal in eventList)
            {
                if(signal.name == name)
                {
                    signal.active = false;
                    break;
                }
            }
        }
    }

    private Dictionary<string, EventStation> eventStore = new Dictionary<string, EventStation>();

    public void AppendEvent(string eventName, string eventFunc, EventAction eventAction)
    {
        if (eventAction == null)
        {
            Debug.LogError("the function for "+ eventFunc + " is null, pleas fix it! ");
            return;
        }

        EventStation station = null;
        if (eventStore.ContainsKey(eventName))
        {
            eventStore.TryGetValue(eventName, out station);
            station.AppendEvent(eventFunc, eventAction);
        }
        else
        {
            station = new EventStation();
            station.AppendEvent(eventFunc, eventAction);
            eventStore.Add(eventName, station);
        }
    }

    public void RemoveEvent(string eventName, string eventFunc)
    {
        EventStation station = null;
        if(eventStore.ContainsKey(eventName))
        {
            eventStore.TryGetValue(eventName, out station);
            station.RemoveEvent(eventFunc);
        }
    }

    public void TriggerEvent(string eventName, params object[] objs)
    {
        EventStation station = null;
        if(eventStore.ContainsKey(eventName))
        {
            eventStore.TryGetValue(eventName,out station);
            try
            {
                station.Dispatch(objs);
            }
            catch(System.Exception exception)
            {
                string str = "!!!!!Error: Game signal handle error:" + eventName + "objsCount:";
                str += objs.Length.ToString();
                foreach (object o in objs)
                {
                    str += " " + o.ToString();
                }
                Debug.LogError(str);
                Debug.LogError(exception);
            }
        }
    }
}
