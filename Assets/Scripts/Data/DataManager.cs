using System;
using System.Collections.Generic;
using Commands;
using Data.DataBase;

namespace Data
{
    public class DataManager
    {
        public  DataStorage<UserQuestStepData> UserSteps;
        public  DataStorage<MessageViewData> UserMessageHistory;
        
        public  DataStorage<StepData> Steps;
        public  DataStorage<QuestTriggerStepData> QuestTriggerStep;
        public  DataStorage<QuestMessageData> QuestMessageStep;

        public event Action OnDataParseComplete = delegate { };
        
        private List<Command> _initStorageCommands;

        
        public DataManager()
        {
            _initStorageCommands = new List<Command>();
        }
        
        

        public void Init(Action onSuccess)
        {
            OnDataParseComplete += onSuccess;

            QuestMessageStep = new DataStorage<QuestMessageData>("message");
            QuestTriggerStep = new DataStorage<QuestTriggerStepData>("trigger");
            Steps = new DataStorage<StepData>("steps");
            UserMessageHistory = new DataStorage<MessageViewData>("user_message_history");
            UserSteps = new DataStorage<UserQuestStepData>("user_steps");
            
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
            _initStorageCommands.Add(command);
        }


        private void OnDbInitComplete()
        {
            CommandManager.ExecuteSequence(OnStoragesInited, _initStorageCommands.ToArray());
        }


        private void OnStoragesInited()
        {
            _initStorageCommands = null;
            OnDataParseComplete.Invoke();

            var s = new StepData() {Type = "errorType", TypeId = ""};
            Steps.Set(s, "error_Id", true);
        }

        public void ClearUserData()
        {
            UserSteps.Clear();
            UserMessageHistory.Clear();
        }
    }
}