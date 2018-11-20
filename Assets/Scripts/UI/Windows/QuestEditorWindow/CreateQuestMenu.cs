using Assets.Scripts.UI.Windows;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.Scripts.UI;
using Data;
using Data.DataTypes;


public class CreateQuestMenuParams : WindowParams
{
    public QuestStepData templateData;
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
        optionsList.Add(new Dropdown.OptionData(QuestStepType.MESSAGE));
        optionsList.Add(new Dropdown.OptionData(QuestStepType.TRIGGER));
        _typeDropdown.ClearOptions();
        _typeDropdown.AddOptions(optionsList);

        //
        if (parameters.templateData != null)
        {   //нельзя менять тип если сохраняем уже существующий шаг
            _typeDropdown.SelectValue(parameters.templateData.stepType);
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
        QuestStepData item = parameters.templateData ?? new QuestStepData() { stepType = _newType };;
        item.Id = _newId;
        item.stepType = _newType;
        item.typeId = _newId;
        item.Save();
        
        if (parameters.OnCreateSuccess != null)
            parameters.OnCreateSuccess.Invoke(item.Id);
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

