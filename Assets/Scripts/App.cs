using UnityEngine;
using Assets.Scripts.UI;
using Global;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
    public static WindowManager UI { get; private set; }

    

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        GlobalEvents.LoadScene.Subscribe(SceneManager.LoadScene);
        
        if (UI == null) 
            UI = GetComponent<WindowManager>() ?? gameObject.AddComponent<WindowManager>();
        
        new AppStarter().Start();
        
    }

}