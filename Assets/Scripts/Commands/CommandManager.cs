using System;
using System.Linq;

namespace Assets.Scripts.Commands
{
    public class CommandManager
    {
        //TODO undo stack

        /// <summary>
        /// do sequence of commands
        /// </summary>
        /// <param name="onSequenceComplete"></param>
        /// <param name="commands"></param>
        public static void ExecuteSequence(Action onSequenceComplete, params Command[] commands)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                if (i < commands.Length - 1)   
                {
                    Command nextCommand = commands[i + 1];
                    commands[i].OnComplete += () => Execute(nextCommand);
                }
                else if (i == commands.Length - 1)
                    commands[i].OnComplete += () => { onSequenceComplete.Invoke(); };
            }

            Execute(commands.First());
        }

        public static void Execute(Command command)
        {
            command.Execute();
            //TODO add to undo stack
            //AddToUndoStack(command)
        }

        public static void ExecuteAsync(Command command, Action completeCallback)
        {
            command.OnComplete += completeCallback;
            Execute(command);
        }


    }
}
