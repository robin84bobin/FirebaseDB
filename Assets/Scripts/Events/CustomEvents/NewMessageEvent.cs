using Assets.Scripts.Events;
using Events;

public class NewMessageEvent : GameParamEvent<MessageViewData>
{
}

public class TypeMessageCompleteEvent : GameParamEvent<MessageViewData> { }
