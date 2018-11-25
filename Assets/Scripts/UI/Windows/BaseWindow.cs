using UnityEngine;

namespace Assets.Scripts.UI.Windows
{
    public class BaseWindow : MonoBehaviour 
    {
        protected WindowParams windowsParameters;

        public void Hide ()
        {
            OnHide();
            App.UI.HideWindow(this);
        }

        public virtual void OnShowComplete(WindowParams param = null)
        {
            windowsParameters = param;
        }

        protected virtual void OnHide()
        {
        }
    }
}