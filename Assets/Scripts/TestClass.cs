using System;
using UnityEngine;
using Zenject;

public class TestClass : ITickable, IInitializable, IDisposable
{
    public void Tick()
    {
        Debug.Log($"{this} : Tick : {Time.time}");
    }

    public void Initialize()
    {
        Debug.Log(this + " : Initialize " + Time.time);
    }

    public void Dispose()
    {
        Debug.Log(this + " : Dispose " + Time.time);
    }
}