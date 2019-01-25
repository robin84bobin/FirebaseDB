using System;
using Data;
using Data.DataTypes;
using Global;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// class controls steps which user had passed
    /// </summary>
    public static class UserQuestController
    {
        public static string activeStepId { get; private set; }

        public static event Action<UserQuestStepData> OnStepComplete = delegate { };

        private static DataStorage<UserQuestStepData> _userStepStorage;


        public static void Init()
        {
            _userStepStorage = Data.Repository.UserSteps;
        }

        public static void GoToStep(string questId, string state = UserQuestState.ACTIVE, int variantId = -1)
        {
            var step = Data.Repository.Steps[questId];

            if (step == null)
                return;

            if (step.stepType == QuestStepType.TRIGGER)
            {
                QuestTriggerStepData questTriggerStepStep = Data.Repository.TriggerSteps[step.typeId];
                CheckTrigger(questTriggerStepStep);
                return;
            }

            if (step.stepType == QuestStepType.MESSAGE)
            {
                QuestMessageData messageStep = Data.Repository.MessageSteps[step.typeId];
                UserQuestStepData userStep = new UserQuestStepData();
                userStep.questStepId = questId;
                userStep.state = state;
            
                CreateMessage(questId, messageStep.text);

                if (variantId > 0)
                    CompleteStep(questId, variantId);

                Data.Repository.UserSteps.Set(userStep, questId);
            }
        
        }

        public static void CompleteStep(string questId, int variantId)
        {
            UserQuestStepData userStep = _userStepStorage.Get(questId);
            if (userStep.state == UserQuestState.COMPLETE)
            {
                Debug.LogError("Failed to complete step! Already completed userStep id=" + questId);
                return;
            }

            QuestStepData questQuestStepData = Data.Repository.Steps[questId];
            if (questQuestStepData.stepType == QuestStepType.MESSAGE)
            {
                QuestMessageData messageStep = Data.Repository.MessageSteps[questQuestStepData.typeId];
                if (variantId < messageStep.variants.Length)
                {
                    QuestVariantData variant = messageStep.variants[variantId];
                    //Complete step
                    userStep.Complete(variantId);
                    //Create message with variant text
                    CreateMessage(questId, variant.text, true);
                    //Go to target step
                    GoToStep(variant.targetStepId);
                }
                else
                {
                    Debug.LogError("Failed to complete step! No variant id=" + variantId.ToString() + " found in step id=" + messageStep.Id);
                    return;
                }
            }
       
            //OnStepComplete.Invoke(userStep);
        }

        static  void CheckTrigger(QuestTriggerStepData questTriggerStepStep)
        {
            TriggerData trigger;
            int length = questTriggerStepStep.Triggers.Length;
            for (int i = 0; i < length; i++) // looking for step to which trigger leads to
            {
                trigger = questTriggerStepStep.Triggers[i];
                UserQuestStepData userStep = _userStepStorage.Get(u => u.questStepId == trigger.triggerStepId);
                //check trigger condition 
                if (trigger.condition == QuestTriggerCondition.COMPLETE)
                {
                    if (userStep != null)
                    {
                        GoToStep(trigger.targetStepId); break;
                    }
                    else
                    if (!string.IsNullOrEmpty(trigger.alterStepId))
                    {
                        GoToStep(trigger.alterStepId); break;
                    }
                }
                else
                if (trigger.condition == QuestTriggerCondition.UNCOMPLETE)
                {
                    if (userStep == null)
                    {
                        GoToStep(trigger.targetStepId); break;
                    }
                    else
                    if (!string.IsNullOrEmpty(trigger.alterStepId))
                    {
                        GoToStep(trigger.alterStepId); break;
                    }
                }
            }
        }

        public static UserQuestStepData GetActiveStep()
        {
            UserQuestStepData activeStep = _userStepStorage.Get(s => s.state == UserQuestState.ACTIVE);
            activeStepId = activeStep.questStepId;
            return activeStep;
        }

        static  void CreateMessage(string parentQuestStepId, string text, bool isUserMsg = false)
        {
            MessageViewData msg = new MessageViewData();
            msg.text = text;
            msg.isUserMsg = isUserMsg;
            msg.parentQuestStepId = parentQuestStepId;

            Data.Repository.UserMessageHistory.Set(msg);
            GlobalEvents.OnMessageNew.Publish(msg);
        }
    }
}
