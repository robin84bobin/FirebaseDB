using Data;

public class MessageViewData : DataItem
{
    public bool isNew;
    /// <summary>
    /// test of the message
    /// </summary>
    public string text;
    /// <summary>
    /// is the message send by user
    /// </summary>
    public bool isUserMsg;
    /// <summary>
    /// id of quest step which created the message
    /// </summary>
    public string parentQuestStepId;


}
