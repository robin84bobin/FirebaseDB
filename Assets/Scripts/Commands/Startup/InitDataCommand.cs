using System;
using Assets.Scripts.UI.Windows;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Commands.Startup
{

    public class InitPreloaderCommand : Command
    {
        public override void Execute()
        {
            SceneManager.LoadSceneAsync(Helper.Scenes.PRELOADER).completed += OnPreloaderLoaded;
        }

        private void OnPreloaderLoaded(AsyncOperation obj)
        {
            PreloaderWindow.Show();
            Complete();
        }
    }
    
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