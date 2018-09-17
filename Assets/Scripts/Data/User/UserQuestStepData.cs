﻿using Data;

/// <summary>
/// Contains info about steps user had passed
/// </summary>
public class UserQuestStepData : DataItem
{
    /// <summary>
    /// Quest step id
    /// </summary>
    public string questStepId;
    
    /// <summary>
    /// State of quest step
    /// </summary>
    public string state;
    
    /// <summary>
    /// variant chosen by user
    /// </summary>
    public int chosenVariantId;
    
    /// <summary>
    /// messages to show.
    /// could contains messages of quest step and
    /// message of user chosen variant
    /// </summary>
    //public List<MessageViewData> messagesToShow = new List<MessageViewData>();

    public void Complete(int variantId)
    {
        state = UserQuestState.COMPLETE;
        chosenVariantId = variantId;
    }

    public QuestMessageData GetQuestStep()
    {
        var step = App.Data.Steps[questStepId];
        if (step.Type == QuestStepType.MESSAGE)
            return App.Data.QuestMessageStep[step.TypeId];
        return null;
    }
}