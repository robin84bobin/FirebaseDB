using System;
using System.Globalization;
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

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (UI == null) 
            UI = GetComponent<WindowManager>() ?? gameObject.AddComponent<WindowManager>();
        
        
        CommandSequence startupCommands = new CommandSequence(
            new InitPreloaderCommand(),
            new ValidateApkCommand(), 
            new InitDataCommand(),
            new InitUserDataCommand()
        );
        startupCommands.OnComplete += ()=>
        {
            InitComplete.Invoke();
        };
        startupCommands.OnProgress += p =>
        {
            GlobalEvents.OnLoadingProgress.Publish((p * 100) + "%");
        };
        CommandManager.Execute(startupCommands);
    }
}