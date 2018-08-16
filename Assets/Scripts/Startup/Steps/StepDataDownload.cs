using Data;

namespace Startup.Steps
{
    class StepDataDownload : StepBase
    {
        public override void Begin()
        {
            GameData.Instance.Init(OnLoadSuccess, OnLoadFail);
        }

        private void OnLoadSuccess()
        {
            Complete(StepResult.Success);
        }

        private void OnLoadFail()
        {
            Complete(StepResult.Fail);
        }
        
        public override string GetFailMessage
        {
            get { return "Can't load data from server"; }
        }
    }
}
