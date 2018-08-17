using System;

namespace Assets.Scripts.Commands
{
    public class CommandManager
    {
        //TODO undo stack

        /// <summary>
        /// do sequence of commands
        /// </summary>
        /// <param name="completeCallback"></param>
        /// <param name="commands"></param>
        public static void DoSequence(Action completeCallback, params Command[] commands)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                if (i < commands.Length - 1)   
                {
                    commands[i].OnComplete += () => Do(commands[i + 1]);
                }
                else if (i == commands.Length - 1)
                {
                    commands[i].OnComplete += completeCallback;
                }
            }

            Do(commands[0]);
        }

        public static void Do(Command c)
        {
            c.Do();
            //TODO add to undo stack
        }

        public static void DoAsync(Command c, Action completeCallback)
        {
            c.OnComplete += completeCallback;
            Do(c);
        }


    }
}
