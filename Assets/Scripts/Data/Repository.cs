using System;
using System.Collections.Generic;
using System.IO;
using Commands;
using Commands.Data;
using Data.DataBase;
using Data.DataTypes;
using Global;
using UnityEngine;
using Zenject;

namespace Data
{
    public class Repository : IRepository
    {
        [Inject] private IDataBaseProxy _dataBase;
        
        public DataStorage<UserQuestStepData> UserSteps;
        public DataStorage<MessageViewData> UserMessageHistory;
        
        public DataStorage<QuestStepData> Steps;
        public DataStorage<QuestTriggerStepData> TriggerSteps;
        public DataStorage<QuestMessageData> MessageSteps;

        public event Action OnInitComplete = delegate { };
        
        private List<Command> _initStoragesCommands;

        
        public Repository()
        {
            _initStoragesCommands = new List<Command>();
            GlobalEvents.OnBackup.Subscribe(DoBackup);
        }
        
        public void Init()
        {
            Steps = CreateStorage<QuestStepData>(Collections.STEP);
            MessageSteps = CreateStorage<QuestMessageData>(Collections.MESSAGE);
            TriggerSteps = CreateStorage<QuestTriggerStepData>(Collections.TRIGGER);
            
            UserSteps = CreateStorage<UserQuestStepData>(Collections.USER_STEP);
            UserMessageHistory = CreateStorage<MessageViewData>(Collections.USER_MESSAGE);
            
            _dataBase.OnInitialized += OnDbInitComplete;
            _dataBase.Init();
        }

        
        /// <summary>
        /// register storage to init later
        /// </summary>
        /// <param name="dataStorage"></param>
        /// <typeparam name="T"></typeparam>
        DataStorage<T> CreateStorage<T>(string collectionName) where T : DataItem, new()
        {
            var dataStorage = new DataStorage<T>(collectionName);
            var command = new InitStorageCommand<T>(dataStorage);
            _initStoragesCommands.Add(command);

            return dataStorage;
        }


        void OnDbInitComplete()
        {
            _dataBase.OnInitialized -= OnDbInitComplete;
            CommandSequence sequence = new CommandSequence(_initStoragesCommands.ToArray());
                sequence.OnComplete += () => OnInitComplete.Invoke();
            CommandManager.Execute( sequence );
        }


        public void ClearUserData()
        {
            UserSteps.Clear();
            UserMessageHistory.Clear();
        }
        
        public void DoBackup(string json)
        {
            if (string.IsNullOrEmpty(json)) 
                return;
            
            Debug.Log(json);

            var filePath = Application.streamingAssetsPath + "/backup.json";
            StreamWriter writer = File.CreateText(filePath);
            writer.Write(json.ToCharArray());
            writer.Close();
        }
    }

    public interface IRepository
    {
        void Init();
        void ClearUserData();
        void DoBackup(string json);
        event Action OnInitComplete;
    }


    public struct Collections
    {
        public const string STEP = "step";
        public const string USER_STEP = "user_step";
        public const string MESSAGE = "message";
        public const string TRIGGER = "trigger";
        public const string USER_MESSAGE = "user_message_history";
    }
}