using Data;

namespace Assets.Scripts.Factories.DataFactories.JsonFactories
{
    public abstract class AbstractJsonFactory
    {
        public abstract DataItem Create(string jsonString_);
     }
}