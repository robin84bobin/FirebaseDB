using System;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Windows;
using Data;
using Data.DataTypes;
using UnityEngine;
using UnityEngine.UI;

public class SaveQuestAsMenuParams : WindowParams
{
    public SaveQuestAsMenuParams(QuestStepData q, DataItem r, Action<string> c = null)
    {
        templateData = q;
        relatedTemplateData = r;
        OnSaveSuccess = c ?? delegate { };
    }
        
    
    public QuestStepData templateData;
    public DataItem relatedTemplateData;
    public Action<string> OnSaveSuccess = delegate { };
}

public class SaveQuestAsMenu : BaseWindow {
    
    [Zenject.Inject] private Repository _repository;
    
    [SerializeField] private Text _errorText;
    [SerializeField] private Dropdown _typeDropdown;
    [SerializeField] private InputField _inputField;

    SaveQuestAsMenuParams _parameters;

    private string _newId;

    public static void Show(SaveQuestAsMenuParams param)
    {
        App.UI.Show("SaveQuestAsMenu", param);
    }

    public override void OnShowComplete(WindowParams param = null)
    {
        base.OnShowComplete(param);
        _parameters = (SaveQuestAsMenuParams)_params;
        Init();
    }

    private void Init()
    {
        List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();
        optionsList.Add(new Dropdown.OptionData(QuestStepType.MESSAGE));
        optionsList.Add(new Dropdown.OptionData(QuestStepType.TRIGGER));
        _typeDropdown.ClearOptions();
        _typeDropdown.AddOptions(optionsList);
        if (_parameters.templateData != null)
            _typeDropdown.SelectValue(_parameters.templateData.stepType);
        _typeDropdown.interactable = false;

    }


    public void OnSaveClick()
    {
        _newId = _inputField.text;

        if (string.IsNullOrEmpty(_newId))
        {
            _errorText.text = "new id is empty!";
            return;
        }

        if ( _repository.Steps.Exists(_newId))
        {
            _errorText.text = "id is already exists!";
            return;
        }

        Save();
        Hide();
    }


    private void Save()
    {
        QuestStepData item = _parameters.templateData;
        item.Id = _newId;
        item.typeId = _newId;

        if (_parameters.relatedTemplateData != null)
            _parameters.relatedTemplateData.Id = item.typeId;
        
        item.Save(_parameters.relatedTemplateData);
        
        if (_parameters.OnSaveSuccess != null)
            _parameters.OnSaveSuccess.Invoke(item.Id);
    }


    public void OnCancelClick()
    {
        Hide();
    }

    protected override void OnHide()
    {
        base.OnHide();
        _params = null; //удаляем ссылку на параметр. т.к. там ссылка на колбек
    }
}