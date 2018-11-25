using Data.DataTypes;
using UnityEngine;

public abstract class AbstractStepEditor : MonoBehaviour
{
    internal abstract void Init(QuestStepData questStepData);
    protected abstract void GrabDataFromUI();
    internal abstract QuestStepData GetData();
    internal abstract void SaveData();

    protected QuestStepData QuestStepData;

    public abstract void SaveAs();
}