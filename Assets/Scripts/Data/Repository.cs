using System;
using System.Collections.Generic;
using Commands;
using Commands.Data;
using Data.DataBase;
using Data.DataTypes;

namespace Data
{
    public static class Repository
    {
        public static  DataStorage<UserQuestStepData> UserSteps;
        public static  DataStorage<MessageViewData> UserMessageHistory;
        
        public static  DataStorage<QuestStepData> Steps;
        public static  DataStorage<QuestTriggerStepData> TriggerSteps;
        public static  DataStorage<QuestMessageData> MessageSteps;

        public  static  event Action OnInitComplete = delegate { };
        
        private static List<Command> _initStoragesCommands;

        
        static Repository()
        {
            _initStoragesCommands = new List<Command>();
        }
        
        

        public static void Init()
        {
            Steps = CreateStorage<QuestStepData>(Collections.STEP);
            MessageSteps = CreateStorage<QuestMessageData>(Collections.MESSAGE);
            TriggerSteps = CreateStorage<QuestTriggerStepData>(Collections.TRIGGER);
            
            UserSteps = CreateStorage<UserQuestStepData>(Collections.USER_STEP);
            UserMessageHistory = CreateStorage<MessageViewData>(Collections.USER_MESSAGE);
            
            DataBaseProxy.Instance.OnInitialized += OnDbInitComplete;
            DataBaseProxy.Instance.Init();
        }

        
        /// <summary>
        /// register storage to init later
        /// </summary>
        /// <param name="dataStorage"></param>
        /// <typeparam name="T"></typeparam>
        static DataStorage<T> CreateStorage<T>(string collectionName) where T : DataItem, new()
        {
            var dataStorage = new DataStorage<T>(collectionName);
            var command = new InitStorageCommand<T>(dataStorage);
            _initStoragesCommands.Add(command);

            return dataStorage;
        }


        static void OnDbInitComplete()
        {
            DataBaseProxy.Instance.OnInitialized -= OnDbInitComplete;
            CommandSequence sequence = new CommandSequence(_initStoragesCommands.ToArray());
                sequence.OnComplete += () => OnInitComplete.Invoke();
            CommandManager.Execute( sequence );
        }


        public static void ClearUserData()
        {
            UserSteps.Clear();
            UserMessageHistory.Clear();
        }
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