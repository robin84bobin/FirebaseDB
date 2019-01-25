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
    
    public static event Action<float> OnInitProgress = delegate { };
    public static event Action InitComplete = delegate { };
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (UI == null) 
            UI = GetComponent<WindowManager>() ?? gameObject.AddComponent<WindowManager>();
        
        SceneManager.LoadSceneAsync(Helper.Scenes.PRELOADER).completed += OnPreloaderLoaded;
    }

    private void OnPreloaderLoaded(AsyncOperation obj)
    {
        InitApp();
    }

    private void InitApp()
    {
        Command[] startupCommands = 
        {
            new ValidateApkCommand(), 
            new InitDataCommand(),
            new InitUserDataCommand()
        };
    
        CommandManager
            .ExecuteSequence(InitComplete, startupCommands)
            .OnProgress += OnInitProgress;
    }
}