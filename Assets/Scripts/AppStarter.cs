using System;
using Commands;
using Commands.Startup;
using Data;
using Data.DataBase;
using Global;
using UnityEngine.SceneManagement;

public class AppStarter
{
    public static event Action InitComplete = delegate { };

    // 
    private InitDataCommand _initDataCommand;

    // 
    private TestClass _testObject;

    public AppStarter(InitDataCommand initCommand, TestClass test)
    {
        _initDataCommand = initCommand;
        _testObject = test;
    }
    
    public void Start()
    {
        CommandSequence startupCommands = new CommandSequence(
            new InitPreloaderCommand(),
            new ValidateApkCommand(), 
            _initDataCommand,
            new InitUserDataCommand()
        );
        
        startupCommands.OnComplete += ()=>
        {
            GlobalEvents.LoadScene.Publish(Helper.Scenes.EDITOR);
        };
        
        startupCommands.OnProgress += p =>
        {
            GlobalEvents.OnLoadingProgress.Publish((p * 100) + "%");
        };
        
        CommandManager.Execute(startupCommands);
    }
}