using System;
using Data;
using Data.DataTypes;
using Global;
using UnityEngine;

namespace Commands.Data
{

    public class RemoveQuestStepCommand : Command 
    {
        [Zenject.Inject] private Repository _repository;
        private readonly QuestStepData _data;

        public RemoveQuestStepCommand(QuestStepData data)
        {
            _data = data;
        }

        public override void Execute()
        {
            System.Action<string> callback = delegate { }; 
            callback += RemoveRelatedData;
            callback += GlobalEvents.OnRemoveStorageItem.Publish;
             _repository.Steps.Remove(_data.Id, true, RemoveRelatedData);
        }

        void RemoveRelatedData(string id)
        {
            switch (_data.stepType)
            {
                case Collections.MESSAGE:
                     _repository.MessageSteps.Remove(_data.typeId, true, OnRemoved);
                    break;
                case Collections.TRIGGER:
                     _repository.TriggerSteps.Remove(_data.typeId, true , OnRemoved);
                    break;
                default:
                    Debug.LogError(this + " Remove(): unknown type: " + _data.stepType);
                    break;
            }
        }

        private void OnRemoved(string id)
        {
            GlobalEvents.OnRemoveStorageItem.Publish(_data.Id);
            //GlobalEvents.OnRemoveStorageItem.Publish(id);
            Complete();
        }
    }
}