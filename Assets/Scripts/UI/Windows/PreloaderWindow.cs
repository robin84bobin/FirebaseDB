using Assets.Scripts.Events;
using Assets.Scripts.Events.CustomEvents;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Windows
{
    public class PreloaderWindow : BaseWindow 
    {
        public Text loadingStatusText;
        public Button startButton;

        public static void Show()
        {
            App.UI.Show("PreloaderWindow");
        }

        public override void OnShowComplete(WindowParams param_ = null)
        {
            base.OnShowComplete (param_);

            EventManager.Get<LoadProgressEvent> ().Subscribe (OnLoadProgressEvent);
            EventManager.Get<DataInitCompleteEvent> ().Subscribe (OnLoadComplete);

            loadingStatusText.text = "please wait...";
            startButton.onClick.AddListener (OnStartButton);
            startButton.gameObject.SetActive(false);
        }


        void OnLoadProgressEvent (string message_)
        {
            loadingStatusText.text = message_;
        }

        void OnLoadComplete ()
        {
            loadingStatusText.text = "Loading Complete!";
            startButton.gameObject.SetActive(true);
        }

        protected override void OnHide ()
        {
            base.OnHide ();
            EventManager.Get<LoadProgressEvent> ().Unsubscribe (OnLoadProgressEvent);
            EventManager.Get<DataInitCompleteEvent> ().Unsubscribe (OnLoadComplete);
        }

        public void OnStartButton ()
        {
            Hide ();
           // Main.Inst.game.LoadCurrentLevel();
        }
    }
}
