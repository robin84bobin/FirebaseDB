namespace Assets.Scripts.UI.Windows.InfoWindows
{
    public class InfoWindowParams : WindowParams
    {
        public string Message { get; private set; }

        public InfoWindowParams (string message_)
        {
            Message = message_;    
        }
    }
}

