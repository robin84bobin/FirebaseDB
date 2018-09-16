using Assets.Scripts.Events;
using Assets.Scripts.Events.CustomEvents;
using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class QuestDropdown : MonoBehaviour {

    public event Action<string> gotoStep = delegate { };

    public const string NONE = "None";

    #region Components

    [SerializeField] public Dropdown dropdown;
    [SerializeField] private Button buttonAdd;
    [SerializeField] private Button buttonRemove;
    [SerializeField] private Button buttonGoTo;

    public Action<string> onQuestSelect = delegate { };

    #endregion


    void OnDestroy()
    {
        onQuestSelect = delegate { };
        gotoStep = delegate { };
        EventManager.Get<StorageUpdatedEvent>().Unsubscribe(OnStorageUpdate);
    }

    private void OnItemRemoved(Type type, string id)
    {
        if (type != typeof(StepData))
            return;

        string selectedId = GetSelectedText();
        if (id == selectedId)
        {
            Select(NONE);
        }

    }


    private void OnStorageUpdate(Type type)
    {
        if (type != typeof(StepData) )
            return;

        //запоминаем выделенный id
        string selectedId = GetSelectedText();
        UpdateQuestList();
        
        if (dropdown.options.Exists(o => o.text == selectedId))
            Select(selectedId);
        else //если выделенный шаг уже удалили из списка
            Select(NONE);
        //onOptionChanged(dropdown.value);
    }

    private void UpdateQuestList(bool keepSelected = true)
    {
        //get _messageViewData
        List<StepData> items = App.Data.Steps.GetAll();
        
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
        dropdown.ClearOptions();
        dropdown.AddOptions(optionsList);
    }

    internal void ResetView()
    {
        Select(NONE);
    }

    public void Init()
    {
        if (buttonAdd != null)
        {
            buttonAdd.onClick.RemoveAllListeners();
            buttonAdd.onClick.AddListener(OnAddClick);
        }

        if (buttonRemove != null)
        {
            buttonRemove.onClick.RemoveAllListeners();
            buttonRemove.onClick.AddListener(OnRemoveClick);
        }

        if (buttonGoTo != null)
        {
            buttonGoTo.onClick.RemoveAllListeners();
            buttonGoTo.onClick.AddListener(OnGoToClick);
        }

        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(onOptionChanged);
        }

        UpdateQuestList();
        
        EventManager.Get<StorageUpdatedEvent>().Subscribe(OnStorageUpdate);
    }



    private void OnGoToClick()
    {
        string id = GetSelectedText();
        if (string.IsNullOrEmpty(id) || id.Equals(NONE))
        {
            return;
        }
        gotoStep.Invoke(id);
    }

    public void Select(string id)
    {
        dropdown.SelectValue(id);
    }

    private void onOptionChanged(int index)
    {
        Dropdown.OptionData optionData = dropdown.options[index];
        onQuestSelect.Invoke(optionData.text);
    }

    private void OnRemoveClick()
    {
        string id = GetSelectedText();
        RemoveQuestMenu.Show( id );
    }

    private void OnAddClick()
    {
        CreateQuestMenu.Show(new CreateQuestMenuParams()
        {
            OnCreateSuccess = (id) => { Select(id); }
        });
    }

    public string GetSelectedText()
    {
        return dropdown.options[dropdown.value].text;
    }
}
