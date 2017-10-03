namespace PGT.Core
{
    using System.Collections.Generic;
    using UnityEngine;
    using Func;
    using DataStructures;

    public enum ReturnState { Done, Keep }

    public class WaitForEvent : CustomYieldInstruction
    {
        bool wait;
        public override bool keepWaiting { get
            {
                return wait;
            }
        }

        public WaitForEvent(string evt)
        {
            wait = true;
            EventRegistry.instance.AddEventListener(evt, () => wait = false);
        }

        public static WaitForEvent Private(UnityEngine.Object ctx, string evt)
        {
            return new WaitForEvent(EventRegistry.instance.PrivateEvent(ctx, evt));
        }
    }
    
    class CheckActive
    {
        MonoBehaviour mb;
        EventRegistry.Callback listener;

        public CheckActive(MonoBehaviour mb, Lambda listener, bool persistent = false)
        {
            this.mb = mb;
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            this.listener = (object t) =>
            {
                listener();
                return persistence;
            };
        }
        public CheckActive(MonoBehaviour mb, System.Action<object> listener, bool persistent = false)
        {
            this.mb = mb;
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            this.listener = (object t) =>
            {
                listener(t);
                return persistence;
            };
        }
        public CheckActive(MonoBehaviour mb, EventRegistry.Callback listener)
        {
            this.mb = mb;
            this.listener = listener;
        }

        public ReturnState untilDisabled(object t)
        {
            if (mb == null) return ReturnState.Done;
            else return listener(t);
        }

        public ReturnState whenActiveAndDisabled(object t)
        {
            if (mb == null) return ReturnState.Done;
            if (mb.isActiveAndEnabled) return listener(t);
            else return ReturnState.Keep;
        }
    }

    public static class EventRegistryMonoBehaviourExtensions
    {
        public static int AddEventListenerUntilDisabled(this MonoBehaviour mb, string Event, Lambda listener, bool persistent = false, UnityEngine.Object context=null)
        {
            Event = context == null ? Event : EventRegistry.instance.PrivateEvent(context, Event);
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            return EventRegistry.instance.AddEventListener(Event, new CheckActive(mb, listener, persistent).untilDisabled);
        }

        public static int AddEventListenerUntilDisabled(this MonoBehaviour mb, string Event, EventRegistry.Callback listener, UnityEngine.Object context=null)
        {
            Event = context == null ? Event : EventRegistry.instance.PrivateEvent(context, Event);
            return EventRegistry.instance.AddEventListener(Event, new CheckActive(mb, listener).untilDisabled);
        }

        public static int AddEventListenerUntilDisabled<T>(this MonoBehaviour mb, string Event, System.Action<T> listener, bool persistent = false, UnityEngine.Object context = null)
        {
            Event = context == null ? Event : EventRegistry.instance.PrivateEvent(context, Event);
            return EventRegistry.instance.AddEventListener(Event, new CheckActive(mb, (object v)=>listener((T)v), persistent).untilDisabled);
        }

        public static int AddEventListenerWhenActiveAndEnabled<T>(
            this MonoBehaviour mb, string Event, System.Action<T> listener, bool persistent = false
        )
        {
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;

            return EventRegistry.instance.AddEventListener(Event, (object t) =>
            {
                if (mb == null) return ReturnState.Done;
                if (mb.isActiveAndEnabled)
                {
                    listener((T)t);
                    return persistence;
                }
                return ReturnState.Keep;
            });
        }

        public static int AddEventListenerWhenActiveAndEnabledPvt<T>(
            this MonoBehaviour mb, string Event, System.Action<T> listener, bool persistent, UnityEngine.Object context 
        ){
            Event = (context == null) ? Event : EventRegistry.instance.PrivateEvent(context, Event);
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            return EventRegistry.instance.AddEventListener(Event, (object t) =>
            {
                if (mb == null) return ReturnState.Done;
                if (mb.isActiveAndEnabled)
                {
                    listener((T)t);
                    return persistence;
                }
                else return ReturnState.Keep;
            });
        }

        public static int AddEventListenerWhenActiveAndEnabledPvt<T>(
            this MonoBehaviour mb, string Event, System.Action<T> listener, bool persistent, IPrivateEventDispatchable context
        )
        {
            Event = (context == null) ? Event : EventRegistry.instance.PrivateEvent(context, Event);
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            return EventRegistry.instance.AddEventListener(Event, (object t) =>
            {
                if (mb == null) return ReturnState.Done;
                if (mb.isActiveAndEnabled)
                {
                    listener((T)t);
                    return persistence;
                }
                else return ReturnState.Keep;
            });
        }

        public static int AddEventListenerWhenActiveAndEnabledPvt(this MonoBehaviour mb, string Event, Lambda listener, bool persistent, UnityEngine.Object context)
        {
            Event = context == null ? Event : EventRegistry.instance.PrivateEvent(context, Event);
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            return EventRegistry.instance.AddEventListener(Event, new CheckActive(mb, listener, persistent).whenActiveAndDisabled);
        }

        public static int AddEventListenerWhenActiveAndEnabledPvt(
            this MonoBehaviour mb, string Event, Lambda listener, bool persistent = false, IPrivateEventDispatchable context = null)
        {
            Event = context == null ? Event : EventRegistry.instance.PrivateEvent(context, Event);
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            return EventRegistry.instance.AddEventListener(Event, new CheckActive(mb, listener, persistent).whenActiveAndDisabled);
        }

        public static int AddEventListenerWhenActiveAndEnabledPvt(this MonoBehaviour mb, string Event, EventRegistry.Callback listener, UnityEngine.Object context)
        {
            Event = context == null ? Event : EventRegistry.instance.PrivateEvent(context, Event);
            return EventRegistry.instance.AddEventListener(Event, new CheckActive(mb, listener).whenActiveAndDisabled);
        }

        public static int AddEventListenerWhenActiveAndEnabledPvt(this MonoBehaviour mb, string Event, EventRegistry.Callback listener, IPrivateEventDispatchable context)
        {
            Event = context == null ? Event : EventRegistry.instance.PrivateEvent(context, Event);
            return EventRegistry.instance.AddEventListener(Event, new CheckActive(mb, listener).whenActiveAndDisabled);
        }
    }


    public class EventRegistry
    {

        //singleton
        private static EventRegistry _instance = null;

        public static EventRegistry instance
        {
            get
            {
                if (_instance == null) _instance = new EventRegistry();
                return _instance;
            }
        }

        public static void Reset()
        {
            _instance = null;
        }

        //singleton members
        public delegate ReturnState Callback(object t);
        private Dictionary<string, Dictionary<int, Callback>> registry;
        int counter;

        int gc_counter;
        int gc_max = 100;

        //bool flag;
        struct Event
        {
            public string evt;
            public object param;
        }
        Heap<float, Event> EventQueue;

        Queue<Tuple<string, object, bool>> InvokeSet;

        float time;

        Dictionary<string, object> lateEvents;

        bool isInvoking = false;

        Queue<Lambda> AddEventListenerQueue;

        //constructor
        private EventRegistry()
        {
            registry = new Dictionary<string, Dictionary<int, Callback>>();
            counter = 0;
            EventQueue = new Heap<float, Event>();
            //flag = false;
            lateEvents = new Dictionary<string, object>();
            InvokeSet = new Queue<Tuple<string, object, bool>>();
            AddEventListenerQueue = new Queue<Lambda>();
        }

        public string PrivateEventIid(int iid, string Event)
        {
            return PrivateEvent(iid.ToString(), Event);
        }

        public string PrivateEvent(IPrivateEventDispatchable obj, string Event)
        {
            return PrivateEvent(obj.instanceId(), Event);
        }

        public string PrivateEvent(string instanceId, string Event)
        {
            return "#private/" + instanceId + "/" + Event;
        }

        public string PrivateEvent(UnityEngine.Object obj, string Event)
        {
            var id = obj.GetInstanceID();
            return PrivateEvent(id.ToString(), Event);
        }

        public int AddEventListener(string Event, Callback listener)
        {
            //check for late listeners
            RemoteDebug.RemoteDebug.instance?.Log("[EventRegistry] Adding listener to event \"" + Event + "\"");


            if (lateEvents != null && lateEvents.ContainsKey(Event) &&
                listener.Invoke(lateEvents[Event]) == ReturnState.Done)
            {
                return -1;
            }
            
            int id = this.counter++;

            if (!isInvoking)
            {
                if (!registry.ContainsKey(Event))
                {
                    registry.Add(Event, new Dictionary<int, Callback>());
                }
                registry[Event].Add(id, listener);

            }
            else
            {
                RemoteDebug.RemoteDebug.instance?.Log("[EventRegistry] Adding a listener within another event listener. The listener will only be added after all events in this frame have been called.");
                AddEventListenerQueue.Enqueue(() =>
                {
                    if (!registry.ContainsKey(Event))
                    {
                        registry.Add(Event, new Dictionary<int, Callback>());
                    }
                    registry[Event].Add(id, listener);
                    RemoteDebug.RemoteDebug.instance?.Log("[EventRegistry] Delayed listener id "+id+" added. (Event: \""+Event+"\")");
                });
            }


            return id;
        }


        public int AddEventListener(string Event, Lambda listener, bool persistent = false)
        {
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            return AddEventListener(Event, (object _flag) =>
            {
                listener.Invoke();
                return persistence;
            });
        }

        public int AddEventListener<T>(string Event, System.Action<T> listener, bool persistent = false)
        {
            return AddEventListener(Event, (object k) => listener((T)k), persistent);
        }

        public int AddEventListener(string Event, System.Action<object> listener, bool persistent = false)
        {
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            return AddEventListener(Event, (object obj) =>
            {
                listener.Invoke(obj);
                return persistence;
            });
        }

        public int AddEventListener(string Event, Future listener, bool persistent = false)
        {
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            return AddEventListener(Event, (object _flag) =>
            {
                listener.bind();
                return persistence;
            });
        }

        public int AddEventListener<T>(string Event, Continuation<T> listener, bool persistent = false)
        {
            return AddEventListener(Event, Continuation<T>.Box(listener), persistent);
        }

        public int AddEventListener(string Event, Continuation<object> listener, bool persistent = false)
        {
            ReturnState persistence = persistent ? ReturnState.Keep : ReturnState.Done;
            return AddEventListener(Event, (object _flag) =>
            {
                listener.apply(_flag).bind();
                return persistence;
            });
        }

        public void InvokePrivateIid(int iid, string Event, object param = null, bool allowLateListeners = false)
        {
            InvokeSet.Enqueue(Tuple._(PrivateEventIid(iid, Event), param, allowLateListeners));
        }

        public void InvokePrivate(UnityEngine.Object ctx, string Event, object param = null, bool allowLateListeners = false)
        {
            InvokeSet.Enqueue(Tuple._(PrivateEvent(ctx, Event), param, allowLateListeners));
        }

        public void InvokePrivate(IPrivateEventDispatchable ctx, string Event, object param = null, bool allowLateListeners = false)
        {
            InvokeSet.Enqueue(Tuple._(PrivateEvent(ctx, Event), param, allowLateListeners));
        }

        public void Invoke(string Event, object param = null, bool allowLateListeners = false)
        {
            InvokeSet.Enqueue(Tuple._(Event, param, allowLateListeners));
        }

        void _invoke(Tuple<string, object, bool> par)
        {
            _invoke(par.car, par.cdr, par.cpr);
        }

        void _invoke(string Event, object param = null, bool allowLateListeners = false)
        {
            isInvoking = true;
            if (allowLateListeners)
            {
                lateEvents.Add(Event, param);
            }
            if (!registry.ContainsKey(Event))
            {
                RemoteDebug.RemoteDebug.instance?.Log("[EventRegistry] Invoking event \"" + Event + "\" with no listener(s).");
                isInvoking = false;
                return;
            }

            RemoteDebug.RemoteDebug.instance?.Log("[EventRegistry] Invoking event \"" + Event + "\" with " + registry[Event].Count + " listener(s).");

            List<int> removals = new List<int>();
            foreach(KeyValuePair<int, Callback> listener in registry[Event])
            {
                if (listener.Value.Invoke(param) == ReturnState.Done)
                    removals.Add(listener.Key);
            }

            foreach(int removal in removals)
            {
                registry[Event].Remove(removal);
            }

            gc_counter++;
            if (gc_counter == gc_max) Clean();
            isInvoking = false;
        }

        public void DisableLateListeners(string Event)
        {
            if (lateEvents != null &&
                lateEvents.ContainsKey(Event))
                lateEvents.Remove(Event);
        }

        public int GetListenerCount(string Event)
        {
            if (!registry.ContainsKey(Event)) return 0;
            return registry[Event].Count;
        }

        public Tuple<string, int> SetTimeout(float seconds, Callback callback, object param = null)
        {
            string evt = System.Guid.NewGuid().ToString();
            int id = AddEventListener(evt, callback);

            Tuple<string, int> ret = Tuple._(evt, id);

            InvokeAfter(seconds, evt, param);
            return ret;
        }

        public Tuple<string, int> SetTimeout(float seconds, Lambda callback)
        {
            return SetTimeout(seconds, (object _flag) =>
            {
                callback.Invoke();
                return ReturnState.Done;
            });
        }

        public Tuple<string, int> SetTimeout<T>(float seconds, System.Action<T> callback, T param)
        {
            return SetTimeout(seconds, () =>
            {
                callback.Invoke(param);
            });
        }

        public Tuple<string, int> SetTimeout(float seconds, Future callback)
        {
            return SetTimeout(seconds, callback.bind);
        }

        public Tuple<string, int> SetTimeout<T>(float seconds, Continuation<T> callback, T param)
        {
            return SetTimeout(seconds, callback.apply(param).bind);
        }

        public void InvokeAfter(float seconds, string Event, object param = null)
        {
            if (EventQueue.Count == 0)
            {
                time = 0;
            }
            Event e = new EventRegistry.Event();
            e.evt = Event;
            e.param = param;
            EventQueue.Insert(e, time + seconds);
        }

        public void RemoveEventListener(string Event, int id = -1)
        {
            if (!registry.ContainsKey(Event)) return;

            if (!isInvoking)
            {
                if (id < 0) registry[Event] = new Dictionary<int, Callback>();
                if (!registry[Event].ContainsKey(id)) return;
                else registry[Event].Remove(id);
            }
            else
            {
                RemoteDebug.RemoteDebug.instance?.Log("[EventRegistry] Removing a listener within another event listener. The listener will only be added after all events in this frame have been called.");
                AddEventListenerQueue.Enqueue(() =>
                {
                    RemoteDebug.RemoteDebug.instance?.Log("[EventRegistry] Delayed event listener(s) removed. (Event: \"" + Event + "\")");
                    if (id < 0) registry[Event] = new Dictionary<int, Callback>();
                    if (!registry[Event].ContainsKey(id)) return;
                    else registry[Event].Remove(id);
                });
            }

        }

        public void Update(float deltaTime)
        {
            while (InvokeSet.Count > 0)
            {
                _invoke(InvokeSet.Dequeue());
            }

            if (EventQueue.Count > 0 && deltaTime > 0)
            {
                time += deltaTime;
                while (EventQueue.Count > 0 && EventQueue.Peek().Key < time)
                {
                    Event e = EventQueue.DeleteMin().Value;
                    _invoke(e.evt, e.param);
                }   
            }
            

            while (AddEventListenerQueue.Count > 0)
            {
                AddEventListenerQueue.Dequeue()();
            }
        }


        public void Clean()
        {
            List<string> clean = new List<string>();
            foreach(KeyValuePair<string, Dictionary<int, Callback>> p in registry)
            {
                if (p.Value.Count == 0) clean.Add(p.Key);
            }
            foreach(string evt in clean)
            {
                registry.Remove(evt);
            }
            gc_counter = 0;
            RemoteDebug.RemoteDebug.instance?.Log("Cleaned " + clean.Count + " unused event(s).");
        }
    }

    class EventRegistryTimerUpdater : MonoBehaviour
    {
        void Update()
        {
            EventRegistry.instance.Update(Time.deltaTime);
        }
    }
}