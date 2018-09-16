using Data;
using UnityEngine;

public class TriggerStepEditor : AbstractStepEditor
{

    [SerializeField] TriggerEditor[] _triggerEditors;

    string _id = string.Empty;
    internal override void Init(StepData stepData)
    {
        QuestTriggerStepData sourceData = App.Data.QuestTriggerStep[stepData.TypeId];
        _id = sourceData.Id;
        for (int i = 0; i < _triggerEditors.Length; i++)
        {
            TriggerData triggerData = i < sourceData.Triggers.Length ? sourceData.Triggers[i] : null;
            _triggerEditors[i].Init(triggerData);
        }
    }

    internal override void SaveData()
    {
        GrabDataFromUI();
        App.Data.Steps.Set(_stepData, _stepData.Id, true);
        App.Data.QuestTriggerStep.Set(_triggerStepData, _triggerStepData.Id, true);
    }

    internal override StepData GetData()
    {
        GrabDataFromUI();
        return _stepData;
    }

    QuestTriggerStepData _triggerStepData;
    protected override void GrabDataFromUI()
    {
        _triggerStepData = new QuestTriggerStepData();
        _triggerStepData.Id = _id;
        _triggerStepData.Triggers = new TriggerData[_triggerEditors.Length];
        for (int i = 0; i < _triggerEditors.Length; i++)
        {
            _triggerStepData.Triggers[i] = _triggerEditors[i].GetTriggerData();
        }
    }

}
