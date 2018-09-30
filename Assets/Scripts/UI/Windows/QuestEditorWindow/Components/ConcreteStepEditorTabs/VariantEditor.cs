using UI.Windows.QuestEditorWindow.Components;
using UnityEngine;
using UnityEngine.UI;

public class VariantEditor : MonoBehaviour {

    [SerializeField]  InputField _text;
    [SerializeField]  QuestDropdown _targetQuestDropdown;

    private QuestVariantData _data;

    internal void SetData(QuestVariantData questVariantData)
    {
        _data = questVariantData;
        _text.text = _data.text;
        _targetQuestDropdown.Init();
        _targetQuestDropdown.Select(_data.targetStepId);
        _targetQuestDropdown.OnQuestSelect += OnTargetQuestSelect;
    }

    private void OnTargetQuestSelect(string id)
    {
        if (_data == null)
        {
            _data = new QuestVariantData();
            _data.text = _text.text;
        }
        _data.targetStepId = id;
    }

    internal void ResetView()
    {
        _data = null;
        _text.text = string.Empty;
        _targetQuestDropdown.ResetView();
        _targetQuestDropdown.OnQuestSelect += OnTargetQuestSelect;
    }

    internal QuestVariantData GetData()
    {
        if (_data == null)
        _data = new QuestVariantData();
        _data.text = _text.text;
        _data.targetStepId = _targetQuestDropdown.GetSelectedText();
        
        return _data;
    }
}
