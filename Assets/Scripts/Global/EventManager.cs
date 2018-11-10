using System;
using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    public sealed class EventManager : MonoBehaviour
    {
        private static EventManager _instance;
        private static object _eventLock = new object();

        void Awake()
        {
            _instance = this;
        }

        void Update()
        {
            CheckEventQueue();
        }

        void CheckEventQueue ()
        {
            lock(_eventLock){
                while (_eventQueue.Count > 0) {
                    _eventQueue.Dequeue().Execute();
                }
            }
        }

        private readonly Queue<IEventCallbackWrapper> _eventQueue = new Queue<IEventCallbackWrapper>();
        public static void AddToQueue(IEventCallbackWrapper callbackWrapper)
        {
            lock (_eventLock) {
                _instance._eventQueue.Enqueue(callbackWrapper);
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

        public EventCallbackWrapper(Action action)
        {
            _action = action;
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

        public EventParamCallbackWrapper(Action<TParam> action, TParam eventParam)
        {
            _action = action;
            _eventParam = eventParam;
        }

        public void Execute()
        {
            if (_action != null) {
                _action(_eventParam);
            }
        }
    }
}