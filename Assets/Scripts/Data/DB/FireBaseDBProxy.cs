using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Data
{
    internal class FireBaseDBProxy : IDataBaseProxy
    {
        FirebaseApp _firebaseApp { get; set; }
        DatabaseReference _database { get; set; }

        Action<string> _onInitCallback;

        public void Init(Action<string> callback)
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

            CRUDTest();
        }

        private void CRUDTest()
        {
            _database.Child("Chapters").GetValueAsync().ContinueWith(OnGetChild);
        }

        private void OnGetChild(Task<DataSnapshot> task)
        {
            string val = InternalNewtonsoft.Json.JsonConvert.SerializeObject(task.Result.Value);

            Debug.Log(" Chapters: " + val);
        }

    }
}