using Scripts.Resources;
using System;

namespace Scripts.Craft
{
    [Serializable]
    public class Formula
    {
        public ResourceType[] Input;
        public ResourceType Output;
    }
}
