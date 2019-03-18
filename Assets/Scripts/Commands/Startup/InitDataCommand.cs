using System;
using System.Runtime.InteropServices;
using Assets.Scripts.UI.Windows;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

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
        [Inject] private Repository _repository;

        public override void Execute()
        {
            _repository.OnInitComplete += OnInitComplete;
            _repository.Init();
        }

        private void OnInitComplete()
        {
            Complete();
        }
    }
}