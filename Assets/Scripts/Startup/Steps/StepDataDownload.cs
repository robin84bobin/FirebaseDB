using Data;

namespace Startup.Steps
{
    class StepDataDownload : StepBase
    {
        public override void Begin()
        {
            App.Data.Init(OnLoadSuccess);
        }

        private void OnLoadSuccess()
        {
            Complete(StepResult.SUCCESS);
        }

        private void OnLoadFail()
        {
            Complete(StepResult.FAIL);
        }
        
        public override string GetFailMessage
        {
            get { return "Can't load _messageViewData from server"; }
        }
    }
}
