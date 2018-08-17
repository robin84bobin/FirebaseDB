using System;
using System.Collections.Generic;
using Assets.Scripts.Commands;
using InternalNewtonsoft.Json.Linq;
using UnityEngine;

namespace Data
{
    public class GameData
    {
        public IDataBaseProxy dbProxy { get; private set; }

        private static GameData _instance;
        public static GameData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameData();
                }
                return _instance;
            }
        }

        public static BaseStorage<StepData> Steps = new BaseStorage<StepData>("steps");
        public static BaseStorage<TriggerData> Triggers = new BaseStorage<TriggerData>("trigger");
        public static BaseStorage<NpcMessageData> NpcMessages = new BaseStorage<NpcMessageData>("npcMessage");


        public event Action OnDataParseComplete = delegate { };
        public event Action OnInitSuccess = delegate { };
        public event Action OnInitFail = delegate { };

        public void Init(Action onSuccess, Action onFail)
        {
            OnInitSuccess += onSuccess;
            OnInitFail += onFail;

            //TODO define DB proxy type in config or so
            dbProxy = new FireBaseDBProxy();
            dbProxy.Init(OnDBInitComplete);
        }

        private void OnDBInitComplete()
        {
            CommandManager.DoSequence(OnStoragesInited, initStorageCommands.ToArray());
        }

        List<Command> initStorageCommands = new List<Command>();
        internal void RegisterStorage<T>(BaseStorage<T> dataStorage) where T : Item, new()
        {
            InitStorageCommand<T> command = new InitStorageCommand<T>(dataStorage);
            initStorageCommands.Add(command);
        }


        private void OnStoragesInited()
        {
            initStorageCommands = null;
            GameData.Instance.OnDataParseComplete.Invoke();
        }


    }
}



