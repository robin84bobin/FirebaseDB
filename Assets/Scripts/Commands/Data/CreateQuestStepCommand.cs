using System;
using Data;
using Data.DataTypes;
using Global;
using UnityEngine;

namespace Commands.Data
{
    public class CreateQuestStepCommand : Command
    {
        private QuestStepData _item;

        public CreateQuestStepCommand(string id, string type)
        {
            QuestStepData item = new QuestStepData();
            item.stepType = type;
            item.Id = id;
            item.typeId = id;
            _item = item;
        }

        public override void Execute()
        {
            global::Data.Repository.Steps.Set(_item, _item.Id, true, CreateRelatedData);
        }

        void CreateRelatedData(QuestStepData item)
        {
            switch (item.stepType)
            {
                case Collections.MESSAGE:
                    var messageData = new QuestMessageData { Id = item.typeId };
                    global::Data.Repository.MessageSteps.Set(messageData, messageData.Id, true, OnCreated);
                    break;
                case Collections.TRIGGER:
                    var triggerData = new QuestTriggerStepData { Id = item.typeId };
                    global::Data.Repository.TriggerSteps.Set(triggerData, triggerData.Id, true, OnCreated);
                    break;
                default:
                    Debug.LogError(this + " Save(): unknown type: " + item.stepType);
                    break;
            }
        }

        private void OnCreated(DataItem relatedData)
        {
            GlobalEvents.OnAddStorageItem.Publish(_item);
            //GlobalEvents.OnAddStorageItem.Publish(relatedData);
            Complete();
        }
    }
}