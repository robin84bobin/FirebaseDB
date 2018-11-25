namespace Data
{
    public class QuestTriggerStepData : DataItem
    {
        public TriggerData[] Triggers = new TriggerData[] { };

        internal  QuestTriggerStepData Clone()
        {
            QuestTriggerStepData data = new QuestTriggerStepData();
            data.Triggers = new TriggerData[Triggers.Length];
            for (int i = 0; i < Triggers.Length; i++)
            {
                data.Triggers[i] = Triggers[i].Clone();
            }
            return data;
        }
    }

}