using Assets.Scripts.UI.Windows;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Windows.InfoWindows;
using Data;
using Data.DataTypes;
using Commands;
using Commands.Data;

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

    public static void Show()
    {
        App.UI.Show("CreateQuestMenu");
    }

    public override void OnShowComplete(WindowParams param = null)
    {
        base.OnShowComplete(param);
        Init();
    }

    private void Init()
    {
        _inputField.onValueChanged.AddListener(Validate); 
        
        _typeDropdown.onValueChanged.RemoveAllListeners();
        _typeDropdown.onValueChanged.AddListener(OnTypeChanged);

        List<Dropdown.OptionData> optionsList = new List<Dropdown.OptionData>();
        optionsList.Add(new Dropdown.OptionData(QuestStepType.MESSAGE));
        optionsList.Add(new Dropdown.OptionData(QuestStepType.TRIGGER));
        _typeDropdown.ClearOptions();
        _typeDropdown.AddOptions(optionsList);

        _typeDropdown.interactable = true;
        _newType = _typeDropdown.options[0].text;
    }

    private void Validate(string inputId)
    {
        Regex regex = new Regex(@"\$+\.+\]+\[+");

        if (regex.IsMatch(inputId))
            _errorText.text = (" Недопустимый символ: " + regex.ToString());
        else
            _errorText.text = string.Empty;
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
        CommandManager.Execute(new CreateQuestStepCommand(_newId, _newType));
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

