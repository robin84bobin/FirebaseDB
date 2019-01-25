using Data;
using Data.DataTypes;
using UI.Windows.QuestEditorWindow.Components.ConcreteStepEditorTabs;
using UnityEngine;

public class TriggerStepEditor : AbstractStepEditor
{

    [SerializeField] TriggerEditor[] _triggerEditors;

    string _id = string.Empty;
    internal override void Init(QuestStepData questStepData)
    {
        QuestStepData = questStepData;
        
        if (!questStepData.stepType.Equals(QuestStepType.TRIGGER))
        {
            Debug.LogError(this.ToString() + " : !questStepData.stepType.Equals(QuestStepType.TRIGGER)");            
            return;
        }
        
        QuestTriggerStepData sourceData = DataManager.TriggerSteps[questStepData.typeId];
        _id = sourceData.Id;
        for (int i = 0; i < _triggerEditors.Length; i++)
        {
            TriggerData triggerData = i < sourceData.Triggers.Length ? sourceData.Triggers[i] : null;
            _triggerEditors[i].Init(triggerData);
        }
    }

    public override void SaveAs()
    {
        GrabDataFromUI();
        SaveQuestAsMenu.Show(new SaveQuestAsMenuParams(
            QuestStepData.Clone(), 
            _triggerStepData.Clone()
        ));
    }
    
    internal override void SaveData()
    {
        GrabDataFromUI();
        DataManager.Steps.Set(QuestStepData, QuestStepData.Id, true);
        DataManager.TriggerSteps.Set(_triggerStepData, _triggerStepData.Id, true);
    }

    internal override QuestStepData GetData()
    {
        GrabDataFromUI();
        return QuestStepData;
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
