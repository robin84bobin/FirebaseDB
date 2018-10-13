using System;
using System.Collections.Generic;
using Assets.Scripts.Events;
using UnityEngine;

namespace Events
{

    public class GlobalEventParam<TParam>
    {
        private object _lockObject = new object();

        private event Action<TParam> Event = delegate { };

        public void Subscribe(Action<TParam> value)
        {
            lock (_lockObject)
            {
                Event += value;
            }
        }
        
        public void Undubscribe(Action<TParam> value)
        {
            lock (_lockObject)
            {
                Event -= value;
            }
        }

        public void Publish(TParam eventParam)
        {
            //lock (_lockObject) Event.Invoke(p);
            GlobalEvents.Instance.AddToQueue( new EventParamCallbackWrapper<TParam>(Event, eventParam));
        }
    }
    
    public class GlobalEvent
    {
        private object _lockObject = new object();

        private event Action Event = delegate { };

        public void Subscribe(Action value)
        {
            lock (_lockObject)
            {
                Event += value;
            }
        }
        
        public void Unsubscribe(Action value)
        {
            lock (_lockObject)
            {
                Event -= value;
            }
        }

        public void Publish()
        {
            //lock (_lockObject) Event.Invoke();
            
            GlobalEvents.Instance.AddToQueue( new EventCallbackWrapper(Event));
        }
    }
    
    
    
    public class GameEvent
    {
        private readonly List<Action> _callbacks;

        public GameEvent()
        {
            _callbacks = new List<Action> ();
        }
	
        public void Subscribe( Action callback)
        {
            if (_callbacks.Contains(callback)){
                Debug.LogWarning(string.Format ("Dublicate event '{0}' subscription callback: {1}", this.GetType().Name, callback.ToString()));
                return;
            }
            _callbacks.Add(callback);
        }
	
        public void Unsubscribe(Action callback)
        {
            if (!_callbacks.Contains(callback)){
                return;
            }
		
            if (_callbacks.Contains(callback)){
                _callbacks.Remove(callback);
            }
        }
	
        public void Publish()
        {
            for (int i = 0; i < _callbacks.Count; i++) {
                GlobalEvents.Instance.AddToQueue( new EventCallbackWrapper(_callbacks[i]));
            }
        }
    }

    public class GameParamEvent <TParam>
    {
        private List<Action<TParam>> _callbacks;

        public GameParamEvent()
        {
            _callbacks = new List<Action<TParam>> ();
        }

        public void Subscribe( Action<TParam> callback)
        {
            if (_callbacks.Contains(callback)){
                Debug.LogWarning(string.Format ("Duplicated event '{0}' subscription callback: {1}", this.GetType().Name, callback.ToString()));
                return;
            }
		
            _callbacks.Add(callback);
        }

        public void Unsubscribe(Action<TParam> callback)
        {
            if (!_callbacks.Contains(callback)){
                return;
            }
		
            if (_callbacks.Contains(callback)){
                _callbacks.Remove(callback);
            }
        }

        public void Publish( TParam eventParam)
        {
            for (int i = 0; i < _callbacks.Count; i++) {
                GlobalEvents.Instance.AddToQueue( new EventParamCallbackWrapper<TParam>(_callbacks[i], eventParam));
            }
        }
    }
}