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

            GlobalEvents.Get<LoadProgressEvent> ().Subscribe (OnLoadProgressEvent);
            App.InitComplete += OnLoadComplete;

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
            GlobalEvents.Get<LoadProgressEvent> ().Unsubscribe (OnLoadProgressEvent);
            App.InitComplete -= OnLoadComplete;
        }

        public void OnStartButton ()
        {
            Hide ();
           // Main.Inst.game.LoadCurrentLevel();
        }
    }
}
