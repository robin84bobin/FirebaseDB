using Data;
using UnityEngine;

namespace Assets.Scripts.Events.CustomEvents
{
    public sealed class CreateObjectEvent : GameParamEvent<CreateParams>
    {
    }

    public class CreateParams
    {
        public Vector3 rotation = Vector3.zero;
        public Vector3 position = Vector3.zero;
        public Vector3 scale = Vector3.one;
        //public BaseEntityModel model;
        public DataItem data;
    }
}