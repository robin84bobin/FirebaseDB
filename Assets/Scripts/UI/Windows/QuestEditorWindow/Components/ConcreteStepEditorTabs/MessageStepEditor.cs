using Data;
using Data.DataTypes;
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
    internal override void Init(QuestStepData questStepData)
    {
        QuestStepData = questStepData;
        QuestMessageData questMessageData = App.Data.MessageSteps[QuestStepData.typeId];
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
        App.Data.Steps.Set(QuestStepData, QuestStepData.Id, true);
        App.Data.MessageSteps.Set(_questMessageData, _questMessageData.Id, true);
    }

    internal override QuestStepData GetData()
    {
        GrabDataFromUI();
        return QuestStepData;
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
