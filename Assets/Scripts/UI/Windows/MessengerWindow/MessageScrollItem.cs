using Assets.Scripts.Events;
using System.Collections;
using UnityEngine;

public class MessageScrollItem : BaseScrollItem {

    public UIWidget container;
    public UIWidget backCommon;
    public UIWidget backUser;
    public UILabel message;

    MessageViewData _messageViewData;



    public override void UpdateView(object dataObject)
    {
        StopAllCoroutines();

        _messageViewData = (MessageViewData)dataObject;
        message.text = _messageViewData.text;
        if (_messageViewData.isNew && !_messageViewData.isUserMsg)
        {
            _messageViewData.isNew = false;
            StartCoroutine(TypeMessage(_messageViewData.text));
        }
        else
        {
            message.text = _messageViewData.text;
            //EventManager.Get<TypeMessageCompleteEvent>().Publish(_messageViewData);
        }
       
        SetMode(_messageViewData.isUserMsg);
    }

    private void SetMode(bool isUser)
    {
        backCommon.gameObject.SetActive(!isUser);
        backUser.gameObject.SetActive(isUser);
    }

    public override void SetEmpty(bool isEmpty)
    {
        container.gameObject.SetActive(!isEmpty);
    }

    public override void SetItemIndex(int contentIndex) { }
    public override void SetSelected(object dataObject) { }

    IEnumerator TypeMessage(string text)
    {
        int chrLeft = text.Length;
        while (chrLeft > 0)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.01f, 0.07f));
            chrLeft--;
            message.text = text.Substring(0, text.Length - chrLeft);
        }

        EventManager.Get<TypeMessageCompleteEvent>().Publish(_messageViewData);
    }
}
