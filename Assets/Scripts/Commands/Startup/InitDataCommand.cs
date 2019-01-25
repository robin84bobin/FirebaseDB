using Data;

namespace Commands.Startup
{
    public class InitDataCommand : Command
    {
        public override void Execute()
        {
            Repository.OnInitComplete += OnInitComplete;
            Repository.Init();
        }

        private void OnInitComplete()
        {
            Complete();
        }
    }
}