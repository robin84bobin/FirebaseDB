using UnityEngine;
using UnityEngine.UI;



namespace Assets.Scripts.UI.Windows.InfoWindows
{
    public class InfoWindow : BaseWindow
    {
        [SerializeField] private Text _messageText;
        
        private InfoWindowParams parameters;


        public static void Show(string message_)
        {
            InfoWindowParams param = new InfoWindowParams (message_);
            Show (param);
        }

        public static void Show(InfoWindowParams param_ = null)
        {
            App.UI.Show(	"InfoWindow", param_);
        }
        
        public override void OnShowComplete(WindowParams param = null)
        {
            base.OnShowComplete(param);
            parameters = (InfoWindowParams)windowsParameters;
            _messageText.text = parameters.Message;
        }
        
    }
    
    public class InfoWindowParams : WindowParams
    {
        public string Message { get; private set; }

        public InfoWindowParams (string message_)
        {
            Message = message_;    
        }
    }
}
