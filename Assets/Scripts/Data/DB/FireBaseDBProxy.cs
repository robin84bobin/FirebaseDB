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
            _onInitCallback = callback;
            CheckDependencies();
        }

        private void CheckDependencies()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    _firebaseApp = FirebaseApp.DefaultInstance;
                    SetSettings();
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

        }

        #endregion



        public void Get<T>(string sourceName, Action<List<T>> callback) where T : Item, new()
        {
            _database.Child("sourceName").GetValueAsync().ContinueWith(
                (t) => { ProcessDataGet<T>(t, callback); }
             );
        }

        private void ProcessDataGet<T>(Task<DataSnapshot> t, Action<List<T>> callback) where T : Item, new()
        {
            List<T> resultList = new List<T>();

            string jString = t.Result.GetRawJsonValue();
            resultList = JsonConvert.DeserializeObject<List<T>>(jString);

            callback.Invoke(resultList);
        }
    }
}