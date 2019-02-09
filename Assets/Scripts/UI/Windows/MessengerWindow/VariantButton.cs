using Controllers;
using UnityEngine;

public class VariantButton : MonoBehaviour {

    [SerializeField] private UIButton button;
    [SerializeField] private UILabel label;

    QuestVariantData _variantData;

    public void ResetView()
    {
        _variantData = null;
        UpdateView();
    }

    public void SetData(QuestVariantData data)
    {
        _variantData = data;
        UpdateView();
    }

    void UpdateView()
    {
        gameObject.SetActive(_variantData != null);
        if (_variantData == null) return;
        
        label.text = _variantData.text;
        button.onClick.Clear();
        button.onClick.Add(new EventDelegate(OnBtnClick));
    }

    private void OnBtnClick()
    {
        if (_variantData == null) return;
        UserQuestController.CompleteStep(_variantData.parentStepId, _variantData.variantId);
    }
}
