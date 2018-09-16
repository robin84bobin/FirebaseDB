using Data;
using UnityEngine;
using UnityEngine.UI;

public class MessageStepEditor : AbstractStepEditor {

    #region Components
    [SerializeField]    InputField _messageInput;
    [SerializeField]    VariantEditor[] _variants;
    #endregion

    private void ResetVariants()
    {
        for (int i = 0; i < _variants.Length; i++)
            _variants[i].ResetView();
    }

    string _id = string.Empty;
    internal override void Init(StepData stepData)
    {
        _stepData = stepData;
        QuestMessageData questMessageData = App.Data.QuestMessageStep[_stepData.TypeId];
        _messageInput.text = questMessageData.text;
        _id = questMessageData.Id;

        ResetVariants();
        for (int i = 0; i < _variants.Length; i++)
        {
            if (i < questMessageData.variants.Length)
                _variants[i].SetData(questMessageData.variants[i]);
        }
    }

    internal override void SaveData()
    {
        GrabDataFromUI();
        App.Data.Steps.Set(_stepData, _stepData.Id, true);
        App.Data.QuestMessageStep.Set(_questMessageData, _questMessageData.Id, true);
    }

    internal override StepData GetData()
    {
        GrabDataFromUI();
        return _stepData;
    }

    QuestMessageData _questMessageData;
    protected override void GrabDataFromUI()
    {
        _questMessageData = new QuestMessageData();
        _questMessageData.Id = _id;
        _questMessageData.text = _messageInput.text;
        
        int varCnt = 0;
        for (int i = 0; i < _variants.Length; i++)
        {
            var varData = _variants[i].GetData();
            if (varData != null && varData.targetStepId != "None")
            {
                varCnt++;
            }
            
        }

        _questMessageData.variants = new QuestVariantData[varCnt];
        for (int j = 0; j < varCnt; j++)
        {
            _questMessageData.variants[j] = _variants[j].GetData();
        }
    }


}
