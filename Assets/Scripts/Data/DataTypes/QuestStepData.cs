﻿using System;
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


        
        public QuestStepData Clone()
        {
            return new QuestStepData()
            {
                Id = this.Id,
                typeId = this.typeId,
                stepType = this.stepType
            };
        }
    }

}