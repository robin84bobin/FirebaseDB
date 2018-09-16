using Assets.Scripts.Events;
using Assets.Scripts.Events.CustomEvents;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessengerWindow : MonoBehaviour {

    [SerializeField] private ScrollPicker _scrollPicker;
    [SerializeField] private VariantButton[] _variantButtons;

    List<MessageViewData> messages = new List<MessageViewData>();

    // Use this for initialization
    void Start()
    {
        ResetView();
        EventManager.Get<DataInitCompleteEvent>().Subscribe(Init);
    }

    private void ResetView()
    {
        foreach (var varButton in _variantButtons)
            varButton.ResetView();
    }

    private IEnumerator DelayInit()
    {
        yield return new WaitForSeconds(2f);
        Init();
    }

    private void Init()
    {
        EventManager.Get<DataInitCompleteEvent>().Unsubscribe(Init);
        EventManager.Get<NewMessageEvent>().Subscribe(ShowNewMessage);
        EventManager.Get<TypeMessageCompleteEvent>().Subscribe(OnTypeMessageComplete);
        InitMessages();
        _scrollPicker.Init();

        _scrollPicker.SetData(messages);

        if (messages.Count > 0)
        {
            InitVariantButtons(messages[messages.Count - 1]);
        }
        else
        {
            App.UserStepsController.GoToStep("start");
        }
       
    }

    private void OnTypeMessageComplete(MessageViewData messageViewData)
    {
        InitVariantButtons(messageViewData);
        ShowNextMessage();
    }

    public void GoToEditor()
    {
        SceneManager.LoadScene("QuestEditor");
    }
    
    public void Restart()
    {
        App.Data.ClearUserData();

        messages.Clear();
        _scrollPicker.SetData(messages);
        App.UserStepsController.GoToStep("start");
    }

    private void InitMessages()
    {
        IEnumerable<MessageViewData> items = App.Data.UserMessageHistory.GetAll();
        messages.AddRange(items);
        
    }

    private void ShowNewMessage(MessageViewData messageViewData)
    {
        messageViewData.isNew = true;
        if (messageQueue.Count <= 0)
        {
            ShowMessage(messageViewData);
        }
        else
        messageQueue.Enqueue(messageViewData);

    }

    private void ShowMessage(MessageViewData messageViewData)
    {
        if (messageViewData == null)
        {
            return;
        }
        _scrollPicker.AddDataItem(messageViewData, true);
        InitVariantButtons(messageViewData);
    }

    Queue<MessageViewData> messageQueue = new Queue<MessageViewData>();
    private void ShowNextMessage()
    {
        if (messageQueue.Count <= 0)
            return;
        
        MessageViewData messageViewData = messageQueue.Dequeue();
        ShowMessage(messageViewData);
    }

    private void InitVariantButtons(MessageViewData messageViewData)
    {
        //init variant buttons
        ResetVariantButtons();
        if (!messageViewData.isUserMsg)
        {
            QuestMessageData questStep = App.Data.QuestMessageStep[messageViewData.parentQuestStepId];
            for (int i = 0; i < _variantButtons.Length; i++)
            {
                if (i < questStep.variants.Length)
                    _variantButtons[i].SetData(questStep.variants[i]);
            }
        }
    }

    private void ResetVariantButtons()
    {
        for (int i = 0; i < _variantButtons.Length; i++)
            _variantButtons[i].ResetView();
    }
}
