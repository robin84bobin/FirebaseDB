using System;
using InternalNewtonsoft.Json.Linq;
using UnityEngine;

namespace Data
{
    public class GameData
    {
        IDataBaseProxy dbProxy;

        private static GameData _instance;
        public static GameData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameData();
                }
                return _instance;
            }
        }


        public event Action<JToken> OnJsonDataLoaded = delegate { };
        public event Action OnDataParseComplete = delegate { };
        public event Action OnInitSuccess = delegate { };
        public event Action OnInitFail = delegate { };

        public void Init(Action onSuccess, Action onFail)
        {
            OnInitSuccess += onSuccess;
            OnInitFail += onFail;

            dbProxy = new FireBaseDBProxy();
            dbProxy.Init(onDataBaseInit);

            Debug.Log(ToString() + " :: Init : " + dbProxy.ToString());
        }

        private void onDataBaseInit(string json)
        {
            Debug.Log(ToString() + " :: OnGetData :");
            Debug.Log(json);

            try
            {
                JObject j = JObject.Parse(json);
                OnJsonDataLoaded.Invoke(j["dictionary"]);
                OnDataParseComplete.Invoke();
                OnInitSuccess.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("OnGetData error: " + e.Message);
                OnInitFail.Invoke();
            }
        }


    }
}



