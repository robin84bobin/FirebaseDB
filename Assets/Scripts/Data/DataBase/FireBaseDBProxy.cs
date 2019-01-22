using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.UI.Windows.InfoWindows;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Global;
using InternalNewtonsoft.Json;
using UnityEngine;

namespace Data.DataBase
{
    internal class FireBaseDbProxy : IDataBaseProxy
    {
        private FirebaseApp FirebaseApp { get; set; }
        private DatabaseReference DbRoot { get; set; }

        public event Action OnInitialized = delegate { };

        private object _lockObject = new object();

        #region INITIALIZING

        public void Init()
        {
            Debug.Log(ToString() + ". Init()");
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
            FirebaseApp.SetEditorDatabaseUrl("https://textquest-test.firebaseio.com/");
            DbRoot = FirebaseDatabase.DefaultInstance.RootReference;
            DbRoot.ValueChanged += OnRootChanges;
            OnInitialized.Invoke();
        }

        private void OnRootChanges(object sender, ValueChangedEventArgs e)
        {
            string json = e.Snapshot.GetRawJsonValue();
            GlobalEvents.OnBackup.Publish(json);
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
                if (callback != null)
                    callback.Invoke(new Dictionary<string, T>());
                
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
                delegate
                {
                    if (callback != null) callback.Invoke();
                }
            );
        }

        public void Save<T>(string collection, T item, string id = "", Action<T> callback = null) where T : DataItem, new()
        {
            if (string.IsNullOrEmpty(item.Id)) 
                item.Id = id;
            var jString = JsonConvert.SerializeObject(item);
            
            //TODO. Почитать возможно надо использовать Push() или Transaction? 
            
                       
            DbRoot.Child(collection).Child(item.Id).SetRawJsonValueAsync(jString).ContinueWith(
                delegate(Task t)
                {
                    if (t.Exception != null)
                    {
                        var message = this + " Error Saving:" + jString + " \n" + t.Exception.ToString();
                        Debug.LogError(message);
                        InfoWindow.Show(message);
                        return;
                    }
    
                    if (callback != null) 
                        callback.Invoke(item);
                }
            );
        }

        public void Remove<T>(string collection, string id = "", Action<string> callback = null) 
        {
            DbRoot.Child(collection).Child(id).RemoveValueAsync().ContinueWith(
                delegate(Task t)
                {
                    if (t.Exception != null)
                    {
                        var message = this + " Error Removing:" + id + " \n" + t.Exception.ToString();
                        Debug.LogError(message);
                        InfoWindow.Show(message);
                        return;
                    }

                    if (callback != null)
                        callback.Invoke(id);
                }
            );
        }


    }
}