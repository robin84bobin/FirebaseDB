using System;
using System.Collections.Generic;
using Assets.Scripts.Commands;
using Data.DataBase;
using InternalNewtonsoft.Json.Linq;
using UnityEngine;

namespace Data
{
    public class GameData
    {
        private static GameData _instance;
        public static GameData Instance {
            get {
                if (_instance == null) _instance = new GameData();
                return _instance;
            }
        }

        public static BaseStorage<StepData> Steps = new BaseStorage<StepData>("steps");
        public static BaseStorage<TriggerData> Triggers = new BaseStorage<TriggerData>("trigger");
        public static BaseStorage<NpcMessageData> NpcMessages = new BaseStorage<NpcMessageData>("npcMessage");
        
        List<Command> initStorageCommands = new List<Command>();
        /// <summary>
        /// register storage to init data later
        /// </summary>
        /// <param name="dataStorage"></param>
        /// <typeparam name="T"></typeparam>
        internal void RegisterStorage<T>(BaseStorage<T> dataStorage) where T : Item, new()
        {
            InitStorageCommand<T> command = new InitStorageCommand<T>(dataStorage);
            initStorageCommands.Add(command);
        }

        public event Action OnDataParseComplete = delegate { };
        public event Action OnInitSuccess = delegate { };
        public event Action OnInitFail = delegate { };

        public void Init(Action onSuccess, Action onFail)
        {
            OnInitSuccess += onSuccess;
            OnInitFail += onFail;

            DataBaseProxy.Instance.Init(OnDBInitComplete);
        }

        
        private void OnDBInitComplete()
        {
            CommandManager.ExecuteSequence(OnStoragesInited, initStorageCommands.ToArray());
        }

        
        private void OnStoragesInited()
        {
            initStorageCommands = null;
            OnDataParseComplete.Invoke();

            StepData s = new StepData() { type = "errorType", typeId = int.MinValue };
            Steps.Set(s, 10, true);
        }


    }
}



