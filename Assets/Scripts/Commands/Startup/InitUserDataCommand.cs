using Controllers;

namespace Commands.Startup
{
    public class InitUserDataCommand : Command
    {
        public override void Execute()
        {
            UserQuestController.Init();
            Complete();
        }
    }
}