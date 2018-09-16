using Assets.Scripts.UI.Windows;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.Scripts.UI;
using Data;


public class CreateQuestMenuParams : WindowParams
{
    public StepData templateData;
    public Action<string> OnCreateSuccess = delegate { };
}


public class CreateQuestMenu : BaseWindow {

    [SerializeField] private Text _errorText;
    [SerializeField] private Dropdown _typeDropdown;
    [SerializeField] private InputField _inputField;

    CreateQuestMenuParams parameters;

    private string _newId;
    private string _newType;

    public static void Show(CreateQuestMenuParams params_)
    {
        App.UI.Show("CreateQuestMenu", params_);
    }

    public override void OnShowComplete(WindowParams param_ = null)
    {
        base.OnShowComplete(param_);
        parameters = (CreateQuestMenuParams)windowsParameters;
        Init();
    }

    private void Init()
    {
        _typeDropdown.onValueChanged.RemoveAllListeners();
        _typeDropdown.onValueChanged.AddListener(OnTypeChanged);

        List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();
        optionsList.Add(new Dropdown.OptionData(QuestStepType.MESSAGE.ToString()));
        optionsList.Add(new Dropdown.OptionData(QuestStepType.TRIGGER.ToString()));
        _typeDropdown.ClearOptions();
        _typeDropdown.AddOptions(optionsList);

        //
        if (parameters.templateData != null)
        {   //нельзя менять тип если сохраняем уже существующий шаг
            _typeDropdown.SelectValue(parameters.templateData.Type);
            _typeDropdown.interactable = false;
        }
        else
        {
            //_typeDropdown.gameObject.SetActive(true);
            _typeDropdown.interactable = true;
            _newType = _typeDropdown.options[0].text;
        }

    }

    private void OnTypeChanged(int val)
    {
        _newType = _typeDropdown.options[val].text;
    }

    public void OnSaveClick()
    {
        _newId = _inputField.text;

        if (string.IsNullOrEmpty(_newId))
        {
            _errorText.text = "new id is empty!";
            return;
        }

        if (App.Data.Steps.Exists(_newId))
        {
            _errorText.text = "id is already exists!";
            return;
        }

        Save();
        Hide();
    }


    private void Save()
    {
        StepData item;
        if (parameters.templateData != null)
        {
            item = parameters.templateData;
            item.Id = _newId;
        }
        else
        {
            item = CreateData();
            item.Id = _newId;
            item.Type = _newType;
        }
        
        App.Data.Steps.Set(item, _newId, true);
        //
        parameters.OnCreateSuccess.Invoke(_newId);
    }

    private StepData CreateData()
    {
        return new StepData() {Type = _newType};
    }

    public void OnCancelClick()
    {
        Hide();
    }

    protected override void OnHide()
    {
        base.OnHide();
        windowsParameters = null; //удаляем ссылку на параметр. т.к. там ссылка на колбек
    }
}

