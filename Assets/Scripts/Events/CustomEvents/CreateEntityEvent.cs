using Data;
using Events;
using UnityEngine;

namespace Assets.Scripts.Events.CustomEvents
{
    public sealed class CreateEntityEvent : GameParamEvent<CreationParams>
    {
    }

    public struct CreationParams
    {
        public Vector3 position;
        public Vector3 direction;
        public DataItem data;
    }
}