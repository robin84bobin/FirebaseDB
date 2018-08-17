using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using InternalNewtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Data
{
    internal class FireBaseDBProxy : IDataBaseProxy
    {
        FirebaseApp _firebaseApp { get; set; }
        DatabaseReference _database { get; set; }

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
            _database = FirebaseDatabase.DefaultInstance.RootReference;

            _onInitCallback.Invoke();
        }

        #endregion



        public void Get<T>(string sourceName, Action<Dictionary<int, T>> callback) where T : Item, new()
        {
            _database.Child(sourceName).GetValueAsync().ContinueWith(
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
                item.Id = i;
                items.Add(item.Id, item);
            }

            callback.Invoke(items);
        }
    }
}