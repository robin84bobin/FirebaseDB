using Data.DataTypes;
using UnityEngine;

abstract public class AbstractStepEditor : MonoBehaviour
{
    internal abstract void Init(QuestStepData questStepData);
    protected abstract void GrabDataFromUI();
    internal abstract QuestStepData GetData();
    internal abstract void SaveData();

    protected QuestStepData QuestStepData;
}