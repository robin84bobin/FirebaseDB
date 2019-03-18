using System;
using Commands;
using Commands.Startup;
using Data;
using Data.DataBase;
using Global;
using UnityEngine.SceneManagement;
using Zenject;

public class AppStarter
{
    public static event Action InitComplete = delegate { };

    
    public void Start()
    {
        
        CommandSequence startupCommands = new CommandSequence(
            new InitPreloaderCommand(),
            new ValidateApkCommand(), 
            new InitDataCommand(),
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