using System;
using System.Linq;

namespace Commands
{
    public static class CommandManager
    {
        //TODO undo stack

        /// <summary>
        /// do sequence of commands
        /// </summary>
        /// <param name="onSequenceComplete"></param>
        /// <param name="commands"></param>
        public static IProgressHandler ExecuteSequence(Action onSequenceComplete, params Command[] commands)
        {
            IProgressHandler progress = new ProgressHandler();
            
            for (var i = 0; i < commands.Length; i++)
            {
                if (i < commands.Length - 1)
                {
                    var nextCommand = commands[i + 1];
                    var currentCommand = i;
                    commands[i].OnComplete += () =>
                    {
                        Execute(nextCommand);
                        progress.Set((float)currentCommand/commands.Length);
                    };
                }
                else if (i == commands.Length - 1)
                    commands[i].OnComplete += () =>
                    {
                        onSequenceComplete();
                        progress.Set(1f);
                    };
                    
            }

            Execute(commands.First());

            return progress;
        }

        public static void Execute(Command command)
        {
            command.Execute();
            //TODO add to undo stack
            //AddToUndoStack(command)
        }


    }

    public class ProgressHandler : IProgressHandler
    {
        public event Action<float> OnProgress = delegate { }; 
        
        public void Set(float progress)
        {
            OnProgress.Invoke(progress);
        }
    }

    public interface IProgressHandler
    {
        /// <summary>
        /// progress value in range 0..1
        /// </summary>
        /// <param name="progress"></param>
        void Set(float progress);

        event Action<float> OnProgress;
    }
}