using System;
using System.Linq;
using Data;
using UnityEngine;

namespace Global
{
    public static class GlobalEvents
    {
        public static readonly GlobalEvent OnDataInited = new GlobalEvent();
        public static readonly GlobalEvent OnStorageLoaded = new GlobalEvent();
        public static readonly GlobalEventParam<string> OnBackup = new GlobalEventParam<string>();

        public static readonly GlobalEventParam<string> OnLoadingProgress = new GlobalEventParam<string>();
        public static readonly GlobalEventParam<KeyCode> OnButton = new GlobalEventParam<KeyCode>();
        public static readonly GlobalEventParam<Vector3> OnMoveControl = new GlobalEventParam<Vector3>();
        public static readonly GlobalEventParam<MessageViewData> OnMessageNew = new GlobalEventParam<MessageViewData>();
        public static readonly GlobalEventParam<MessageViewData> OnMessageTypeComplete = new GlobalEventParam<MessageViewData>();
        //
        public static readonly GlobalEventParam<string> OnRemoveStorageItem = new GlobalEventParam<string>();
        public static readonly GlobalEventParam<DataItem> OnAddStorageItem = new GlobalEventParam<DataItem>();

        public static readonly GlobalEventParam<string> LoadScene = new GlobalEventParam<string>();
    }
    
    
    public class GlobalEvent
    {
        private  object _lockObject = new object();

        protected  event Action Event = delegate { };

        public  void Subscribe(Action value)
        {
            lock (_lockObject)
            {
                Event += value;
            }
        }
        
        public  void Unsubscribe(Action value)
        {
            lock (_lockObject)
            {
                Event -= value;
            }
        }

        public  void Publish()
        {
            EventManager.AddToQueue( new EventCallbackWrapper(Event));
        }
    }
    
    public class GlobalEventParam<TParam>
    {
        private object _lockObject = new object();

        private event Action<TParam> Event = delegate { };

        public void Subscribe(Action<TParam> value)
        {
            lock (_lockObject)
            {
                if (!Event.GetInvocationList().Contains(value)) Event += value;
            }
        }
        
        public void Unsubscribe(Action<TParam> value)
        {
            lock (_lockObject)
            {
                Event -= value;
            }
        }

        public void Publish(TParam eventParam)
        {
            EventManager.AddToQueue( new EventParamCallbackWrapper<TParam>(Event, eventParam));
        }
    }
    
    
}