using System;
using System.Collections.Generic;
using Assets.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    public class WindowManager : MonoBehaviour 
    {
        public Canvas UiRoot;

        void Awake()
        {
            Init ();
            SceneManager.sceneUnloaded += onChangeScene;
        }

        private void onChangeScene(Scene scene)
        {
            HideAllWindows();
            UiRoot.transform.DestroyChildren();
        }


        private List<BaseWindow> _shownWindows;
        void Init()
        {
            UiRoot = FindObjectOfType<Canvas>();
            if (UiRoot == null) 
                throw new NullReferenceException(this + ":: no UIRoot found in scene ");
            DontDestroyOnLoad(UiRoot.gameObject);
            DontDestroyOnLoad(this.gameObject);
            _shownWindows = new List<BaseWindow>();
        }

        public GameObject LoadPrefab(string prefabName)
        {
            string path = string.Format ("UI/Windows/{0}", prefabName);
            GameObject go = Resources.Load(path) as GameObject;
            go.name = prefabName;
            return go;
        }

        public void Show(string windowName, WindowParams param = null)
        {
            HideAllWindows ();
            GameObject windowGo = LoadPrefab(windowName);
            BaseWindow newWindow = InstantiateWindow(windowGo);
            newWindow.OnShowComplete(param);
            _shownWindows.Add(newWindow);
        }

        BaseWindow InstantiateWindow(GameObject windowGameObject)
        {
            GameObject parent = UiRoot.gameObject;
            GameObject go = GameObject.Instantiate(windowGameObject) as GameObject;
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

        public void HideWindow (BaseWindow window)
        {
            _shownWindows.Remove(window);

            window.transform.parent = null;
            GameObject.Destroy(window.gameObject);
        }
    }
}
