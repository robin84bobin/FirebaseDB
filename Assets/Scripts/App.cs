using UnityEngine;
using System.Collections;
using Data;
using Startup;

public class App : MonoBehaviour
{
    // Use this for initialization
    void Awake()
    {
        StartupController sc = new StartupController();
        sc.Start();
    }
}
