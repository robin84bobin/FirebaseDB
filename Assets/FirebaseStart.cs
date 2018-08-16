using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

public class FirebaseStart : MonoBehaviour {

    public static Firebase.FirebaseApp firebaseApp { get; private set; }
    public static Firebase.Database.DatabaseReference database { get; private set; }

    // Use this for initialization
    void Start ()
    {
        CheckDependencies();
    }

    private static void SetSettings()
    {
        firebaseApp.SetEditorDatabaseUrl("https://text-quest.firebaseio.com/");
        //firebaseApp.SetEditorP12FileName
        //firebaseApp.Options.AppId = "1:1036694325886:android:4afdfd116f177693";
        //firebaseApp.Options.ProjectId = "text-quest";
        database = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;

        CRUDTest();
    }

    private static void CRUDTest()
    {
        database.Child("Chapters").Child("1").GetValueAsync().ContinueWith(OnGetChild);
    }

    private static void OnGetChild(Task<DataSnapshot> task)
    {
        Debug.Log(" Chapter One: " + task.Result.Value.ToString());
    }

    private static void CheckDependencies()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp, i.e.
                firebaseApp = FirebaseApp.DefaultInstance;
                SetSettings();
                // where app is a Firebase.FirebaseApp property of your application class.

                // Set a flag here indicating that Firebase is ready to use by your
                // application.
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
}
