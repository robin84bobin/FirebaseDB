using Data;

namespace Commands.Startup
{
    public class InitDataCommand : Command
    {
        public override void Execute()
        {
            DataManager.OnInitComplete += OnInitComplete;
            DataManager.Init();
        }

        private void OnInitComplete()
        {
            Complete();
        }
    }
}