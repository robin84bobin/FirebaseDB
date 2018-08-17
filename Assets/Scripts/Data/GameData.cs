using System;
using System.Collections.Generic;
using Assets.Scripts.Commands;
using InternalNewtonsoft.Json.Linq;
using UnityEngine;

namespace Data
{
    public class GameData
    {
        

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


        public event Action<JToken> OnJsonDataLoaded = delegate { };
        public event Action OnDataParseComplete = delegate { };
        public event Action OnInitSuccess = delegate { };
        public event Action OnInitFail = delegate { };

        public void Init(Action onSuccess, Action onFail)
        {
            OnInitSuccess += onSuccess;
            OnInitFail += onFail;

            //TODO define DB proxy type in config or so
            IDataBaseProxy dbProxy = new FireBaseDBProxy();
            dbProxy.Init(OnDBInitComplete);
            BaseStorage.dbProxy = dbProxy;

            Debug.Log(ToString() + " :: Init : " + dbProxy.ToString());
        }

        private void OnDBInitComplete()
        {
            CommandManager.DoSequence(OnStoragesInited, initStorageCommands.ToArray());
        }

        List<Command> initStorageCommands = new List<Command>();
        internal void RegisterStorage<T>(DataStorage<T> dataStorage) where T : Item, new()
        {
            InitStorageCommand<T> command = new InitStorageCommand<T>(dataStorage);
            initStorageCommands.Add(command);
        }


        private void OnStoragesInited()
        {
            throw new NotImplementedException();
        }


    }
}



