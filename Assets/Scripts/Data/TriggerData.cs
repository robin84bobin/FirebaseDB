using Data;

/// <summary>
/// types of conditions to switch quest trigger
/// </summary>

public class TriggerData : DataItem
{
    /// <summary>
    /// condition to switch quest trigger
    /// </summary>
    public string condition;
    /// <summary>
    ///  Id of trigger step 
    /// </summary>
    public string triggerStepId;
    /// <summary>
    /// Id of step to go if condition is true
    /// </summary>
    public string targetStepId;
    /// <summary>
    /// Id of step to go if condition is false
    /// </summary>
    public string alterStepId;

    public TriggerData Clone()
    {
        return new TriggerData()
            {
                alterStepId = this.alterStepId,
                condition = this.condition,
                Id = this.Id,
                targetStepId = this.targetStepId,
                triggerStepId = this.triggerStepId
            };
    }
}
