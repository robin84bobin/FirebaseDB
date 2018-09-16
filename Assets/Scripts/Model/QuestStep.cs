using System;

public class QuestStep
{
    public event Action onActivate;
    public event Action<string> onComplete;


    UserQuestStepData _data;

    public void Init(UserQuestStepData data)
    {

    }

    public void Complete()
    {
        //...
        string nextStepId = GetNextStepId();
        //
        onComplete.Invoke(nextStepId);
    }

    public void Activate()
    {
        //...
        onActivate.Invoke();
    }

    public void SelectOption(int optionIndex)
    {
        //get next step id from option _messageViewData
        //..
        Complete();
    }

    string GetNextStepId()
    {
        return "";
    }
}