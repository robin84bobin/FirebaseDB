using System;
using InternalNewtonsoft.Json;
using UnityEngine;

namespace Data.DataTypes
{
    public class QuestStepData : DataItem
    {
        /// <summary>
        /// Type of step. Collection name in dataBase
        /// </summary>
        public string stepType  = string.Empty;
        
        /// <summary>
        /// Step id of current type.
        /// </summary>
        public string typeId = string.Empty;

        private int _savedBytes;

        public void Create()
        {
            App.Data.Steps.Set(this, Id, true, delegate(QuestStepData item)
            {
                switch (item.stepType)
                {
                    case Collections.MESSAGE:
                        var messageData = new QuestMessageData {Id = item.typeId};
                        App.Data.MessageSteps.Set(messageData, messageData.Id, true);
                        break;
                    case Collections.TRIGGER:
                        var triggerData = new QuestTriggerStepData {Id = item.typeId};
                        App.Data.TriggerSteps.Set(triggerData, triggerData.Id, true);
                        break;
                    default:
                        Debug.LogError(this + " Save(): unknown type: " + item.stepType);
                        break;
                }
            });
        }


        public void Save(DataItem relatedData = null)
        {
            App.Data.Steps.Set(this, Id, true, delegate(QuestStepData item)
            {
                switch (item.stepType)
                {
                    case Collections.MESSAGE:
                        var messageData = relatedData as QuestMessageData ?? new QuestMessageData {Id = item.typeId};
                        App.Data.MessageSteps.Set(messageData, messageData.Id, true);
                        break;
                    case Collections.TRIGGER:
                        var triggerData = relatedData as QuestTriggerStepData?? new QuestTriggerStepData {Id = item.typeId};
                        App.Data.TriggerSteps.Set(triggerData, triggerData.Id, true);
                        break;
                    default:
                        Debug.LogError(this + " Save(): unknown type: " + item.stepType);
                        break;
                }
            });
        }


        public void Remove()
        {
            App.Data.Steps.Remove(Id, true, RemoveRelatedData);
        }

        private void RemoveRelatedData(string id)
        {
            switch (stepType)
            {
                case Collections.MESSAGE:
                    App.Data.MessageSteps.Remove(typeId, true);
                    break;
                case Collections.TRIGGER:
                    App.Data.TriggerSteps.Remove(typeId, true);
                    break;
                default:
                    Debug.LogError(this + " Remove(): unknown type: " + stepType);
                    break;
            }
        }

    }

}