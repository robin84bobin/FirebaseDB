using System;
using System.Collections.Generic;
using Data;
using Data.DataBase;
using Data.DataTypes;
using Global;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.QuestEditorWindow.Components
{
    public class QuestDropdown : MonoBehaviour {

        [Zenject.Inject] private Repository _repository;
        
        public event Action<string> GotoStep = delegate { };

        public const string NONE = "None";

        #region Components

        [SerializeField] public Dropdown Dropdown;
        [SerializeField] private Button _buttonAdd;
        [SerializeField] private Button _buttonRemove;
        [SerializeField] private Button _buttonGoTo;
        

        public event Action<string> OnQuestSelect = delegate { };

        #endregion

        public void Init()
        {
            if (_buttonAdd != null)
            {
                _buttonAdd.onClick.RemoveAllListeners();
                _buttonAdd.onClick.AddListener(OnAddClick);
            }

            if (_buttonRemove != null)
            {
                _buttonRemove.onClick.RemoveAllListeners();
                _buttonRemove.onClick.AddListener(OnRemoveClick);
            }

            if (_buttonGoTo != null)
            {
                _buttonGoTo.onClick.RemoveAllListeners();
                _buttonGoTo.onClick.AddListener(OnGoToClick);
            }

            if (Dropdown != null)
            {
                Dropdown.onValueChanged.RemoveAllListeners();
                Dropdown.onValueChanged.AddListener(onOptionChanged);
            }

            UpdateQuestList();

            GlobalEvents.OnAddStorageItem.Subscribe(OnAddItem);
            GlobalEvents.OnRemoveStorageItem.Subscribe(OnRemoveItem);
            
            Resources.LoadAsync<>()
        }

        private void OnRemoveItem(string id)
        {
            //запоминаем выделенный id
            string selectedId = GetSelectedText();
            UpdateQuestList();

            if (!Dropdown.options.Exists(o => o.text == selectedId))
                Select(NONE);
        }

        private void OnAddItem(DataItem item)
        {
            if (item.GetType() != typeof(QuestStepData) )
                return;
            
            UpdateQuestList();

            if (_selectOnAdd)
            {
                _selectOnAdd = false;
                SelectItem(item);
            }
        }

        private void SelectItem(DataItem item)
        {
            //запоминаем выделенный id
            string selectedId = GetSelectedText();

            if (Dropdown.options.Exists(o => o.text == item.Id))
            {
                Select(item.Id);
                return;
            }

            if (Dropdown.options.Exists(o => o.text == selectedId))
                Select(selectedId);
            else //если выделенный шаг уже удалили из списка
                Select(NONE);
        }


        private void UpdateQuestList(bool keepSelected = true)
        {
            //get _messageViewData
            List<QuestStepData> items =  _repository.Steps.GetAll();
        
            //create option list
            List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();
            optionsList.Add(new Dropdown.OptionData(NONE));
            for (int i = 0; i < items.Count; i++)
                optionsList.Add(new Dropdown.OptionData(items[i].Id));
            //sort list
            /*optionsList.Sort(delegate (Dropdown.OptionData x, Dropdown.OptionData y) {
            if (x.text == null && y.text == null) return 0;
            else if (x.text == null) return -1;
            else if (y.text == null) return 1;
            else return x.text.CompareTo(y.text);
            });*/
            //fill list
            Dropdown.ClearOptions();
            Dropdown.AddOptions(optionsList);
        }

        
        internal void ResetView()
        {
            Select(NONE);
        }


        private void OnGoToClick()
        {
            string id = GetSelectedText();
            if (string.IsNullOrEmpty(id) || id.Equals(NONE))
            {
                return;
            }
            GotoStep.Invoke(id);
        }

        
        public void Select(string id)
        {
            Dropdown.SelectValue(id);
        }

        
        private void onOptionChanged(int index)
        {
            Dropdown.OptionData optionData = Dropdown.options[index];
            OnQuestSelect.Invoke(optionData.text);
        }

        
        private void OnRemoveClick()
        {
            string id = GetSelectedText();
            RemoveQuestMenu.Show( id );
        }

        /// <summary>
        /// выделяем свежесозданный шаг только в том questDropdown из которого вызвана команда на его создание
        /// </summary>
        private bool _selectOnAdd = false;
        private void OnAddClick()
        {
            var p = new CreateQuestMenuParams();
            p.OnCreateCancel = () =>
            {
                _selectOnAdd = false;
            };
            CreateQuestMenu.Show(p);

            _selectOnAdd = true;
        }

        
        public string GetSelectedText()
        {
            var o = Dropdown.options[Dropdown.value];
            return o!=null?o.text:NONE;
        }
        
        
        void OnDestroy()
        {
            OnQuestSelect = delegate { };
            GotoStep = delegate { };
            GlobalEvents.OnAddStorageItem.Unsubscribe(OnAddItem);
            GlobalEvents.OnRemoveStorageItem.Unsubscribe(OnRemoveItem);
            //GlobalEvents.OnStorageUpdated.Unsubscribe(OnStorageUpdate);
        }
    }
}
