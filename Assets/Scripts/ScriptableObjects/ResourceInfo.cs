using Scripts.Resources;
using System;

namespace Scripts.ScriptableObjects
{
    [Serializable]
    public struct ResourceInfo
    {
        public ResourceType Type;
        public int Amount;
    }
}
