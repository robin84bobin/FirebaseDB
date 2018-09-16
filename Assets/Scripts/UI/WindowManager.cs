using System.Collections.Generic;
using Assets.Scripts.UI.Windows;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class WindowManager : MonoBehaviour 
    {
        public Canvas uiRoot;

        void Awake()
        {
            Init ();
        }

        private List<BaseWindow> _shownWindows;
        void Init()
        {
            GameObject.DontDestroyOnLoad(uiRoot.gameObject);
            GameObject.DontDestroyOnLoad(this.gameObject);
            _shownWindows = new List<BaseWindow>();
        }

        public GameObject LoadPrefab(string prefabName_)
        {
            string path = string.Format ("UI/Windows/{0}", prefabName_);
            GameObject go = Resources.Load(path) as GameObject;
            go.name = prefabName_;
            return go;
        }

        public void Show(string windowName_, WindowParams param_ = null)
        {
            HideAllWindows ();
            GameObject windowGo = LoadPrefab(windowName_);
            BaseWindow newWindow = InstantiateWindow(windowGo);
            newWindow.OnShowComplete(param_);
            _shownWindows.Add(newWindow);
        }

        BaseWindow InstantiateWindow(GameObject windowGo_)
        {
            GameObject parent = uiRoot.gameObject;
            GameObject go = GameObject.Instantiate(windowGo_) as GameObject;
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
                go.SetActive(true);
            }

            return go.GetComponent<BaseWindow> ();
        }

        void HideAllWindows ()
        {
            for (int i = 0; i < _shownWindows.Count; i++) {
                _shownWindows[i].Hide();
            }
        }

        public void HideWindow (BaseWindow window_)
        {
            _shownWindows.Remove(window_);

            window_.transform.parent = null;
            GameObject.Destroy(window_.gameObject);
        }
    }
}
