using Data;
using UnityEngine;

abstract public class AbstractStepEditor : MonoBehaviour
{
    internal abstract void Init(StepData stepData);
    protected abstract void GrabDataFromUI();
    internal abstract StepData GetData();
    internal abstract void SaveData();

    protected StepData _stepData;
}