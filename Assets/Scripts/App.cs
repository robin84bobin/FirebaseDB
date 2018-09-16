using UnityEngine;
using Data;
using Assets.Scripts.UI;

public class App : MonoBehaviour
{
    public static WindowManager UI { get; private set; }
    public static UserStepsController UserStepsController { get; private set; }
    public static DataManager Data { get; private set; }
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        Data = new DataManager();
        Data.Init(OnLoadSuccess);
    }

    private void OnLoadFail()
    {
       // throw new NotImplementedException();
    }

    private void OnLoadSuccess()
    {
        UserStepsController = new UserStepsController();
        UserStepsController.Init();
    }
    
    
    
}
