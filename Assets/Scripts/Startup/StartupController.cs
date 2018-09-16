using Startup.Steps;
using System;
using System.Collections.Generic;

namespace Startup
{
    public class StartupController
    {
        private static List<StepBase> _steps = new List<StepBase>();
        private static Dictionary<Type, List<Action>> _stepCallbacks = new Dictionary<Type, List<Action>>();
        private int _currentStep;

        public void Start()
        {
            AddStep(new StepDataDownload());
            _steps[0].Begin();
        }


        private void AddStep(StepBase step)
        {
            step.OnComplete = NextStep;
            _steps.Add(step);
        }

        private void NextStep(StepResult result)
        {
            if (result == StepResult.FAIL)
            {
                if (_steps[_currentStep].RepeatOnFail)
                {
                    _steps[_currentStep].Begin();
                }

                return;
            }

            _currentStep++;

            if (_currentStep >= _steps.Count)
            {
                StepsComplete();
                return;
            }

            _steps[_currentStep].Begin();
        }

        private void StepsComplete()
        {
        }

  
        public static List<Action> GetStepCallbacks(Type stepType)
        {
            return _stepCallbacks.ContainsKey(stepType) ? _stepCallbacks[stepType] : new List<Action>();
        }
        
    }
}