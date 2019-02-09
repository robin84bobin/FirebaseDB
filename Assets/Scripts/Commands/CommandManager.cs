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
        /// <param name="commands"></param>
        public static void ExecuteSequence(params Command[] commands)
        {
            ExecuteSequence(null, null, commands);
        }

        
        /// <summary>
        /// do sequence of commands
        /// </summary>
        /// <param name="onSequenceComplete"></param>
        /// <param name="commands"></param>
        public static void ExecuteSequence(Action onSequenceComplete, params Command[] commands)
        {
            ExecuteSequence(onSequenceComplete, null, commands);
        }
        
        
        /// <summary>
        /// do sequence of commands
        /// </summary>
        /// <param name="onSequenceComplete"></param>
        /// <param name="progressCallback"></param>
        /// <param name="commands"></param>
        public static void ExecuteSequence(Action onSequenceComplete, Action<float> progressCallback, params Command[] commands)
        {
            for (var i = 0; i < commands.Length; i++)
            {
                if (i < commands.Length - 1)
                {
                    var nextCommand = commands[i + 1];
                    var currentCommand = i;
                    commands[i].OnComplete += () =>
                    {
                        Execute(nextCommand);
                        progressCallback.Invoke((float)currentCommand/commands.Length);
                    };
                }
                else if (i == commands.Length - 1)
                    commands[i].OnComplete += () =>
                    {
                        onSequenceComplete();
                        progressCallback.Invoke(1f);
                    };
                    
            }

            Execute(commands.First());
        }

        public static void Execute(Command command)
        {
            command.Execute();
            //TODO add to undo stack
            //AddToUndoStack(command)
        }


    }

}