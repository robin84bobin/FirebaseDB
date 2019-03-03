using Assets.Scripts.UI.Windows.InfoWindows;
using Global;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Windows
{
    public class PreloaderWindow : BaseWindow 
    {
        public Text loadingStatusText;

        public static void Show()
        {
            App.UI.Show("PreloaderWindow");
        }

        public override void OnShowComplete( WindowParams param = null)
        {
            base.OnShowComplete ( param);

            GlobalEvents.OnLoadingProgress.Subscribe (OnLoadProgressEvent);

            loadingStatusText.text = "please wait...";
        }


        void OnLoadProgressEvent (string message_)
        {
            loadingStatusText.text = message_;
        }

   

        protected override void OnHide ()
        {
            base.OnHide ();
            GlobalEvents.OnLoadingProgress.Unsubscribe(OnLoadProgressEvent);
        }

        public void OnStartButton ()
        {
            Hide ();
        }
    }
}
