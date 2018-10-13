using System;
using Assets.Scripts.Events;
using Assets.Scripts.Events.CustomEvents;
using UnityEngine;
using Data;
using Assets.Scripts.UI;
using Controllers;

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

        
        GlobalEvents.DataInitedEvent.Publish();
        
        //GlobalEvents.Get<DataInitCompleteEvent>().Publish();
    }
    
    
    
}
