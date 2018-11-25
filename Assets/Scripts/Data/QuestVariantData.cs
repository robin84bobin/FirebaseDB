public class QuestVariantData 
{
    /// <summary>
    /// variant id in parent quest step
    /// </summary>
    public int variantId;
    /// <summary>
    /// parent step id
    /// </summary>
    public string parentStepId;
    /// <summary>
    /// Id of step to go to
    /// </summary>
    public string targetStepId;// { get; private set; }
    /// <summary>
    /// Option message
    /// </summary>
    public string text;// { get; private set; }

    public QuestVariantData Clone()
    {
        return new QuestVariantData()
        {
            variantId = variantId,
            parentStepId = parentStepId,
            targetStepId = targetStepId,
            text = text
        };
    }
}