using UnityEngine;
using Assets.Scripts.UI;
using Data;
using Data.DataBase;
using Global;
using UnityEngine.SceneManagement;
using Zenject;

public class App : MonoBehaviour
{
    public static WindowManager UI { get; private set; }

    

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        GlobalEvents.LoadScene.Subscribe(SceneManager.LoadScene);
        
        if (UI == null) 
            UI = GetComponent<WindowManager>() ?? gameObject.AddComponent<WindowManager>();
        
        //ProjectContext.Instance.Container.BindInterfacesAndSelfTo<Repository>().AsSingle();
        //ProjectContext.Instance.Container.Bind<IDataBaseProxy>().To<FireBaseDbProxy>().AsSingle();
        
        new AppStarter().Start();
        
    }

}