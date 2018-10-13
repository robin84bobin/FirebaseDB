using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class EventParam{
	
    }


    public sealed class GlobalEvents : MonoBehaviour
    {
        private static GlobalEvents _instance;
        public static GlobalEvents Instance {
            get {
                return _instance;
            }
        }

        private object _eventLock = new object();

        void Awake()
        {
            _instance = this;
        }

        public static T Get<T>() where T: new ()
        {
            return _instance._getEvent<T> ();
        }

        private readonly Dictionary<Type, object> _events = new Dictionary<Type, object>();
        private TEvent _getEvent<TEvent>() where TEvent: new ()
        {
            lock(_eventLock){
                var type = typeof(TEvent);

                object resultEvent = null;
                if (!_events.TryGetValue(type, out resultEvent)){
                    resultEvent = new TEvent();
                    _events.Add( type, resultEvent);
                    return (TEvent)resultEvent;
                }
                else{
                    return (TEvent)resultEvent;
                }
            }
        }


        private List<IEventCallbackWrapper> _eventQueue = new List<IEventCallbackWrapper>();
        public void AddToQueue(IEventCallbackWrapper callbackWrapper_)
        {
            lock (_eventLock) {
                _eventQueue.Add(callbackWrapper_);
            }
        }


        void Update()
        {
            CheckEventQueue();
        }

        void CheckEventQueue ()
        {
            lock(_eventLock){
                while (_eventQueue.Count > 0) {
                    _eventQueue[0].Execute();
                    _eventQueue.RemoveAt(0);
                }
            }
        }
    }


    public interface IEventCallbackWrapper
    {
        void Execute();
    }

    public sealed class EventCallbackWrapper : IEventCallbackWrapper
    {
        private Action _action;

        public EventCallbackWrapper(Action action_)
        {
            _action = action_;
        }

        public void Execute()
        {
            if (_action != null) {
                _action();
            }
        }
    }

    public sealed class EventParamCallbackWrapper<TParam> : IEventCallbackWrapper
    {
        private Action<TParam> _action;
        private TParam _eventParam;

        public EventParamCallbackWrapper(Action<TParam> action_, TParam eventParam_)
        {
            _action = action_;
            _eventParam = eventParam_;
        }

        public void Execute()
        {
            if (_action != null) {
                _action(_eventParam);
            }
        }
    }
}