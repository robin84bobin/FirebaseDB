using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.QuestEditorWindow.Components.ConcreteStepEditorTabs
{
    public class TriggerEditor : MonoBehaviour {

        [SerializeField] Dropdown _conditionDropdown;
        [SerializeField] QuestDropdown _triggerStepDropdown;
        [SerializeField] QuestDropdown _targetStepDropdown;
        [SerializeField] QuestDropdown _alterStepDropdown;

        TriggerData _data;

        public void Init(TriggerData data = null)
        {
            _data = data;

            List<Dropdown.OptionData> optList = new List<Dropdown.OptionData>()
            {
                new Dropdown.OptionData(QuestTriggerCondition.COMPLETE),
                new Dropdown.OptionData(QuestTriggerCondition.UNCOMPLETE)
            };
            _conditionDropdown.ClearOptions();
            _conditionDropdown.AddOptions(optList);

            _triggerStepDropdown.Init();
            _targetStepDropdown.Init();
            _alterStepDropdown.Init();

            if (_data != null)
            {
                _conditionDropdown.value = _conditionDropdown.options.FindIndex( o => o.text == _data.condition );
                _triggerStepDropdown.Dropdown.value = _triggerStepDropdown.Dropdown.options.FindIndex(o => o.text == _data.triggerStepId);
                _targetStepDropdown.Dropdown.value =  _targetStepDropdown.Dropdown.options.FindIndex(o => o.text == _data.targetStepId);
                _alterStepDropdown.Dropdown.value =   _alterStepDropdown.Dropdown.options.FindIndex(o => o.text == _data.alterStepId);
            }
        }

        internal TriggerData GetTriggerData()
        {
            TriggerData qtd = new TriggerData();
            qtd.condition = _conditionDropdown.options[_conditionDropdown.value].text;
            qtd.triggerStepId = _triggerStepDropdown.GetSelectedText();
            qtd.targetStepId = _targetStepDropdown.GetSelectedText();
            qtd.alterStepId = _alterStepDropdown.GetSelectedText();
            return qtd;
        }
    }
}
