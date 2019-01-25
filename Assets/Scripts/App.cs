using System;
using UnityEngine;
using Data;
using Assets.Scripts.UI;
using Controllers;
using Global;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
    public static StartupController startupController { get; private set; }
    public static WindowManager UI { get; private set; }
    
    public static event Action InitComplete = delegate { };
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (UI == null) 
            UI = GetComponent<WindowManager>() ?? gameObject.AddComponent<WindowManager>();
        
        startupController = new StartupController();
        
        SceneManager.LoadSceneAsync(Helper.Scenes.PRELOADER).completed += OnPreloaderLoaded;
    }

    private void OnPreloaderLoaded(AsyncOperation obj)
    {
        startupController.Init();
    }





    
    
    
}