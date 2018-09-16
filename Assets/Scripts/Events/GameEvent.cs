using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class GameEvent
    {
        private readonly List<Action> _callbacks;

        public GameEvent()
        {
            _callbacks = new List<Action> ();
        }
	
        public void Subscribe( Action callback_)
        {
            if (_callbacks.Contains(callback_)){
                Debug.LogWarning(string.Format ("Dublicate event '{0}' subscription callback: {1}", this.GetType().Name, callback_.ToString()));
                return;
            }
            _callbacks.Add(callback_);
        }
	
        public void Unsubscribe(Action callback_)
        {
            if (!_callbacks.Contains(callback_)){
                return;
            }
		
            if (_callbacks.Contains(callback_)){
                _callbacks.Remove(callback_);
            }
        }
	
        public void Publish()
        {
            for (int i = 0; i < _callbacks.Count; i++) {
                EventManager.Instance.AddToQueue( new EventCallbackWrapper(_callbacks[i]));
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

        public void Subscribe( Action<TParam> callback_)
        {
            if (_callbacks.Contains(callback_)){
                Debug.LogWarning(string.Format ("Dublicate event '{0}' subscription callback: {1}", this.GetType().Name, callback_.ToString()));
                return;
            }
		
            _callbacks.Add(callback_);
        }

        public void Unsubscribe(Action<TParam> callback_)
        {
            if (!_callbacks.Contains(callback_)){
                return;
            }
		
            if (_callbacks.Contains(callback_)){
                _callbacks.Remove(callback_);
            }
        }

        public void Publish( TParam eventParam_)
        {
            for (int i = 0; i < _callbacks.Count; i++) {
                EventManager.Instance.AddToQueue( new EventParamCallbackWrapper<TParam>(_callbacks[i], eventParam_));
            }
        }
    }
}