using System;
using UnityEngine;
using Data;
using Assets.Scripts.UI;
using Commands;
using Commands.Startup;
using Controllers;
using Global;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
    
    
    public static WindowManager UI { get; private set; }
    
    public static event Action InitComplete = delegate { };
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadSceneAsync("Preloader").completed += OnPreloaderLoaded;
    }

    private void OnPreloaderLoaded(AsyncOperation obj)
    {
        Init();
    }


    private void Init()
    {
        if (UI == null) 
            UI = GetComponent<WindowManager>() ?? gameObject.AddComponent<WindowManager>();

        Command[] startupCommands = 
        {
            new ValidateApkCommand(), 
            new InitDataCommand(),
            new InitUserDataCommand()
        };
        CommandManager.ExecuteSequence(OnInitComplete, startupCommands);
    }

    private void OnInitComplete()
    {
        
    }
    
    
    
}
