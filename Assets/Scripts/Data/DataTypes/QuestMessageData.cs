namespace Data
{
    public class QuestMessageData : DataItem
    {
        /// <summary>
        /// message displayed before player could choose option to answer
        /// </summary>
        public string text = string.Empty;//{ get; internal set; }
        /// <summary>
        /// Array of variants to answer
        /// </summary>
        public QuestVariantData[] variants = new QuestVariantData[] { };// { get; private set; }
        /// <summary>
        /// Delay to wait after step conplete
        /// </summary>
        public float delayAfterStep;// { get; private set; }
        /// <summary>
        /// Message displayed on waiting after step
        /// </summary>
        public float delayAfterStepMessage;// { get; private set; }
    }

}