using UnityEngine;
using System.Collections;
using Data;
using Startup;
using System;
using Assets.Scripts.Commands;

public class App : MonoBehaviour
{
    // Use this for initialization
    void Awake()
    {
        GameData.Instance.Init(OnLoadSuccess, OnLoadFail);
    }

    private void OnLoadFail()
    {
        throw new NotImplementedException();
    }

    private void OnLoadSuccess()
    {
        throw new NotImplementedException();
    }
}
