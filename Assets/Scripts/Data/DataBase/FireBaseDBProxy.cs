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
    internal class FireBaseDbProxy : IDataBaseProxy
    {
        private FirebaseApp FirebaseApp { get; set; }
        private DatabaseReference DbRoot { get; set; }

        private Action _onInitCallback;

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
                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseApp = FirebaseApp.DefaultInstance;
                }
                else
                {
                    Debug.LogError(string.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }

                SetSettings();
            });
        }

        private void SetSettings()
        {
            FirebaseApp.SetEditorDatabaseUrl("https://text-quest.firebaseio.com/");
            DbRoot = FirebaseDatabase.DefaultInstance.RootReference;

            _onInitCallback.Invoke();
        }

        #endregion


        public void Get<T>(string collectionName, Action<Dictionary<string, T>> callback, bool createIfNotExist = true) where T : DataItem, new()
        {
            var test = DbRoot.Child(collectionName);
            
            Debug.Log(this.ToString() + ":: Get (" + collectionName + ") = " + test.ToString());
            
            DbRoot.Child(collectionName).GetValueAsync().ContinueWith(
                t => ConvertData(t, callback)
            );
        }

        private void CreateIfNotExist<T>(string collectionName, Action<Dictionary<string, T>> callback) where T : DataItem, new()
        {
            throw new NotImplementedException();
        }

        private void ConvertData<T>(Task<DataSnapshot> t, Action<Dictionary<string, T>> callback) where T : DataItem, new()
        {
            var items = new Dictionary<string, T>();
            var jString = t.Result.GetRawJsonValue();
            var list = JsonConvert.DeserializeObject<List<T>>(jString);

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    continue;
                var item = list[i];

                items.Add(item.Id, item);
            }

            callback.Invoke(items);
        }

        public void SaveCollection<T>(string collectionName, Dictionary<string, T> items) where T : DataItem, new()
        {
            var jString = JsonConvert.SerializeObject(items);
            DbRoot.Child(collectionName).SetRawJsonValueAsync(jString);
        }

        public void Save<T>(string collectionName, T item, string id = "") where T : DataItem, new()
        {
            item.Id = id;

            var jString = JsonConvert.SerializeObject(item);
            DbRoot.Child(collectionName).Child(item.Id.ToString()).SetRawJsonValueAsync(jString);
        }
    }
}