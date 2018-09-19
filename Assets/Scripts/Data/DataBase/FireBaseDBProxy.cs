using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using InternalNewtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions.Must;

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


        public void Get<T>(string collection, Action<Dictionary<string, T>> callback, bool createIfNotExist = true) where T : DataItem, new()
        {
            var test = DbRoot.Child(collection); 
            Debug.Log(this.ToString() + ":: Get (" + collection + ") = " + test.ToString());
            
            DbRoot.Child(collection).GetValueAsync().ContinueWith(
                t => GetDataHandler(collection, t, callback, createIfNotExist)
            );
        }


        private void GetDataHandler<T>(
            string collection, 
            Task<DataSnapshot> t,
            Action<Dictionary<string, T>> callback, 
            bool createIfNotExist = true) where T : DataItem, new()
        {
            if (t.IsFaulted )
            {
                Debug.LogError(this.ToString() + ":: Get Data Failed. Exception: " + t.Exception.ToString());
                Debug.LogError(this.ToString() + ":: Get Data Failed. Result: " + t.Result);
                return;
            }
            
            if (t.IsCanceled )
            {
                Debug.LogError(this.ToString() + ":: Get Data IsCanceled. Exception: " + t.Exception.ToString());
                Debug.LogError(this.ToString() + ":: Get Data IsCanceled. Result: " + t.Result);
                return;
            }

            if (!t.Result.Exists && createIfNotExist)
            {
                Debug.LogError(string.Concat(this.ToString(), ":: Collection not exists: ",collection));
                Dictionary<string, T> rawDict = new Dictionary<string, T>();
                rawDict.Add("emptyItem", new T());
                
                SaveCollection(collection, rawDict, () =>
                {
                    Get(collection, callback, createIfNotExist);
                });
                return;
            }
            
            var jString = t.Result.GetRawJsonValue();
            var items = JsonConvert.DeserializeObject<Dictionary<string, T>>(jString);
            
            foreach (var item in items)
                item.Value.Id = item.Key;

            if (callback != null)
                callback.Invoke(items);
        }

        public void SaveCollection<T>(string collection, Dictionary<string, T> items, Action callback = null) where T : DataItem, new()
        {
            var jString = JsonConvert.SerializeObject(items);
            DbRoot.Child(collection).SetRawJsonValueAsync(jString).ContinueWith(
                t =>
                {
                    if (callback != null) callback.Invoke();
                }
            );
        }

        public void Save<T>(string collectionName, T item, string id = "") where T : DataItem, new()
        {
            item.Id = id;
            var jString = JsonConvert.SerializeObject(item);
            
            //TODO. Почитать возможно надо использовать Push() или Transaction? 
            DbRoot.Child(collectionName).Child(item.Id).SetRawJsonValueAsync(jString);
        }
    }
}