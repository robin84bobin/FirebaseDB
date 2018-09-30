using System;
using System.Collections.Generic;
using Commands;
using Data.DataBase;
using Data.DataTypes;

namespace Data
{
    public class DataManager
    {
        public  DataStorage<UserQuestStepData> UserSteps;
        public  DataStorage<MessageViewData> UserMessageHistory;
        
        public  DataStorage<QuestStepData> Steps;
        public  DataStorage<QuestTriggerStepData> TriggerSteps;
        public  DataStorage<QuestMessageData> MessageSteps;

        public event Action OnInitComplete = delegate { };
        
        private List<Command> _initStoragesCommands;

        
        public DataManager()
        {
            _initStoragesCommands = new List<Command>();
        }
        
        

        public void Init(Action onSuccess)
        {
            OnInitComplete += onSuccess;

            Steps = new DataStorage<QuestStepData>(Collections.STEP);
            MessageSteps = new DataStorage<QuestMessageData>(Collections.MESSAGE);
            TriggerSteps = new DataStorage<QuestTriggerStepData>(Collections.TRIGGER);
            
            UserSteps = new DataStorage<UserQuestStepData>(Collections.USER_STEP);
            UserMessageHistory = new DataStorage<MessageViewData>(Collections.USER_MESSAGE);
            
            DataBaseProxy.Instance.Init(OnDbInitComplete);
        }

        
        /// <summary>
        /// register storage to init later
        /// </summary>
        /// <param name="dataStorage"></param>
        /// <typeparam name="T"></typeparam>
        internal void RegisterStorage<T>(DataStorage<T> dataStorage) where T : DataItem, new()
        {
            var command = new InitStorageCommand<T>(dataStorage);
            _initStoragesCommands.Add(command);
        }


        private void OnDbInitComplete()
        {
            CommandManager.ExecuteSequence( OnInitComplete, _initStoragesCommands.ToArray());
        }


        public void ClearUserData()
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