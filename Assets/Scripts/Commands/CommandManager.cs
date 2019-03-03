using System;
using System.Linq;

namespace Commands
{
    public static class CommandManager
    {
        //TODO undo stack


        
        public static void Execute(Command command)
        {
            command.Execute();
            //TODO add to undo stack
            //AddToUndoStack(command)
        }


    }

}