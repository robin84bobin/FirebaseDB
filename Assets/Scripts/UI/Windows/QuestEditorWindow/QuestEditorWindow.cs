﻿using Assets.Scripts.Events;
using Assets.Scripts.Events.CustomEvents;
using Assets.Scripts.UI.Windows;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class QuestEditorWindow : BaseWindow {

    #region COMPONENTS

    [SerializeField] private Dropdown _dropdownType;
    [SerializeField] private QuestDropdown _dropdownId;

    [SerializeField] private MessageStepEditor _messageStepEditor;
    [SerializeField] private TriggerStepEditor _triggerStepEditor;
    private AbstractStepEditor _currentEditor;

    #endregion

    string _currentQuestId = string.Empty;
    private StepData _currentQuestData;

    public static void Show()
    {
        App.UI.Show("QuestEditorWindow");
    }

    // Use this for initialization
    void Start ()
    {
        EventManager.Get<DataInitCompleteEvent>().Subscribe(Init);
    }

    private void Init()
    {
        //init questDropDowns
        //from some of them we can switch current quest step to show by "gotoStep" Buttons
        var questDropDowns = GetComponentsInChildren<QuestDropdown>(true);
        foreach (var dropDown in questDropDowns)
        {
            dropDown.Init();
            dropDown.gotoStep += SelectQuest;
        }

        //main questDropDown directly switches the current quest step to show
        _dropdownId.onQuestSelect += OnSelectQuest;
        if (string.IsNullOrEmpty(_currentQuestId))
        {
            SelectQuest(_currentQuestId);
        }
    }

    private void SelectQuest(string id)
    {
        _dropdownId.Select(id);
    }

    #region BUTTONS CALLBACKS

    public void OnSwitchToGameClick()
    {
        if (_currentEditor != null)
            _currentEditor.SaveData();
        SceneManager.LoadScene("Main");
    }

    public void OnSaveClick()
    {
        _currentEditor.SaveData();
    }

    public void OnSaveAsClick()
    {
        CreateQuestMenu.Show(
            new CreateQuestMenuParams()
            {
                templateData = _currentEditor.GetData(),
            }
       );
    }

    public void OnRemoveClick()
    {
        if (string.IsNullOrEmpty(_currentQuestId) || _currentQuestId.Equals(QuestDropdown.NONE))
        {
            return;
        }
        RemoveQuestMenu.Show(_currentQuestId);
    }


    /// <summary>
    /// костыль что не сохранять в истории шаги при переходе по кнопкам Prev/Next 
    /// </summary>
    bool saveStory = true;

    public void OnNextClick()
    {
        saveStory = false;
        int nextId = _currentStoryIndex + 1;
        if (nextId >= 0 &&  nextId < _selectedQuestStory.Count)
        {
            string id = _selectedQuestStory[nextId];
            Debug.Log("id > " + id);
            SelectQuest(id);
            _currentStoryIndex = nextId;
        }
    }

    public void OnPrevClick()
    {
        saveStory = false;
        int prevId = _currentStoryIndex - 1;
        if (prevId >= 0 && prevId < _selectedQuestStory.Count)
        {
            string id = _selectedQuestStory[prevId];
            Debug.Log("id > " + id);
            SelectQuest(id);
            _currentStoryIndex = prevId;
        }
    }

    #endregion

    private List<string> _selectedQuestStory = new List<string>();
    private int _currentStoryIndex = 0;


    private void OnSelectQuest(string Id)
    {
        if (_currentEditor != null)
        {
            //_currentEditor.SaveData();
        }

        _currentQuestId = Id;

        //save story
        if (saveStory)
        {
            //если текущий шаг не последний - удаляем те, что после него
            if (_currentStoryIndex < _selectedQuestStory.Count - 1)
            {
                _selectedQuestStory.RemoveRange(_currentStoryIndex, _selectedQuestStory.Count - _currentStoryIndex);
            }
            _selectedQuestStory.Add(_currentQuestId);
            _currentStoryIndex++;
            Debug.Log("Add Id : " + _currentQuestId);
        }
        saveStory = true;
        ///

        _currentQuestData = App.Data.Steps[Id];

        if (_currentQuestData == null)
        {
            _messageStepEditor.gameObject.SetActive(false);
            _triggerStepEditor.gameObject.SetActive(false);
            _currentEditor = null;
            return;
        }

        switch(_currentQuestData.Type)
        {
            case QuestStepType.MESSAGE:  ShowCommonEditorTab(); break;
            case QuestStepType.TRIGGER:  ShowTriggetEditorTab(); break;
        }

    }

    private void ShowTriggetEditorTab()
    {
        _messageStepEditor.gameObject.SetActive(false);
        _triggerStepEditor.gameObject.SetActive(true);

        _currentEditor = _triggerStepEditor;
        _currentEditor.Init(_currentQuestData);
    }

    private void ShowCommonEditorTab()
    {
        _messageStepEditor.gameObject.SetActive(true);
        _triggerStepEditor.gameObject.SetActive(false);

        _currentEditor = _messageStepEditor;
        _currentEditor.Init(_currentQuestData);
    }

}