using System;
using UnityEngine;
using Data;
using Assets.Scripts.UI;
using Controllers;
using Global;

public class App : MonoBehaviour
{
    public static WindowManager UI { get; private set; }
    public static UserQuestController UserQuestController { get; private set; }
    public static DataManager Data { get; private set; }
    
    public static event Action InitComplete = delegate { };
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Init();
        
        Data = new DataManager();
        Data.Init(OnLoadSuccess);
    }
    

    private void Init()
    {
        if (UI == null) 
            UI = GetComponent<WindowManager>() ?? gameObject.AddComponent<WindowManager>();
    }

    private void OnLoadFail()
    {
       // throw new NotImplementedException();
    }

    private void OnLoadSuccess()
    {
        UserQuestController = new UserQuestController();
        UserQuestController.Init();
        
        GlobalEvents.OnDataInited.Publish();
    }
    
    
    
}
