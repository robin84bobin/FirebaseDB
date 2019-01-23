using UnityEngine;

namespace Assets.Scripts.UI.Windows
{
    public class BaseWindow : MonoBehaviour 
    {
        protected WindowParams _params;

        public void Hide ()
        {
            OnHide();
            App.UI.HideWindow(this);
        }

        public virtual void OnShowComplete(WindowParams param = null)
        {
            _params = param;
        }

        protected virtual void OnHide()
        {
            _params = null;
        }
    }
}