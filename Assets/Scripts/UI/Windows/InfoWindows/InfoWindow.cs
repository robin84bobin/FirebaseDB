using UnityEngine;

namespace Assets.Scripts.UI.Windows.InfoWindows
{
    public class InfoWindow : MonoBehaviour {

        public static void Show(string message_)
        {
            InfoWindowParams param = new InfoWindowParams (message_);
            Show (param);
        }

        public static void Show(InfoWindowParams param_ = null)
        {
            App.UI.Show(	"InfoWindow", param_);
        }
    }
}
