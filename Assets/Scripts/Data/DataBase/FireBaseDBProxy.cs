using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using InternalNewtonsoft.Json;
using UnityEngine;

namespace Data.DataBase
{
    internal class FireBaseDBProxy : IDataBaseProxy
    {
        FirebaseApp _firebaseApp { get; set; }
        DatabaseReference _dbRoot { get; set; }

        Action _onInitCallback;

        #region INITIALIZING

        public void Init(Action callback)
        {
            Debug.Log(ToString() + ". Init()");
            _onInitCallback = callback;
            CheckDependencies();
        }

        private void CheckDependencies()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    _firebaseApp = FirebaseApp.DefaultInstance;
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }

                SetSettings();
            });
        }

        private void SetSettings()
        {
            _firebaseApp.SetEditorDatabaseUrl("https://text-quest.firebaseio.com/");
            _dbRoot = FirebaseDatabase.DefaultInstance.RootReference;

            _onInitCallback.Invoke();
        }

        #endregion



        public void Get<T>(string collectionName, Action<Dictionary<int, T>> callback) where T : Item, new()
        {
            _dbRoot.Child(collectionName).GetValueAsync().ContinueWith(
                (t) => {
                    ConvertData(t, callback);
                }
             );
        }

        private void ConvertData<T>(Task<DataSnapshot> t, Action<Dictionary<int, T>> callback) where T : Item, new()
        {
            Dictionary<int,T> items = new Dictionary<int,T>();

            string jString = t.Result.GetRawJsonValue();
            List<T> list = JsonConvert.DeserializeObject<List<T>>(jString);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    continue;
                T item = list[i];
                if (item.id < 0)
                    item.id = i;
                items.Add(item.id, item);
            }

            callback.Invoke(items);
        }

        public void SaveCollection<T>(string collectionName, Dictionary<int, T> items) where T : Item, new()
        {
            string jString = JsonConvert.SerializeObject(items);
            _dbRoot.Child(collectionName).SetRawJsonValueAsync(jString);
        }

        public void Save<T>(string collectionName, T item, int id = -1) where T : Item, new()
        {
            if (id > 0)
                item.id = id;

            string jString = JsonConvert.SerializeObject(item);
            _dbRoot.Child(collectionName).Child(item.id.ToString()).SetRawJsonValueAsync(jString);
        }
    }
}