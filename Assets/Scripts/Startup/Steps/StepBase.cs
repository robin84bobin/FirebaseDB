using System;

namespace Startup.Steps
{
    public enum StepResult
    {
        Success,
        Fail
    }

    public abstract class StepBase
    {
        public delegate void CompleteAction(StepResult result);

        public string StepName { get { return GetType().Name; } }
        public bool RepeatOnFail { get; protected set; }
        public bool Completed { get; protected set; }
        public CompleteAction OnComplete;

        public abstract void Begin();
        public abstract string GetFailMessage { get; }
        
        protected virtual void Complete(StepResult result)
        {
            Completed = true;
            foreach (Action callback in StartupController.GetStepCallbacks(GetType()))
            {
                callback.Invoke();
            }

            if (OnComplete != null)
            {
                OnComplete.Invoke(result);
            }
        }
    }
}
