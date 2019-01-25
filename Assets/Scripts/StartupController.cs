using System;
using Commands;
using Commands.Startup;

public class StartupController
{
    public event Action<float> onProgress = delegate { };
    public event Action onComplete = delegate { };
    
    public void Init()
    {
        Command[] startupCommands = 
        {
            new ValidateApkCommand(), 
            new InitDataCommand(),
            new InitUserDataCommand()
        };
        
        CommandManager
            .ExecuteSequence(OnInitComplete, startupCommands)
            .OnProgress += onProgress;
    }
    
    void OnInitComplete()
    {
        onComplete.Invoke();
    }
}