using Controllers;

namespace Commands.Startup
{
    public class ValidateApkCommand : Command
    {
        public override void Execute()
        {
            //TODO
            Complete();
        }
    }
    
    public class InitUserDataCommand : Command
    {
        public override void Execute()
        {
            UserQuestController.Init();
            Complete();
        }
    }
}