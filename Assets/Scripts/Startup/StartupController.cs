using Startup.Steps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Startup
{
    public class StartupController
    {
      //  [SerializeField] private GameObject _loadingObject;
      //  [SerializeField] private GameObject _noConnectionObject;
      //  [SerializeField] private GameObject _updateGameObject;
       // [SerializeField] private GameObject _banGameObject;

        private const string LEVEL_GARAGE = "Garage";
        private const string LEVEL_MENU = "MainMenu";

        private static List<StepBase> _steps = new List<StepBase>();
        private static Dictionary<Type, List<Action>> _stepCallbacks = new Dictionary<Type, List<Action>>();
        private int _currentStep;
        //private UILabel _loadingLabel;

        public void Start()
        {
            //string nativeId = AccountController.GetId();
            //string outerId = AccountController.outerId;
           /* if (string.IsNullOrEmpty(outerId))
            {
                yield break;
            }*/


            //FsmVariables.GlobalVariables.GetFsmString("VersionGame").Value = BuildData.Data.GameVersion;
            
          //  _noConnectionObject.SetActive(false);
           // _updateGameObject.SetActive(false);
           // _banGameObject.SetActive(false);

            //FLogger.Message("StartupController Start");
           // _currentStep = 0;
           // _loadingObject.SetActive(true);
           // _loadingLabel = _loadingObject.GetComponent<UILabel>();

            //_loadingLabel.text = "0%";
            //делай шаги получай гиги
            //AddStep(new StepConnectionCheck(_noConnectionObject));
            //AddStep(new StepServerTimeGet());
            //AddStep(new StepLanguageSet());
            //AddStep(new StepConfigDownload());
            //AddStep(new StepLocalizationDownload());
            //steps.Add(new DebugLongLoadingStep());
#if !APPTUTTI
            //AddStep(new StepAPKValidation(_updateGameObject));
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            AddAndroidSteps();
#endif           
            //AddStep(new StepVersionCheck(_updateGameObject));
            //AddStep(new StepAuthorization(nativeId, outerId));
            //AddStep(new StepCheckMarathon());
            AddStep(new StepDataDownload());
            //AddStep(new StepPurchaseInit());
            //AddStep(new StepSocialConnect(gameObject));
            //AddStep(new StepBanCheck(_banGameObject));
            //AddStep(new StepSynchronizaton());
            //AddStep(new StepInitSession());
            //AddStep(new StepBadBoyCheck());
            //AddStep(new StepAnalyticsInit());//steps.Add(new InitAnalyticsStep());  //under construction
            //AddStep(new StepUnityPurchaseInit());

            //AddStep(new StepAppFacadeInit());

            _steps[0].Begin();
        }

 /*       private void AddAndroidSteps()
        {
            AddStepCallback(typeof(StepPermissions), delegate
            {
                FLogger.Message("InitAccountController!");
                AccountController.instance.Init();
            });   
            AddStep(new StepPermissions(gameObject));
            if (BuildData.Data.UsingOBB)
            {
                AddStep(new StepAndroidOBBLoad(gameObject));
            }
        }*/

        private void AddStep(StepBase step)
        {
            step.OnComplete = NextStep;
            _steps.Add(step);
        }

        private void NextStep(StepResult result)
        {
            //FLogger.ResultMessage(result == StepResult.Success, string.Format("Step {0}. \"{1}\" complete with <b>{2}</b> result", _currentStep + 1, _steps[_currentStep].StepName, result));

            if (result == StepResult.Fail)
            {
                if (_steps[_currentStep].RepeatOnFail)
                {
                    _steps[_currentStep].Begin();
                }
                else
                {
            //        MessagePanel.Instance.ShowError(_steps[_currentStep].GetFailMessage);
            //        _loadingLabel.gameObject.SetActive(false);
                }

                return;
            }

            _currentStep++;
          //  FLogger.Message(_currentStep + " / " + _steps.Count + " * 100 + %");
          //  FLogger.Message(_currentStep * 1f / _steps.Count * 100 + "%");
          //  _loadingLabel.text = Convert.ToInt32(_currentStep * 1f / _steps.Count * 100) + "%";

            if (_currentStep >= _steps.Count)
            {
                StepsComplete();
                return;
            }

           // FLogger.Message(string.Format("Go to next step: {0}. \"{1}\"", _currentStep + 1, _steps[_currentStep].StepName));
            _steps[_currentStep].Begin();
        }

        private void StepsComplete()
        {
            //FLogger.Message("All steps complete");

            //TODO включить тренировку, когда починят
            //if (ObscuredPrefs.GetInt("TrainingState") != 2)
            //    DALProxy.Request(DAL.Common.UserDataApi.GetTrainigState(), OnGetTrainingState, null, false);
            //else
            //    LevelLoader.Instance.LoadLevel(LEVEL_GARAGE, _loadingLabel);

            //Events.GameStartComplete();
        }

   /*     private void OnGetTrainingState(TrainingStateResponse response)
        {
           int state = (int)response.State;

            switch(state)
            {
                case 0:
                    LevelLoader.Instance.LoadLevel(LEVEL_MENU, _loadingLabel);
                    break;

                case 1:
                    Events.OnTrainingStart();
                    break;

                case 2:
                    LevelLoader.Instance.LoadLevel(LEVEL_GARAGE, _loadingLabel);
                    break;
            }
        }*/

        //==    Experimental. Use with very caution
        public static void AddStepCallback(Type stepType, Action callback)
        {
           // FLogger.Message("Adding " + stepType);
            if (!stepType.IsSubclassOf(typeof(StepBase)))
            {
            //    FLogger.FailMessage(string.Format("{0} is not a subclass of {1}", stepType, typeof(StepBase)));
                return;
            }

            StepBase step = _steps.FirstOrDefault(s => s.GetType() == stepType);
            if (step != null && step.Completed)     //Если шаг уже завершён - выполняем каллбек и выходим
            {
                callback.Invoke();
                return;
            }

            if (_stepCallbacks.ContainsKey(stepType))      //Если у шага уже есть каллбеки - добавляем к существующему списку
            {
                _stepCallbacks[stepType].Add(callback);
            }
            else                                    //Если нет - создаём новый список для этого шага
            {
                _stepCallbacks.Add(stepType, new List<Action>{callback});
            }
        }

        public static List<Action> GetStepCallbacks(Type stepType)
        {
            return _stepCallbacks.ContainsKey(stepType) ? _stepCallbacks[stepType] : new List<Action>();
        }
        
    }
}